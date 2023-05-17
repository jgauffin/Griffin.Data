using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Griffin.Data.Migrations;

/// <summary>
///     A collection of scripts for a specific migration.
/// </summary>
public class ScriptCollection
{
    private readonly string _migrationName;
    private readonly Assembly _scriptAssembly;
    private readonly Dictionary<int, string> _versions = new();

    /// <summary>
    /// </summary>
    /// <param name="migrationName">Name of the migrations category.</param>
    /// <param name="scriptAssembly">Assembly that the scripts can be found in.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public ScriptCollection(string migrationName, Assembly scriptAssembly)
    {
        _migrationName = migrationName ?? throw new ArgumentNullException(nameof(migrationName));
        _scriptAssembly = scriptAssembly ?? throw new ArgumentNullException(nameof(scriptAssembly));
    }

    /// <summary>
    ///     There are no scripts.
    /// </summary>
    public bool IsEmpty => _versions.Count == 0;

    /// <summary>
    ///     Add a new script.
    /// </summary>
    /// <param name="version">Version of this script.</param>
    /// <param name="scriptName">Script name (manifest resource name).</param>
    /// <exception cref="ArgumentOutOfRangeException">version is not specified.</exception>
    /// <exception cref="ArgumentNullException">Script name is not specified.</exception>
    public void AddScriptName(int version, string scriptName)
    {
        if (version < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(version));
        }

        _versions[version] = scriptName ?? throw new ArgumentNullException(nameof(scriptName));
    }

    /// <summary>
    ///     Execute a specific version.
    /// </summary>
    /// <param name="connection">Open connection.</param>
    /// <param name="version">Version to execute.</param>
    public void Execute(IDbConnection connection, int version)
    {
        if (connection == null)
        {
            throw new ArgumentNullException(nameof(connection));
        }

        if (version <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(version));
        }

        var script = LoadScript(version);
        var sb = new StringBuilder();
        var sr = new StringReader(script);
        while (true)
        {
            var line = sr.ReadLine();
            if (line == null)
            {
                break;
            }

            if (!line.Equals("go"))
            {
                sb.AppendLine(line);
                continue;
            }

            ExecuteSql(connection, sb.ToString());
            sb.Clear();
        }

        //do the remaining part of the script (or everything if GO was not used).
        ExecuteSql(connection, sb.ToString());

        ExecuteSql(connection, $"UPDATE DatabaseSchema SET Version={version} WHERE Name = '{_migrationName}'");
    }

    /// <summary>
    ///     Get highest implemented version.
    /// </summary>
    /// <returns></returns>
    public int GetHighestVersion()
    {
        if (_versions.Count == 0)
        {
            return -1;
        }

        return _versions.Max(x => x.Key);
    }

    /// <summary>
    ///     Load the script contents.
    /// </summary>
    /// <param name="version">Version to load.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentOutOfRangeException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public string LoadScript(int version)
    {
        if (version <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(version));
        }

        if (!_versions.TryGetValue(version, out var scriptName))
        {
            throw new InvalidOperationException($"Version was not found: {version} for migration {_migrationName}. Available versions: {string.Join(", ", _versions.Keys)}");
        }

        var res = _scriptAssembly.GetManifestResourceStream(scriptName);
        if (res == null)
        {
            throw new InvalidOperationException(
                $"Failed to find script {scriptName} for migration {_migrationName}.  Available versions: {string.Join(", ", _versions.Keys)}");
        }

        return new StreamReader(res).ReadToEnd();
    }

    private static void ExecuteSql(IDbConnection connection, string sql)
    {
        var parts = sql.Split(new[] { "\r\ngo\r\n", "\r\nGO\r\n", "\r\ngo;\r\n" },
            StringSplitOptions.RemoveEmptyEntries);
        foreach (var part in parts)
        {
            using var transaction = connection.BeginTransaction();
            using (var cmd = connection.CreateCommand())
            {
                cmd.Transaction = transaction;
                cmd.CommandText = part;
                cmd.ExecuteNonQuery();
            }

            transaction.Commit();
        }
    }
}

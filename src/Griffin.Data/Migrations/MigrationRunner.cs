using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using System.Reflection;
using Griffin.Data.Helpers;

namespace Griffin.Data.Migrations;

/// <summary>
///     Database script migrations.
/// </summary>
public class MigrationRunner
{
    private readonly Func<IDbConnection> _connectionFactory;
    private readonly Assembly _scriptAssembly;
    private readonly ScriptCollection _scripts;

    /// <summary>
    /// </summary>
    /// <param name="connectionFactory"></param>
    /// <param name="migrationName">Migration name (the prefix in the migration files)</param>
    /// <exception cref="ArgumentNullException">One or more arguments are not specified.</exception>
    public MigrationRunner(Func<IDbConnection> connectionFactory, string migrationName)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
        MigrationName = migrationName ?? throw new ArgumentNullException(nameof(migrationName));
        _scriptAssembly = GetScriptAssembly(migrationName);
        _scripts = new ScriptCollection(migrationName, _scriptAssembly);
    }

    /// <summary>
    ///     Migration name (the prefix in the migration files)
    /// </summary>
    public string MigrationName { get; }

    /// <summary>
    ///     The character used to separate the filename into parts. Default is '_'.
    /// </summary>
    public char PartSeparator { get; set; } = '_';

    /// <summary>
    ///     Gt current version.
    /// </summary>
    /// <returns></returns>
    public int GetCurrentSchemaVersion()
    {
        string[] scripts =
        {
            @"IF OBJECT_ID (N'DatabaseSchema', N'U') IS NULL
                        BEGIN
                            CREATE TABLE [dbo].DatabaseSchema (
                                [Version] int not null,
                                [Name] varchar(50) NOT NULL
                            );
                        END",
            @"IF COL_LENGTH('DatabaseSchema', 'Name') IS NULL
                    BEGIN
                        ALTER TABLE DatabaseSchema ADD [Name] varchar(50) NULL;
                    END;",
            @"UPDATE DatabaseSchema SET Name = 'DbScripts' WHERE Name IS NULL"
        };

        using var con = _connectionFactory();
        foreach (var script in scripts)
        {
            using var cmd = con.CreateCommand();
            cmd.CommandText = script;
            cmd.ExecuteNonQuery();
        }

        using (var cmd = con.CreateCommand())
        {
            try
            {
                cmd.CommandText = "SELECT Version FROM DatabaseSchema WHERE Name = @name";
                cmd.AddParameter("name", MigrationName);
                var result = cmd.ExecuteScalar();
                if (result is null)
                {
                    return -1;
                }

                return (int)result;
            }
            catch (DbException ex)
            {
                var prop = ex.GetType().GetProperty("Number");
                if (prop == null)
                {
                    throw;
                }

                var value = (int)prop.GetValue(ex, null);

                //invalid object name
                if (value == 208)
                {
                    return -1;
                }

                throw;
            }
        }
    }

    /// <summary>
    ///     Get most recently applied migration version.
    /// </summary>
    /// <returns></returns>
    public int GetLatestSchemaVersion()
    {
        EnsureLoaded();
        return _scripts.GetHighestVersion();
    }

    /// <summary>
    ///     Run all migrations.
    /// </summary>
    public void Run()
    {
        EnsureLoaded();
        if (!CanSchemaBeUpgraded())
        {
            return;
        }

        UpgradeDatabaseSchema();
    }

    /// <summary>
    ///     Check if the current DB schema is out of date compared to the embedded schema resources.
    /// </summary>
    protected bool CanSchemaBeUpgraded()
    {
        var version = GetCurrentSchemaVersion();
        var embeddedSchema = GetLatestSchemaVersion();
        return embeddedSchema > version;
    }

    /// <summary>
    ///     Load all scripts.
    /// </summary>
    protected void LoadScripts()
    {
        var names =
            _scriptAssembly
                .GetManifestResourceNames()
                .Where(x => x.Contains($"{MigrationName}{PartSeparator}v") &&
                            x.EndsWith(".sql", StringComparison.OrdinalIgnoreCase));

        foreach (var name in names)
        {
            var pos = name.IndexOf($"{PartSeparator}v", StringComparison.OrdinalIgnoreCase) + 2; //2 extra for ".v"
            var endPos = name.IndexOf(PartSeparator.ToString(), pos, StringComparison.Ordinal);
            var versionStr = name.Substring(pos, endPos - pos);
            var version = int.Parse(versionStr);
            _scripts.AddScriptName(version, name);
        }
    }

    /// <summary>
    ///     Upgrade schema
    /// </summary>
    /// <param name="toVersion">-1 = latest version</param>
    protected void UpgradeDatabaseSchema(int toVersion = -1)
    {
        if (toVersion == -1)
        {
            toVersion = GetLatestSchemaVersion();
        }

        var currentSchema = GetCurrentSchemaVersion();
        if (currentSchema < 1)
        {
            currentSchema = 0;
        }

        for (var version = currentSchema + 1; version <= toVersion; version++)
        {
            try
            {
                using var con = _connectionFactory();
                _scripts.Execute(con, version);
                if (version != 1)
                {
                    continue;
                }

                using var cmd = con.CreateCommand();
                cmd.CommandText =
                    $"INSERT INTO DatabaseSchema (Name, Version) VALUES('{MigrationName}', 1)";
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                throw new InvalidDataException($"Failed to run script {MigrationName} v" + version, ex);
            }
        }
    }

    private void EnsureLoaded()
    {
        if (_scripts.IsEmpty)
        {
            LoadScripts();
        }
    }

    private Assembly GetScriptAssembly(string migrationName)
    {
        if (migrationName == null)
        {
            throw new ArgumentNullException(nameof(migrationName));
        }

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            if (assembly.IsDynamic)
            {
                continue;
            }

            if (assembly.GetName().Name?.StartsWith("System", StringComparison.OrdinalIgnoreCase) == true)
            {
                continue;
            }

            if (assembly.GetName().Name?.StartsWith("Microsoft", StringComparison.OrdinalIgnoreCase) == true)
            {
                continue;
            }

            var names = assembly.GetManifestResourceNames();
            foreach (var name in names)
            {
                var pos = name.IndexOf($"{migrationName}{PartSeparator}v", StringComparison.OrdinalIgnoreCase);
                if (pos != -1)
                {
                    continue;
                }

                var pos2 = name.IndexOf(".sql", StringComparison.OrdinalIgnoreCase);
                if (pos2 == -1)
                {
                    continue;
                }

                if (pos2 < pos)
                {
                    continue;
                }

                return assembly;
            }
        }

        throw new InvalidOperationException($"Failed to find scripts for migration '{migrationName}'.");
    }
}

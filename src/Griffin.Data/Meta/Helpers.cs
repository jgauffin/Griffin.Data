using System;
using System.Data.Common;
using System.Text.RegularExpressions;
using Griffin.Data.Meta.Engines;

namespace Griffin.Data.Meta;

/// <summary>
///     Helper methods.
/// </summary>
public class Helpers
{
    /// <summary>
    /// </summary>
    /// <param name="providerName">ADO.NET provider to use.</param>
    /// <param name="connectionString">Connection string to the DB.</param>
    public Helpers(string providerName, string connectionString)
    {
        ProviderName = providerName ?? throw new ArgumentNullException(nameof(providerName));
        ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    /// <summary>
    ///     Prefix to add to every generated class.
    /// </summary>
    public string ClassPrefix { get; set; } = "";

    /// <summary>
    ///     Suffix to add to every generated class.
    /// </summary>
    public string ClassSuffix { get; set; } = "";

    /// <summary>
    ///     Connection string to open a connection with.
    /// </summary>
    protected string ConnectionString { get; }

    /// <summary>
    ///     Selected SQL provider.
    /// </summary>
    protected string ProviderName { get; }

    private TableCollection LoadTables()
    {
        var factory = DbProviderFactories.GetFactory(ProviderName);
        using var conn = factory.CreateConnection();
        if (conn == null)
        {
            throw new InvalidOperationException("Failed to create connection instance.");
        }

        conn.ConnectionString = ConnectionString;
        conn.Open();

        SchemaReader? reader = factory.GetType().Name switch
        {
            // MySql
            "MySqlClientFactory" => new MySqlSchemaReader(),
            // SQL CE
            "SqlCeProviderFactory" => new SqlServerCeSchemaReader(),
            // PostgreSQL
            "NpgsqlFactory" => new PostGreSqlSchemaReader(),
            // Oracle
            "OracleClientFactory" => new OracleSchemaReader(),
            _ => new SqlServerSchemaReader()
        };

        //reader.outer = this;
        var result = reader.ReadSchema(conn, factory);

        // Remove unrequired tables/views
        for (var i = result.Count - 1; i >= 0; i--)
        {
            /*TODO: Add again
                if (SchemaName != null && string.Compare(result[i].Schema, SchemaName, true) != 0)
                {
                    result.RemoveAt(i);
                    continue;
                }
                if (!IncludeViews && result[i].IsView)
                {
                    result.RemoveAt(i);
                    continue;
                }
                 * */
        }

        conn.Close();

        var rxClean =
            new Regex(
                "^(Equals|GetHashCode|GetType|ToString|repo|Save|IsNew|Insert|Update|Delete|Exists|SingleOrDefault|Single|First|FirstOrDefault|Fetch|Page|Query)$");
        foreach (var t in result)
        {
            t.ClassName = ClassPrefix + t.ClassName + ClassSuffix;
            foreach (var c in t.Columns)
            {
                c.PropertyName = rxClean.Replace(c.PropertyName, "_$1");

                // Make sure property name doesn't clash with class name
                if (c.PropertyName == t.ClassName)
                {
                    c.PropertyName = "_" + c.PropertyName;
                }
            }
        }

        return result;
    }
}

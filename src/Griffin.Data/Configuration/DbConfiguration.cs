using System;
using System.Data;
using System.Data.SqlClient;
using Griffin.Data.Dialects;
using Griffin.Data.Mappings;

namespace Griffin.Data;

/// <summary>
///     Configuration for this ORM library.
/// </summary>
public class DbConfiguration
{
    /// <summary>
    /// </summary>
    /// <param name="connectionString">Standard ADO.NET Connection string used to connect to the database.</param>
    public DbConfiguration(string connectionString)
    {
        ConnectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    /// <summary>
    ///     Standard ADO.NET Connection string used to connect to the database.
    /// </summary>
    public string ConnectionString { get; }

    /// <summary>
    ///     Dialect used to apply DB engine specific SQL variants to generated statements.
    /// </summary>
    public ISqlDialect Dialect { get; set; } = new SqlServerDialect();

    /// <summary>
    ///     Registry used to load mappings.
    /// </summary>
    public IMappingRegistry MappingRegistry { get; set; } = new MappingRegistry();

    /// <summary>
    ///     Create a new active transaction.
    /// </summary>
    /// <returns>Active transaction.</returns>
    public IDbTransaction BeginTransaction()
    {
        var con = new SqlConnection(ConnectionString);
        con.Open();
        return con.BeginTransaction();
    }
}

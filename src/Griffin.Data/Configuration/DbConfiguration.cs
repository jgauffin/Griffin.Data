using System;
using System.Data;
using Griffin.Data.ChangeTracking;
using Griffin.Data.Dialects;
using Griffin.Data.Mappings;

namespace Griffin.Data.Configuration;

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
    ///     Change tracker to use (optional).
    /// </summary>
    public Func<IChangeTracker>? ChangeTrackerFactory { get; set; }

    /// <summary>
    ///     Standard ADO.NET Connection string used to connect to the database.
    /// </summary>
    public string ConnectionString { get; }

    /// <summary>
    ///     Dialect used to apply DB engine specific SQL variants to generated statements.
    /// </summary>
    public ISqlDialect? Dialect { get; set; }

    /// <summary>
    ///     Registry used to load mappings.
    /// </summary>
    public IMappingRegistry MappingRegistry { get; set; } = new MappingRegistry();

    /// <summary>
    ///     Pluralize table names per default.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Will take class names and pluralize them when loading class mappings which have not specified a table name.
    ///     </para>
    /// </remarks>
    public bool PluralizeTableNames
    {
        get
        {
            if (MappingRegistry is MappingRegistry p)
            {
                return p.PluralizeTableNames;
            }

            return true;
        }
        set
        {
            if (MappingRegistry is MappingRegistry p)
            {
                p.PluralizeTableNames = value;
            }
        }
    }

    /// <summary>
    ///     Create a new active transaction.
    /// </summary>
    /// <returns>Active transaction.</returns>
    public IDbConnection OpenConnection()
    {
        if (Dialect == null)
        {
            throw new InvalidOperationException("DbConfiguration do not have a Dialect specified.");
        }

        var connection = Dialect.CreateConnection();
        connection.ConnectionString = ConnectionString;
        connection.Open();
        return connection;
    }
}

using Griffin.Data.Configuration;

namespace Griffin.Data.SqlServer;

/// <summary>
///     Extensions to make configuration of SQL server integration fluent.
/// </summary>
public static class ConfigExtensions
{
    /// <summary>
    ///     Use SQL Server dialect.
    /// </summary>
    /// <param name="configuration">config</param>
    /// <returns>config</returns>
    public static DbConfiguration UseSqlServer(this DbConfiguration configuration)
    {
        configuration.Dialect = new SqlServerDialect();
        return configuration;
    }
}

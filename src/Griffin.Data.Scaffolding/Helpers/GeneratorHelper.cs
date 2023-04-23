using System.Data;
using Griffin.Data.Dialects;
using Griffin.Data.Scaffolding.Mapper;
using Griffin.Data.Scaffolding.Queries;

namespace Griffin.Data.Scaffolding.Helpers;

internal class GeneratorHelper
{
    public static async Task GenerateOrm(string? dbEngineName, string connectionString, string directory)
    {
        var dialect = GetDialect(dbEngineName, directory);
        var scaffolder = new MapperScaffolder();
        await scaffolder.Generate(dialect, connectionString, directory);
    }

    public static async Task GenerateQueries(string? dbEngineName, string connectionString, string directory)
    {
        using var connection = OpenConnection(dbEngineName, connectionString, directory);
        var scaffolder = new QueryScaffolder();
        await scaffolder.Generate(connection, directory);
    }

    private static ISqlDialect GetDialect(string? dbEngine, string directory)
    {
        var dialect = string.IsNullOrEmpty(dbEngine)
            ? ConfigurationUtils.FindSqlDialectUsingConfig(directory)
            : ConfigurationUtils.FindSqlDialect(dbEngine);

        return dialect;
    }

    private static IDbConnection OpenConnection(string? dbEngine, string connectionString, string directory)
    {
        var dialect = string.IsNullOrEmpty(dbEngine)
            ? ConfigurationUtils.FindSqlDialectUsingConfig(directory)
            : ConfigurationUtils.FindSqlDialect(dbEngine);

        using var connection = dialect.CreateConnection();
        connection.ConnectionString = connectionString;
        connection.Open();

        return connection;
    }
}

using System.Data;
using Griffin.Data.Scaffolding.Mapper;
using Griffin.Data.Scaffolding.Queries;

namespace Griffin.Data.Scaffolding.Helpers;

internal class GeneratorHelper
{
    public static async Task GenerateOrm(string dbEngineName, string connectionString, string directory)
    {
        var scaffolder = new MapperScaffolder();
        await scaffolder.Generate(dbEngineName, connectionString, directory);
    }

    public static async Task GenerateQueries(IDbConnection connection, string directory)
    {
        var scaffolder = new QueryScaffolder();
        await scaffolder.Generate(connection, directory);
    }
}

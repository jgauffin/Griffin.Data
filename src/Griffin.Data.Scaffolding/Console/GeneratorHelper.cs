using System.Data;
using Griffin.Data.Scaffolding.Mapper;
using Griffin.Data.Scaffolding.Queries;

namespace Griffin.Data.Scaffolding.Console
{
    internal class GeneratorHelper
    {
        public static async Task GenerateQueries(IDbConnection connection, string directory)
        {
            var scaffolder = new QueryScaffolder();
            await scaffolder.Generate(connection, directory);
        }

        public static async Task GenerateOrm(IDbConnection connection, string directory)
        {
            var scaffolder = new MapperScaffolder();
            await scaffolder.Generate(connection, directory);
        }
    }
}

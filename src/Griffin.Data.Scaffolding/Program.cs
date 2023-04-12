using System.Data.SqlClient;
using Griffin.Data.Scaffolding.Console;
using Microsoft.Extensions.Configuration;

if (args.Length == 0)
{
    HelpInfo.Display();
    return;
}

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

var connection = new SqlConnection(config.GetConnectionString("Db"));
connection.Open();

switch (args[0])
{
    case "generate":
    {
        if (args.Length == 1)
        {
            await GeneratorHelper.GenerateOrm(connection, Environment.CurrentDirectory);
            await GeneratorHelper.GenerateQueries(connection, Environment.CurrentDirectory);
        }
        else switch (args[1])
        {
            case "queries":
                await GeneratorHelper.GenerateQueries(connection, Environment.CurrentDirectory);
                break;
            case "mappings":
                await GeneratorHelper.GenerateOrm(connection, Environment.CurrentDirectory);
                break;
            default:
                Console.WriteLine("Unknown command");
                break;
        }

        return;
    }
    case "config":
        return;
    default:
        HelpInfo.Display();
        break;
}

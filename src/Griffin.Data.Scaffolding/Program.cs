using System.Data.SqlClient;
using Griffin.Data.Scaffolding.Helpers;
using Microsoft.Extensions.Configuration;

if (args.Length == 0)
{
    HelpInfo.Display();
    return;
}

var config = ConfigHelper.LoadConfig();
if (config == null)
{
    return;
}

var conString = config.GetConnectionString("Db");
if (conString == null)
{
    Console.WriteLine("The must be a connection string named 'Db' in 'appsettings.json'.");
    return;
}

var connection = new SqlConnection(conString);
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

    case "queries":
        await GeneratorHelper.GenerateQueries(connection, Environment.CurrentDirectory);
        break;

    case "mappings":
        await GeneratorHelper.GenerateOrm(connection, Environment.CurrentDirectory);
        break;

    case "config":
        return;

    default:
        HelpInfo.Display();
        break;
}

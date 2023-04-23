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

switch (args[0])
{
    case "generate":
    {
        if (args.Length == 1)
        {
            await GeneratorHelper.GenerateOrm("mssql", conString, Environment.CurrentDirectory);
            await GeneratorHelper.GenerateQueries("mssql", conString, Environment.CurrentDirectory);
        }
        else
        {
            switch (args[1])
            {
                case "queries":
                    await GeneratorHelper.GenerateQueries("mssql", conString, Environment.CurrentDirectory);
                    break;
                case "mappings":
                    await GeneratorHelper.GenerateOrm("SqlServer", conString, Environment.CurrentDirectory);
                    break;
                default:
                    Console.WriteLine("Unknown command: " + args[1]);
                    break;
            }
        }

        return;
    }

    case "queries":
        await GeneratorHelper.GenerateQueries("mssql", conString, Environment.CurrentDirectory);
        break;

    case "mappings":
        await GeneratorHelper.GenerateOrm("SqlServer", conString, Environment.CurrentDirectory);
        break;

    case "config":
        return;

    default:
        HelpInfo.Display();
        break;
}

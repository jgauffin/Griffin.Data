using System.Reflection;
using System.Text.Json;
using Griffin.Data.Dialects;
using Griffin.Data.Scaffolding.Meta;

namespace Griffin.Data.Scaffolding;

internal class ConfigurationUtils
{
    public static ISqlDialect FindSqlDialect(string engineName)
    {
        var schemaTypes = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
            let types = assembly.GetTypes()
            from type in types
            where IsSqlDialect(type)
            let attribute = type.GetCustomAttribute<DbEngineNameAttribute>()
            where attribute?.DatabaseEngineName.Equals(engineName, StringComparison.OrdinalIgnoreCase) == true
            select type).ToList();

        if (schemaTypes.Count == 1)
        {
            return (ISqlDialect)Activator.CreateInstance(schemaTypes[0])!;
        }

        if (schemaTypes.Count > 1)
        {
            throw CreateMultipleEnginesException();
        }

        throw CreateNotFoundException();
    }

    public static ISqlDialect FindSqlDialectUsingConfig(string directory)
    {
        var schemaTypes = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
            let types = assembly.GetTypes()
            from type in types
            where IsSqlDialect(type)
            select type).ToList();

        if (schemaTypes.Count == 1)
        {
            return (ISqlDialect)Activator.CreateInstance(schemaTypes[0])!;
        }

        var config = LoadConfig(directory);
        if (config?.DbEngine == null)
        {
            if (schemaTypes.Count > 1)
            {
                throw CreateMultipleEnginesException();
            }

            throw CreateNotFoundException();
        }

        var chosen = schemaTypes.FirstOrDefault(x =>
            x.GetCustomAttribute<DbEngineNameAttribute>()?.DatabaseEngineName
                .Equals(config.DbEngine, StringComparison.OrdinalIgnoreCase) == true);
        if (chosen == null)
        {
            throw new InvalidOperationException(
                $"An 'griffin.data.json' was found, but the selected dialect '{config.DbEngine}' was not found. Have you installed the correct nuget package?");
        }

        return (ISqlDialect)Activator.CreateInstance(chosen)!;
    }

    public static ScaffoldingConfig? LoadConfig(string currentDirectory)
    {
        var fileName = Path.Combine(currentDirectory, "griffin.data.json");
        if (!File.Exists(fileName))
        {
            return null;
        }

        var settings = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        var json = File.ReadAllText(fileName);
        var jsonObj = JsonSerializer.Deserialize<ScaffoldingConfig>(json, settings);
        return jsonObj;
    }

    private static InvalidOperationException CreateMultipleEnginesException()
    {
        return new InvalidOperationException(
            @"Found multiple DB engine libraries for Griffin.Data. Either uninstall one, or create 'griffin.data.json' in your root directory and add '{ dbEngine: ""Griffin.Data.SqlServer"" }}', or your chosen DB provider (i.e. the same name as the nuget package).");
    }

    private static InvalidOperationException CreateNotFoundException()
    {
        return new InvalidOperationException(
            @"Failed to find a DB engine library for Griffin.Data, or it wasn't loaded. If you have installed it, create 'griffin.data.json' in your root directory and add '{ dbEngine: ""Griffin.Data.SqlServer"" }', or your chosen DB provider (i.e. the same name as the nuget package).");
    }

    private static bool IsSqlDialect(Type type)
    {
        return typeof(ISqlDialect).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract;
    }
}

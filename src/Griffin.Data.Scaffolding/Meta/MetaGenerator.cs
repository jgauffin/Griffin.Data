using System.Reflection;
using System.Text.Json;
using Griffin.Data.Scaffolding.Config;
using Griffin.Data.Scaffolding.Mapper;
using Griffin.Data.Scaffolding.Meta.Analyzers;

namespace Griffin.Data.Scaffolding.Meta;

internal class MetaGenerator
{
    public MetaGenerator()
    {
    }

    public async Task<IReadOnlyList<Table>> ReadSchema(string engineName, string connectionString)
    {
        var reader = FindReader(engineName);
        var tables = new List<Table>();
        var context = new SchemaReaderContext(connectionString, tables);
        await reader.ReadSchema(context);

        return tables;
    }

    private string? FindConfigProvider(string currentDirectory)
    {
        var fileName = Path.Combine(currentDirectory, "griffin.data.json");
        if (!File.Exists(fileName))
        {
            return null;
        }

        var settings = new JsonSerializerOptions() { PropertyNameCaseInsensitive = true };

        var json = File.ReadAllText(fileName);
        var jsonObj = JsonSerializer.Deserialize<ScaffoldingConfig>(json, settings);
        return jsonObj?.DbEngine;
    }

    private ISchemaReader FindReader(string engineName)
    {
        var schemaTypes = (from assembly in AppDomain.CurrentDomain.GetAssemblies()
            where assembly.GetName().Name?.StartsWith("Griffin.Data") == true
            let types = assembly.GetTypes()
            from type in types
            where IsSchemaReader(type)
            select type).ToList();

        if (schemaTypes.Count == 1)
        {
            return (ISchemaReader)Activator.CreateInstance(schemaTypes[0])!;
        }

        var chosenProvider = FindConfigProvider(Environment.CurrentDirectory);
        if (chosenProvider != null)
        {
            var assembly = Assembly.Load(chosenProvider);
            var readerTypes = assembly.GetTypes().Where(IsSchemaReader).ToList();
            foreach (var readerType in readerTypes)
            {
                var attr = readerType.GetCustomAttribute<SchemaReaderAttribute>();
                if (attr != null && attr.DatabaseEngineName.Equals(engineName, StringComparison.OrdinalIgnoreCase))
                {
                    return (ISchemaReader)Activator.CreateInstance(readerType)!;
                }
            }
        }

        if (schemaTypes.Count > 1)
        {
            throw new InvalidOperationException(
                @"Found multiple DB engine libraries for Griffin.Data. Either uninstall one, or create 'griffin.data.json' in your root directory and add '{ dbEngine: ""Griffin.Data.SqlServer"" }}', or your chosen DB provider (i.e. the same name as the nuget package).");
        }

        throw new InvalidOperationException(
            @"Failed to find a DB engine library for Griffin.Data, or it wasn't loaded. If you have installed it, create 'griffin.data.json' in your root directory and add '{ dbEngine: ""Griffin.Data.SqlServer"" }', or your chosen DB provider (i.e. the same name as the nuget package).");
    }

    private static bool IsSchemaReader(Type type)
    {
        return typeof(ISchemaReader).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract;
    }
}

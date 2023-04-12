using System.Data;
using Griffin.Data.Scaffolding.Mapper.Generators;
using Griffin.Data.Scaffolding.Meta;

namespace Griffin.Data.Scaffolding.Mapper;

/// <summary>
///     Builds classes for the object/relationship mapper.
/// </summary>
public class MapperScaffolder
{
    /// <summary>
    /// </summary>
    /// <param name="connection"></param>
    /// <param name="directory"></param>
    /// <returns></returns>
    public async Task Generate(IDbConnection connection, string directory)
    {
        var reader = new TableSchemaReader();
        var tables = await reader.Generate(connection);

        var g1 = new ClassGenerator();
        var g2 = new MappingGenerator();
        foreach (var table in tables)
        {
            var folder = table.Namespace.Replace(".", "\\");
            var subDir = Path.Combine(directory, folder);
            if (!Directory.Exists(subDir))
            {
                Directory.CreateDirectory(subDir);
            }

            var text = g1.Generate(table, tables);
            await File.WriteAllTextAsync(Path.Combine(subDir, table.ClassName + ".cs"), text);
            var text2 = g2.Generate(table);
            await File.WriteAllTextAsync(Path.Combine(subDir, table.ClassName + "Mapping.cs"), text2);
        }
    }
}

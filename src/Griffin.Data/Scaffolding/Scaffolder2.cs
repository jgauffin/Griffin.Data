using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using Griffin.Data.Scaffolding.Generators;

namespace Griffin.Data.Scaffolding;

public class Scaffolder2
{
    public async Task Do(string connectionString, string directory)
    {
        await using var connection = new SqlConnection(connectionString);
        connection.Open();

        var reader = new TableReader();
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

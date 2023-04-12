using System.Data;
using Griffin.Data.Scaffolding.Queries.Generators;
using Griffin.Data.Scaffolding.Queries.Meta;
using Griffin.Data.Scaffolding.Queries.Parser;

namespace Griffin.Data.Scaffolding.Queries;

public class QueryScaffolder
{
    private static readonly IQueryGenerator[] Generators =
    {
        new QueryClassGenerator(), new QueryResultGenerator(), new QueryResultItemGenerator(),
        new QueryRunnerGenerator()
    };

    /// <summary>
    /// </summary>
    /// <param name="connectionString"></param>
    /// <param name="directory">Directory recursively scan for query files.</param>
    /// <returns></returns>
    public async Task Generate(IDbConnection connection, string directory)
    {
        var files = new List<QueryFile>();
        await ParseQueryFiles(directory, files);
        var metas = GenerateQueryMetadata(files, connection);
        await GenerateClasses(metas);
    }

    private static async Task GenerateClasses(List<QueryMeta> metas)
    {
        foreach (var meta in metas)
        {
            foreach (var generator in Generators)
            {
                var file = await generator.Generate(meta);
                var fullPath = Path.Combine(meta.Directory, file.ClassName + ".cs");
                await File.WriteAllTextAsync(fullPath, file.Contents);
            }
        }
    }

    private static List<QueryMeta> GenerateQueryMetadata(List<QueryFile> queryFiles, IDbConnection connection)
    {
        var metas = new List<QueryMeta>();
        foreach (var queryFile in queryFiles)
        {
            var parser = new MetaProvider();
            var meta = parser.GenerateMeta(queryFile, connection);
            metas.Add(meta);
        }

        return metas;
    }

    private static async Task ParseQueryFiles(string directory, ICollection<QueryFile> foundFiles)
    {
        var files = Directory.EnumerateFiles(directory)
            .Where(x => x.EndsWith(".query.sql", StringComparison.OrdinalIgnoreCase))
            .ToList();
        foreach (var queryFileName in files)
        {
            var sql = await File.ReadAllTextAsync(queryFileName);
            var parser = new QueryParser();
            var queryFile = parser.ParseFile(queryFileName, sql);
            foundFiles.Add(queryFile);
        }

        var subDirs = Directory.GetDirectories(directory);
        foreach (var subDir in subDirs)
        {
            if (subDir.StartsWith("."))
            {
                continue;
            }

            await ParseQueryFiles(subDir, foundFiles);
        }
    }
}

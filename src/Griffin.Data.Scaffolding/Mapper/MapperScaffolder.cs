﻿using System.Reflection;
using Griffin.Data.Dialects;
using Griffin.Data.Scaffolding.Config;
using Griffin.Data.Scaffolding.Mapper.Generators;
using Griffin.Data.Scaffolding.Meta;

namespace Griffin.Data.Scaffolding.Mapper;

/// <summary>
///     Builds classes for the object/relationship mapper.
/// </summary>
public class MapperScaffolder
{
    private readonly List<IMetaAnalyzer> _analyzers = new();
    private readonly List<IClassGenerator> _generators = new();

    public MapperScaffolder()
    {
        FindGenerators();
        FindAnalyzers();
    }

    public bool OverwriteFiles { get; set; }

    /// <summary>
    /// </summary>
    /// <param name="connectionString">Connection string.</param>
    /// <param name="directory">Solution directory.</param>
    /// <param name="dialect">Used to create the correct SchemaReader.</param>
    /// <returns></returns>
    public async Task Generate(ISqlDialect dialect, string connectionString, string directory)
    {
        var namespaceFinder = new ProjectFolderGuesser();
        var folders = namespaceFinder.GetFolders(directory);

        var meta = new MetaGenerator();
        var tables = await meta.ReadSchema(dialect, connectionString);

        var context = new GeneratorContext(tables, folders);
        foreach (var analyzer in _analyzers.OrderBy(x => x.Priority))
        {
            analyzer.Analyze(context);
        }

        foreach (var table in tables)
        {
            foreach (var generator in _generators)
            {
                generator.Generate(table, context);
            }
        }

        foreach (var file in context.GeneratedFiles)
        {
            var subDir = Path.Combine(directory, file.RelativeDirectory);
            if (!Directory.Exists(subDir))
            {
                Directory.CreateDirectory(subDir);
            }

            var fullPath = Path.Combine(subDir, file.ClassName + ".cs");
            if (File.Exists(fullPath))
            {
                if (!OverwriteFiles)
                {
                    continue;

                }

                // Don't overwrite if file is frozen.
                var contents = await File.ReadAllTextAsync(fullPath);
                if (contents.Contains("[Freeze]"))
                {
                    continue;
                }
            }

            await File.WriteAllTextAsync(fullPath, file.Contents);
        }
    }

    private void FindAnalyzers()
    {
        var analyzers = from type in Assembly.GetExecutingAssembly().GetTypes()
            where typeof(IMetaAnalyzer).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract
            select (IMetaAnalyzer)Activator.CreateInstance(type)!;

        _analyzers.AddRange(analyzers);
    }

    private void FindGenerators()
    {
        var generators = from type in Assembly.GetExecutingAssembly().GetTypes()
            where typeof(IClassGenerator).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract
            select (IClassGenerator)Activator.CreateInstance(type)!;

        _generators.AddRange(generators);
    }

    private string? FindRootNamespace(string directory, string lastParts = "")
    {
        var files = Directory.GetFiles(directory, "*.csproj");
        if (!files.Any())
        {
            var parent = Directory.GetParent(directory);
            if (parent == null)
            {
                return null;
            }

            return lastParts == ""
                ? FindRootNamespace(parent.FullName, new DirectoryInfo(directory).Name)
                : FindRootNamespace(parent.FullName, $"{lastParts}.{new DirectoryInfo(directory).Name}");
        }

        var name = Path.GetFileNameWithoutExtension($"{files[0]}.{lastParts}");
        return name;
    }
}

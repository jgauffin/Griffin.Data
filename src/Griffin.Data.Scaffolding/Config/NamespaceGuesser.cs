namespace Griffin.Data.Scaffolding.Config;

public class NamespaceGuesser
{
    private readonly List<Func<string, bool>> _coreParts = new()
    {
        x => x.Contains("Application"),
        x => x.EndsWith("App"),
        x => x.Contains("Business"),
        x => x.Contains("Core"),
        x => x.Contains("Domain")
    };

    private readonly List<Func<string, bool>> _dataParts = new()
    {
        x => x.EndsWith("Data"), x => x.Contains("Repositories")
    };

    private readonly List<Func<string, bool>> _testParts;

    public NamespaceGuesser()
    {
        _testParts = new List<Func<string, bool>>
        {
            x => x.Contains("Tests") && _dataParts.Any(y => y(x)),
            x => x.EndsWith("Test") && _dataParts.Any(y => y(x))
        };
    }

    public void Analyze()
    {
        var solutionDirectory = FindSolutionDirectory();

        var lastPartOfDirs =
            Directory.GetDirectories(solutionDirectory).Select(x => new DirectoryInfo(x).Name).ToList();
        var dataProjectFolder = lastPartOfDirs.FirstOrDefault(dir => _dataParts.Any(filter => filter(dir)));
        var entityFolder = lastPartOfDirs.FirstOrDefault(dir => _coreParts.Any(filter => filter(dir)));
        var testFolder = lastPartOfDirs.FirstOrDefault(dir => _testParts.Any(filter => filter(dir)));

        var config = new ScaffoldingConfiguration();
        config.TargetLocations = new TargetLocations();

        if (dataProjectFolder != null)
        {
            var projectFile = Directory.GetFiles(dataProjectFolder, "*.csproj").FirstOrDefault();
            var ns = projectFile == null
                ? new DirectoryInfo(dataProjectFolder).Name
                : Path.GetFileNameWithoutExtension(projectFile);
            config.TargetLocations.Mappings = new TargetLocation
            {
                NamespaceTemplate = $"{ns}.[PluralEntityName]",
                ProjectDirectory = dataProjectFolder.Remove(0, solutionDirectory.Length).TrimStart('\\'),
                ProjectName = ns
            };

            config.TargetLocations.Queries = new TargetLocation
            {
                NamespaceTemplate = $"{ns}.Queries",
                ProjectDirectory = dataProjectFolder.Remove(0, solutionDirectory.Length).TrimStart('\\'),
                ProjectName = ns
            };

            config.TargetLocations.RepositoryClasses = new TargetLocation
            {
                NamespaceTemplate = $"{ns}.[PluralEntityName]",
                ProjectDirectory = dataProjectFolder.Remove(0, solutionDirectory.Length).TrimStart('\\'),
                ProjectName = ns
            };
        }

        if (entityFolder != null)
        {
        }
    }

    private static string FindSolutionDirectory()
    {
        var solutionDirectory = Environment.CurrentDirectory;
        while (solutionDirectory.Length > 2)
        {
            var solution = Directory.GetFiles(solutionDirectory, "*.sln").FirstOrDefault();
            if (solution != null)
            {
                break;
            }

            solutionDirectory = Path.Combine(Environment.CurrentDirectory, "..");
        }

        return solutionDirectory;
    }
}

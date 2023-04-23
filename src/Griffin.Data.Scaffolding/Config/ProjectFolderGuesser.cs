namespace Griffin.Data.Scaffolding.Config;

public class ProjectFolderGuesser
{
    private readonly List<Func<string, bool>> _coreParts = new()
    {
        x => x.Contains("Application", StringComparison.OrdinalIgnoreCase),
        x => x.EndsWith("App"),
        x => x.Contains("Business", StringComparison.OrdinalIgnoreCase),
        x => x.Contains("Core", StringComparison.OrdinalIgnoreCase),
        x => x.Contains("Domain", StringComparison.OrdinalIgnoreCase)
    };

    private readonly List<Func<string, bool>> _coreTestParts = new()
    {
        x => x.Contains("Application", StringComparison.OrdinalIgnoreCase) &&
             x.Contains("Test", StringComparison.OrdinalIgnoreCase),
        x => x.Contains("App") && x.Contains("Test", StringComparison.OrdinalIgnoreCase),
        x => x.Contains("Business", StringComparison.OrdinalIgnoreCase) &&
             x.Contains("Test", StringComparison.OrdinalIgnoreCase),
        x => x.Contains("Core", StringComparison.OrdinalIgnoreCase) &&
             x.Contains("Test", StringComparison.OrdinalIgnoreCase),
        x => x.Contains("Domain", StringComparison.OrdinalIgnoreCase) &&
             x.Contains("Test", StringComparison.OrdinalIgnoreCase)
    };

    private readonly List<Func<string, bool>> _dataParts = new()
    {
        x => x.EndsWith("Data", StringComparison.OrdinalIgnoreCase),
        x => x.Contains("Repositories", StringComparison.OrdinalIgnoreCase),
        x => x.Contains("Datalayer", StringComparison.OrdinalIgnoreCase),
        x => x.Contains("Db")
    };

    private readonly List<Func<string, bool>> _dataTestsParts = new()
    {
        x => x.Contains("Data", StringComparison.OrdinalIgnoreCase) &&
             x.Contains("Test", StringComparison.OrdinalIgnoreCase),
        x => x.Contains("Repositories", StringComparison.OrdinalIgnoreCase) &&
             x.Contains("Test", StringComparison.OrdinalIgnoreCase),
        x => x.Contains("Datalayer", StringComparison.OrdinalIgnoreCase) &&
             x.Contains("Test", StringComparison.OrdinalIgnoreCase),
        x => x.Contains("Db") && x.Contains("Test", StringComparison.OrdinalIgnoreCase)
    };

    public ProjectFolders GetFolders(string rootDirectory)
    {
        var solutionDirectory = FindFileUp(rootDirectory, "*.sln");
        if (solutionDirectory == null)
        {
            throw new InvalidOperationException("Failed to find solution file in path: " + rootDirectory);
        }

        var firstProjectDirectory = FindFirstProject(rootDirectory);
        if (firstProjectDirectory == null)
        {
            throw new InvalidOperationException("Did not find a project in any of the sub folders of: " +
                                                rootDirectory);
        }

        var lastPartOfDirs =
            Directory.GetDirectories(solutionDirectory).Select(x => new DirectoryInfo(x).Name).ToList();

        var dataProjectFolder = lastPartOfDirs.FirstOrDefault(dir => _dataParts.Any(filter => filter(dir)));
        var entityFolder = lastPartOfDirs.FirstOrDefault(dir => _coreParts.Any(filter => filter(dir)));
        var entityTestFolder = lastPartOfDirs.FirstOrDefault(dir => _coreTestParts.Any(filter => filter(dir)));
        var dataTestFolder = lastPartOfDirs.FirstOrDefault(dir => _dataTestsParts.Any(filter => filter(dir)));

        var config = new ScaffoldingConfiguration { TargetLocations = new TargetLocations() };

        var folders = new ProjectFolders();
        if (dataProjectFolder != null)
        {
            folders.DataFolder = dataProjectFolder;
            folders.DataNamespace = GetNamespaceFromProjectFileName(Path.Combine(rootDirectory, dataProjectFolder));
        }
        else
        {
            folders.DataFolder = Path.Combine(firstProjectDirectory, "Data");
            folders.DataNamespace = GetNamespaceFromProjectFileName(firstProjectDirectory) + ".Data";
        }

        if (dataTestFolder != null)
        {
            folders.DataTestFolder = dataTestFolder;
            folders.DataTestNamespace = GetNamespaceFromProjectFileName(Path.Combine(rootDirectory, dataTestFolder));
        }
        else
        {
            folders.DataTestFolder = Path.Combine(firstProjectDirectory, "Data\\Tests");
            folders.DataTestNamespace = GetNamespaceFromProjectFileName(firstProjectDirectory) + ".Data.Tests";
        }

        if (entityFolder != null)
        {
            folders.DomainFolder = entityFolder;
            folders.DomainNamespace = GetNamespaceFromProjectFileName(Path.Combine(rootDirectory, entityFolder));
        }
        else
        {
            folders.DomainFolder = Path.Combine(firstProjectDirectory, "Domain");
            folders.DomainNamespace = GetNamespaceFromProjectFileName(firstProjectDirectory) + ".Domain";
        }

        if (entityTestFolder != null)
        {
            folders.DomainTestFolder = entityTestFolder;
            folders.DomainTestNamespace =
                GetNamespaceFromProjectFileName(Path.Combine(rootDirectory, entityTestFolder));
        }
        else
        {
            folders.DomainTestFolder = Path.Combine(firstProjectDirectory, "Domain.Tests");
            folders.DomainTestNamespace = GetNamespaceFromProjectFileName(firstProjectDirectory) + ".Domain.Tests";
        }

        return folders;
    }

    private static string? FindFileDown(string rootDirectory, string searchPattern)
    {
        var files = Directory.GetFiles(rootDirectory, "*.csproj");
        if (files.Any())
        {
            return rootDirectory;
        }

        var dirs = Directory.GetDirectories(rootDirectory);
        foreach (var dir in dirs)
        {
            var foundDir = FindFileDown(dir, searchPattern);
            if (foundDir != null)
            {
                return foundDir;
            }
        }

        return null;
    }

    private static string? FindFileUp(string rootDirectory, string fileType)
    {
        var currentDirectory = rootDirectory.TrimEnd(Path.DirectorySeparatorChar);
        while (currentDirectory.Length > 3)
        {
            var solution = Directory.GetFiles(currentDirectory, fileType).FirstOrDefault();
            if (solution != null)
            {
                return currentDirectory;
            }

            var newDir = Path.GetFullPath("..", currentDirectory);

            // no more parent folder
            if (newDir == currentDirectory)
            {
                break;
            }

            currentDirectory = newDir;
        }

        return null;
    }

    private static string? FindFirstProject(string directory)
    {
        var files = Directory.GetFiles(directory, "*.csproj");
        if (files.Any())
        {
            return directory;
        }

        return FindFileDown(directory, "*.csproj");
    }

    private static string GetNamespaceFromProjectFileName(string projectFolder)
    {
        var projectFile = Directory.GetFiles(projectFolder, "*.csproj").FirstOrDefault();
        var ns = projectFile == null
            ? new DirectoryInfo(projectFolder).Name
            : Path.GetFileNameWithoutExtension(projectFile);
        return ns;
    }
}

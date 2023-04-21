namespace Griffin.Data.Scaffolding.Config;

internal static class StartupHelper
{
    public static void GenerateConfig()
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

        var subDirs = Directory.GetDirectories(solutionDirectory);

        var config = new ScaffoldingConfiguration();
        var dataFolder = subDirs.FirstOrDefault(x => new DirectoryInfo(x).Name.Contains("Data"));
        var entityFolder = subDirs.FirstOrDefault(x =>
        {
            var info = new DirectoryInfo(x).Name;
            return info.Contains("Business") || info.Contains("Core") || info.Contains("Application") ||
                   info.EndsWith("App") || info.Contains("Domain");
        });

        if (dataFolder != null)
        {
            //config.Namespaces.Mappings
        }
    }

    public static string GetConfigPath(IDictionary<string, string[]> arguments)
    {
        if (arguments.TryGetValue("config", out var values))
        {
            if (values.Any())
            {
                return values[0];
            }

            PrintError(
                $"-config must have a complete path to '{ScaffoldingConfiguration.Filename}'. The path may be relative to the working directory. That file do not need to exist (it will be generated if missing).");

            return values[0];
        }

        var configFile = Path.Combine(Environment.CurrentDirectory, ScaffoldingConfiguration.Filename);
        if (File.Exists(configFile))
        {
            return configFile;
        }

        return ScanDirectoryAfterConfig(Environment.CurrentDirectory) ?? configFile;
    }

    public static void PrintError(string s)
    {
    }

    public static string? ScanDirectoryAfterConfig(string directory)
    {
        var dirs = Directory.GetDirectories(directory);
        foreach (var dir in dirs)
        {
            var configFile = Path.Combine(dir, ScaffoldingConfiguration.Filename);
            if (File.Exists(configFile))
            {
                return configFile;
            }

            ScanDirectoryAfterConfig(dir);
        }

        return null;
    }
}

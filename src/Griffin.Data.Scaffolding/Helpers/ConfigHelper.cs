using Microsoft.Extensions.Configuration;

namespace Griffin.Data.Scaffolding.Helpers
{
    internal class ConfigHelper
    {
        public static IConfiguration? LoadConfig()
        {
            if (!TryFindConfigDirectory(Directory.GetCurrentDirectory(), out var directory))
            {
                Console.WriteLine("The must be a appsettings.json and the working directory or any if its sub directories.");
                return null;
            }

            var config = new ConfigurationBuilder()
                .SetBasePath(directory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            return config;
        }

        private static bool TryFindConfigDirectory(string directory, out string foundDirectory)
        {
            var filename = Path.Combine(directory, "appsettings.json");
            if (File.Exists(filename))
            {
                foundDirectory = directory;
                return true;
            }

            foreach (var dir in Directory.GetDirectories(directory))
            {
                if (dir.Contains("."))
                    continue;

                if (TryFindConfigDirectory(dir, out foundDirectory))
                {
                    return true;
                }
            }

            foundDirectory = "";
            return false;
        }
    }
}

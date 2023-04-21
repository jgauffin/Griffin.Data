using System.Reflection;

namespace Griffin.Data.Scaffolding.Helpers;

internal class HelpInfo
{
    public static void Display()
    {
        var versionString = Assembly.GetExecutingAssembly()?
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
            .InformationalVersion;

        Console.WriteLine($"Griffin.Data Scaffolding v{versionString}");
        Console.WriteLine("-------------");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine();
        Console.WriteLine("  gd config             Generate a config file.");
        Console.WriteLine();
        Console.WriteLine("  gd generate           Generate queries and entities.");
        Console.WriteLine("  gd generate mappings  Generate entities and mappings.");
        Console.WriteLine(
            "  gd generate repos     Generate repositories (requires that entities have been generated).");
        Console.WriteLine("  gd generate queries   Generate queries.");
    }
}

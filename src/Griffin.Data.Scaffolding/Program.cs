using System.Reflection;

if (args.Length == 0)
{
    var versionString = Assembly.GetExecutingAssembly()?
        .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
        .InformationalVersion
        .ToString();

    Console.WriteLine($"Griffin.Data Scaffolding v{versionString}");
    Console.WriteLine("-------------");
    Console.WriteLine();
    Console.WriteLine("Usage:");
    Console.WriteLine("  gf config          Generate a config file.");
    Console.WriteLine("  gf generate        Generate queries and entities.");
    Console.WriteLine("  gf generate repos  Generate repositories (requires that entities have been generated).");
    return;
}

if (args[0] == "config")
{

}


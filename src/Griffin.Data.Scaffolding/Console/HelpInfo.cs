using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Griffin.Data.Scaffolding.Console
{
    internal class HelpInfo
    {
        public static void Display()
        {
            var versionString = Assembly.GetExecutingAssembly()?
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion
                .ToString();

            System.Console.WriteLine($"Griffin.Data Scaffolding v{versionString}");
            System.Console.WriteLine("-------------");
            System.Console.WriteLine();
            System.Console.WriteLine("Usage:");
            System.Console.WriteLine();
            System.Console.WriteLine("  gd config             Generate a config file.");
            System.Console.WriteLine();
            System.Console.WriteLine("  gd generate           Generate queries and entities.");
            System.Console.WriteLine("  gd generate mappings  Generate entities and mappings.");
            System.Console.WriteLine("  gd generate repos     Generate repositories (requires that entities have been generated).");
            System.Console.WriteLine("  gd generate queries   Generate queries.");
        }
    }
}

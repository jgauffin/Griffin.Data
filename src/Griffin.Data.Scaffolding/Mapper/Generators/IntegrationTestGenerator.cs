using Griffin.Data.Helpers;

namespace Griffin.Data.Scaffolding.Mapper.Generators;

internal class IntegrationTestGenerator : IClassGenerator
{
    private bool _generated;

    public void Generate(Table table, GeneratorContext context)
    {
        if (_generated)
        {
            return;
        }

        _generated = true;

        var sb = new TabbedStringBuilder();
        sb.AppendLine(@"using System.Data.SqlClient;");
        sb.AppendLine(@"using Griffin.Data;");
        sb.AppendLine(@"using Griffin.Data.ChangeTracking;");
        sb.AppendLine(@"using Griffin.Data.Mappings;");
        sb.AppendLine(@"using Microsoft.Extensions.Configuration;");
        sb.AppendLine($"using {context.Folders.DataFolder}.{table.RelativeNamespace}.Mappings;");
        sb.AppendLine();
        sb.AppendLine($"namespace {context.Folders.DataTestNamespace}");
        sb.AppendLineIndent("{");

        sb.AppendLine(@"/// <summary>");
        sb.AppendLine(
            @"///     Install the nuget package ""Microsoft.Extensions.Configuration.Json"". Add a ""appsettings.json"", mark it as ""copy");
        sb.AppendLine(
            @"///     always"" and then add a connection string named ""TestDb"" to it. Or change the contents below ;)");
        sb.AppendLine(@"/// </summary>");
        sb.AppendLine(@"public class IntegrationTest : IDisposable");
        sb.AppendLineIndent(@"{");
        sb.AppendLine(@"protected IntegrationTest()");
        sb.AppendLineIndent(@"{");
        sb.AppendLineIndent(@"var config = new ConfigurationBuilder()");
        sb.AppendLine(@".SetBasePath(Directory.GetCurrentDirectory())");
        sb.AppendLine(@".AddJsonFile(""appsettings.json"", false)");
        sb.AppendLine(@".Build();");
        sb.DedentAppendLine();
        sb.AppendLine(@"var connectionString = config.GetConnectionString(""TestDb"");");
        sb.AppendLine(@"if (connectionString == null)");
        sb.AppendLine(
            @"    throw new InvalidOperationException(""Failed to find a connection string named 'TestDb' in appsettings.json"");");
        sb.AppendLine();
        sb.AppendLine(@"// Change to the correct ADO.NET Provider.");
        sb.AppendLine(@"var connection = new SqlConnection(connectionString);");
        sb.AppendLine(@"var dialect = new SqlServerDialect();");
        sb.AppendLine(@"connection.Open();");
        sb.AppendLine();
        sb.AppendLine(@"var registry = new MappingRegistry();");
        sb.AppendLine($"registry.Scan(typeof({table.ClassName}Mapping).Assembly);");
        sb.AppendLine();
        sb.AppendLine(@"var changeTracking = new SnapshotChangeTracking(registry);");
        sb.AppendLine();
        sb.AppendLine(@"Session = new Session(connection.BeginTransaction(), registry, dialect, changeTracking);");
        sb.DedentAppendLine(@"}");
        sb.AppendLine();
        sb.AppendLine(@"protected Session Session { get; }");
        sb.AppendLine();

        DisposeMethods(sb);

        sb.DedentAppendLine("}");
        var file = new GeneratedFile("IntegrationTest", FileType.Data, sb.ToString())
        {
            RelativeDirectory = context.Folders.DataTestFolder
        };
        context.Add(file);
    }

    private static void DisposeMethods(TabbedStringBuilder sb)
    {
        sb.AppendLine(@"public void Dispose()");
        sb.AppendLineIndent(@"{");
        sb.AppendLine(@"Dispose(true);");
        sb.AppendLine(@"GC.SuppressFinalize(this);");
        sb.DedentAppendLine(@"}");

        sb.AppendLine();

        sb.AppendLine(@"protected virtual void Dispose(bool isDisposing)");
        sb.AppendLineIndent(@"{");
        sb.AppendLine(@"Session.Dispose();");
        sb.DedentAppendLine(@"}");
        sb.DedentAppendLine("}");
    }
}

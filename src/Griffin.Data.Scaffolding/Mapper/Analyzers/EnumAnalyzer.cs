using Griffin.Data.Helpers;

namespace Griffin.Data.Scaffolding.Mapper.Analyzers;

public class EnumAnalyzer : IMetaAnalyzer
{
    public int Priority => 10;

    public void Analyze(GeneratorContext context)
    {
        foreach (var table in context.Tables)
        {
            AnalyzeTable(table, context);
        }
    }

    public void AnalyzeTable(Table table, GeneratorContext context)
    {
        if (table == null)
        {
            throw new ArgumentNullException(nameof(table));
        }

        foreach (var column in table.Columns)
        {
            string enumName;
            if (column.Name.Equals("State", StringComparison.OrdinalIgnoreCase))
            {
                enumName = $"{table.ClassName}State";
            }
            else if (column.Name.StartsWith("State", StringComparison.OrdinalIgnoreCase))
            {
                enumName = $"{column.Name[5..]}State";
            }
            else if (column.Name.EndsWith("State", StringComparison.OrdinalIgnoreCase))
            {
                enumName = column.Name;
            }
            else
            {
                continue;
            }

            column.CustomPropertyType = enumName;

            if (context.GeneratedFiles.Any(x => x.ClassName.Equals(enumName)))
            {
                continue;
            }

            var file = GenerateEnum($"{context.Folders.DomainNamespace}.{table.RelativeNamespace}", enumName,
                column.PropertyType);
            file.RelativeDirectory = Path.Combine(context.Folders.DomainFolder,
                table.RelativeNamespace.Replace('.', Path.DirectorySeparatorChar));
            context.Add(file);
        }
    }

    private static GeneratedFile GenerateEnum(string domainNamespace, string enumName, string propertyType)
    {
        var sb = new TabbedStringBuilder();
        if (domainNamespace.Length > 0)
        {
            sb.AppendLine($"namespace {domainNamespace}");
            sb.AppendLineIndent("{");
        }

        if (propertyType != "int" && propertyType != "string")
        {
            sb.AppendLine($@"public enum {enumName} : {propertyType}");
        }
        else
        {
            sb.AppendLine($@"public enum {enumName}");
        }

        sb.AppendLineIndent("{");
        sb.AppendLine("NotSpecified");
        sb.DedentAppendLine("}");

        if (domainNamespace.Length > 0)
        {
            sb.DedentAppendLine("}");
        }

        return new GeneratedFile($"{enumName}", FileType.Domain, sb.ToString());
    }
}

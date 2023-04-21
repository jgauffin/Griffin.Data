using Griffin.Data.Helpers;
using Griffin.Data.Scaffolding.Config;
using Griffin.Data.Scaffolding.Meta;

namespace Griffin.Data.Scaffolding.Mapper.Generators;

/// <summary>
///     Generate a entity class.
/// </summary>
public class ClassGenerator : GeneratorWithNamespace
{
    protected override GeneratedFile GenerateFile(Table table, GeneratorContext context, string contents)
    {
        return new GeneratedFile($"{table.ClassName}", FileType.Domain, contents);
    }

    protected override void GenerateClass(TabbedStringBuilder sb, Table table, GeneratorContext context)
    {
        sb.AppendLine($@"public class {table.ClassName}");
        sb.AppendLineIndent("{");

        foreach (var reference in table.References)
        {
            var propName = reference.ReferencingTable.ClassName.Replace(table.ClassName, "").Pluralize();
            sb.AppendLine($"private readonly List<{reference.ReferencingTable.ClassName}> _{camelHump(propName)} = new();");
        }

        if (table.References.Any())
        {
            sb.AppendLine();
        }

        var allRequired = table.Columns.Where(x => !x.IsAutoIncrement && !x.IsNullable && string.IsNullOrEmpty(x.DefaultValue)).ToList();
        if (allRequired.Any())
        {
            sb.Append($"public {table.ClassName}(");
            for (var index = 0; index < allRequired.Count; index++)
            {
                var column = allRequired[index];
                sb.Append($"{column.CustomPropertyType ?? column.PropertyType} {camelHump(column.PropertyName)}");
                if (index < allRequired.Count - 1)
                {
                    sb.Append(", ");
                }
            }
            sb.AppendLine(")");
            sb.AppendLineIndent("{");
            foreach (var column in allRequired)
            {
                sb.AppendLine($"{column.PropertyName} = {camelHump(column.PropertyName)};");
            }
            sb.DedentAppendLine("}");
            sb.AppendLine();
        }

        foreach (var column in table.Columns)
        {
            var typeName = column.CustomPropertyType ?? column.PropertyType;
            if (!column.IsNullable)
            {
                sb.Append($"public {typeName} {column.PropertyName} {{ get; private set; }}");
            }
            else
            {
                sb.Append($"public {typeName} {column.PropertyName} {{ get; set; }}");
            }

            if (string.IsNullOrEmpty(column.DefaultValue))
            {
                sb.AppendLine();
                continue;
            }

            var valueStr = typeName == "string"
                ? $"\"{column.DefaultValue}\";"
                : $"{column.DefaultValue};";
            sb.AppendLine($" = {valueStr}");
        }

        sb.AppendLine();
        foreach (var reference in table.References)
        {
            var propName = reference.ReferencingTable.ClassName.Replace(table.ClassName, "").Pluralize();
            sb.AppendLine(
                $"public IReadOnlyList<{reference.ReferencingTable.ClassName}> {propName} => _{camelHump(propName)};");
            sb.AppendLine(
                $"// public {reference.ReferencingTable.ClassName} {reference.ReferencingTable.ClassName} {{ get; set; }}");
            sb.AppendLine();
        }

        sb.DedentAppendLine("}");
    }

    private string camelHump(string propertyName)
    {
        return char.ToLower(propertyName[0]) + propertyName[1..];
    }
    protected override string GetNamespaceName(Table table, ProjectFolders projectFolders)
    {
        return $"{projectFolders.DomainNamespace}.{table.RelativeNamespace}";
    }
}

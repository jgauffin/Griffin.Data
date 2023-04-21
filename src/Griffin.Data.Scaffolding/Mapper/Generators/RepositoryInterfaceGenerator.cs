using Griffin.Data.Helpers;
using Griffin.Data.Scaffolding.Config;

namespace Griffin.Data.Scaffolding.Mapper.Generators;

internal class RepositoryInterfaceGenerator : GeneratorWithNamespace
{
    protected override void AddUsings(Table table, TabbedStringBuilder sb, GeneratorContext context)
    {
        sb.AppendLine("using Griffin.Data.Domain;");
    }

    protected override void GenerateClass(TabbedStringBuilder sb, Table table, GeneratorContext context)
    {
        sb.AppendLine($"public interface I{table.ClassName}Repository : ICrudOperations<{table.ClassName}>");
        sb.AppendLineIndent("{");
        sb.Append($"Task<{table.ClassName}> GetById(");

        var pks = table.Columns.Where(x => x.IsPrimaryKey).ToList();
        for (var i = 0; i < pks.Count; i++)
        {
            var pk = pks[i];
            sb.Append($"{pk.PropertyType} {ToCamelCase(pk.PropertyName)}");
            if (i != pks.Count - 1)
            {
                sb.Append(", ");
            }
        }

        sb.AppendLine(");");

        sb.DedentAppendLine("}");
    }

    protected override GeneratedFile GenerateFile(Table table, GeneratorContext context, string contents)
    {
        return new GeneratedFile($"I{table.ClassName}Repository", FileType.Domain, contents);
    }

    protected override string GetNamespaceName(Table table, ProjectFolders projectFolders)
    {
        return $"{projectFolders.DomainNamespace}.{table.RelativeNamespace}";
    }

    private string ToCamelCase(string name)
    {
        return char.ToLower(name[0]) + name[1..];
    }
}

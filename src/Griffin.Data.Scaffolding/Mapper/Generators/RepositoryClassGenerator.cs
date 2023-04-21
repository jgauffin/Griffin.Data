﻿using Griffin.Data.Helpers;
using Griffin.Data.Scaffolding.Config;
using Griffin.Data.Scaffolding.Meta;

namespace Griffin.Data.Scaffolding.Mapper.Generators;

internal class RepositoryClassGenerator : GeneratorWithNamespace
{
    protected override GeneratedFile GenerateFile(Table table, GeneratorContext context, string contents)
    {
        return new GeneratedFile($"{table.ClassName}Repository", FileType.Data, contents);
    }

    protected override void AddUsings(Table table, TabbedStringBuilder sb, GeneratorContext context)
    {
        sb.AppendLine("using Griffin.Data;");
        sb.AppendLine($"using {context.Folders.DomainNamespace}.{table.RelativeNamespace};");
    }

    protected override void GenerateClass(TabbedStringBuilder sb, Table table, GeneratorContext context)
    {
        sb.AppendLine($"public class {table.ClassName}Repository : CrudOperations<{table.ClassName}>");
        sb.AppendLineIndent("{");

        sb.AppendLine($"public {table.ClassName}Repository(Session session) : base(session)");
        sb.AppendLineIndent("{");
        sb.AppendLine("if (session == null) throw new ArgumentNullException(nameof(session));");
        sb.DedentAppendLine("}");

        sb.Append($"public async Task<{table.ClassName}> GetById(");

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

        sb.AppendLine(")");
        sb.AppendLineIndent("{");
        sb.Append("return await Session.First(new {");

        for (var i = 0; i < pks.Count; i++)
        {
            var pk = pks[i];
            sb.Append($"{ToCamelCase(pk.PropertyName)}");
            if (i != pks.Count - 1)
            {
                sb.Append(", ");
            }
        }

        sb.AppendLine("});");
        sb.DedentAppendLine("}");


        sb.DedentAppendLine("}");
    }

    private string ToCamelCase(string name)
    {
        return char.ToLower(name[0]) + name[1..];
    }

    protected override string GetNamespaceName(Table table, ProjectFolders projectFolders)
    {
        return $"{projectFolders.DataNamespace}.{table.RelativeNamespace}";
    }
}

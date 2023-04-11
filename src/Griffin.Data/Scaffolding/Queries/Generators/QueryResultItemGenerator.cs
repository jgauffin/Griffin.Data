using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Griffin.Data.Scaffolding.Helpers;
using Griffin.Data.Scaffolding.Queries.Meta;

namespace Griffin.Data.Scaffolding.Queries.Generators;

public class QueryResultItemGenerator : IQueryGenerator
{
    private static readonly Dictionary<Type, string> Aliases =
        new()
        {
            { typeof(byte), "byte" },
            { typeof(sbyte), "sbyte" },
            { typeof(short), "short" },
            { typeof(ushort), "ushort" },
            { typeof(int), "int" },
            { typeof(uint), "uint" },
            { typeof(long), "long" },
            { typeof(ulong), "ulong" },
            { typeof(float), "float" },
            { typeof(double), "double" },
            { typeof(decimal), "decimal" },
            { typeof(object), "object" },
            { typeof(bool), "bool" },
            { typeof(char), "char" },
            { typeof(string), "string" },
            { typeof(void), "void" }
        };

    public Task<GeneratedFile> Generate(QueryMeta meta)
    {
        var sb = new TabbedStringBuilder();
        if (meta.Namespace.Length > 0)
        {
            sb.AppendLine($"namespace {meta.Namespace}.Queries");
            sb.AppendLineIndent("{");
        }

        sb.AppendLine($@"public class {meta.QueryName}ResultItem");
        sb.AppendLineIndent("{");

        foreach (var column in meta.Columns)
        {
            var typeName = Aliases.TryGetValue(column.PropertyType, out var a) ? a : column.PropertyType.Name;
            sb.AppendLine($"public {typeName} {column.Name} {{ get; set; }}");
        }

        sb.DedentAppendLine("}");

        if (meta.Namespace.Length > 0)
        {
            sb.DedentAppendLine("}");
        }

        return Task.FromResult(new GeneratedFile()
        {
            Contents = sb.ToString(),
            RelativeDirectory = "Queries\\",
            ClassName = meta.QueryName + "ResultItem"
        });
    }
}

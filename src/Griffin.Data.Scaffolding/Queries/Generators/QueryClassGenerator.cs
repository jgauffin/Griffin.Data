using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Griffin.Data.Helpers;
using Griffin.Data.Scaffolding.Helpers;
using Griffin.Data.Scaffolding.Meta;
using Griffin.Data.Scaffolding.Queries.Meta;

namespace Griffin.Data.Scaffolding.Queries.Generators;

/// <summary>
///     Generate a entity class.
/// </summary>
public class QueryClassGenerator : IQueryGenerator
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

    /// <summary>
    ///     Generate an entity class from a table.
    /// </summary>
    /// <param name="meta">Table to generate for.</param>
    /// <param name="allTables">All read tables (required to be able to build relations).</param>
    /// <returns>Generated class (including namespace).</returns>
    public Task<GeneratedFile> Generate(QueryMeta meta)
    {
        var sb = new TabbedStringBuilder();
        sb.AppendLine("using Griffin.Data.Queries;");
        sb.AppendLine();

        if (meta.Namespace.Length > 0)
        {
            sb.AppendLine($"namespace {meta.Namespace}.Queries");
            sb.AppendLineIndent("{");
        }

        sb.Append($@"public class {meta.QueryName}");
        if (meta.UseSorting && meta.UsePaging)
        {
            sb.AppendLine($" : IQuery<{meta.QueryName}Result>, IPagedQuery, ISortedQuery");
        }
        else if (meta.UseSorting)
        {
            sb.AppendLine($" : IQuery<{meta.QueryName}Result>, ISortedQuery");
        }
        else if (meta.UsePaging)
        {
            sb.AppendLine($" : IQuery<{meta.QueryName}Result>, IPagedQuery");
        }
        else
        {
            sb.AppendLine($" : IQuery<{meta.QueryName}Result>");
        }

        sb.AppendLineIndent("{");
        foreach (var parameter in meta.Parameters)
        {
            var typeName = Aliases.TryGetValue(parameter.PropertyType, out var a) ? a : parameter.PropertyType.Name;
            sb.AppendLine($"public {typeName} {char.ToUpper(parameter.Name[0])}{parameter.Name[1..]} {{ get; set; }}");
        }

        if (meta.UsePaging)
        {
            sb.AppendLine("/// <summary>");
            sb.AppendLine("/// One-based page number.");
            sb.AppendLine("/// </summary>");
            sb.AppendLine("public int? PageNumber { get; set; }");

            sb.AppendLine("/// <summary>");
            sb.AppendLine("/// Number of items per page.");
            sb.AppendLine("/// </summary>");
            sb.AppendLine("public int? PageSize { get; set; }");
        }

        if (meta.UseSorting)
        {
            sb.AppendLine("/// <summary>");
            sb.AppendLine("/// Sort result using property names");
            sb.AppendLine("/// </summary>");
            sb.AppendLine("public IList<SortEntry> SortEntries { get; set; } = new List<SortEntry>();");
        }

        sb.DedentAppendLine("}");

        if (meta.Namespace.Length > 0)
        {
            sb.DedentAppendLine("}");
        }

        return Task.FromResult(new GeneratedFile(meta.QueryName, sb.ToString())
        {
            RelativeDirectory = "Queries\\"
        });
    }
}

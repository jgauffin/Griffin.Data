using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
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
        if (meta.UseSorting && meta.UserPaging)
        {
            sb.AppendLine($" : PagedAndSortedQuery, IQuery<{meta.QueryName}Result>");
        }
        else if (meta.UseSorting)
        {
            sb.AppendLine($" : SortedQuery, IQuery<{meta.QueryName}Result>");
        }
        else if (meta.UserPaging)
        {
            sb.AppendLine($" : PagedQuery, IQuery<{meta.QueryName}Result>");
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
        
        sb.DedentAppendLine("}");

        if (meta.Namespace.Length > 0)
        {
            sb.DedentAppendLine("}");
        }

        return Task.FromResult(new GeneratedFile()
        {
            Contents = sb.ToString(),
            ClassName = meta.QueryName,
            RelativeDirectory = "Queries\\"
        });
    }
}

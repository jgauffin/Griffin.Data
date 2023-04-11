using System.Threading.Tasks;
using Griffin.Data.Helpers;
using Griffin.Data.Scaffolding.Helpers;
using Griffin.Data.Scaffolding.Queries.Meta;

namespace Griffin.Data.Scaffolding.Queries.Generators;

public class QueryResultGenerator:IQueryGenerator
{
    public Task<GeneratedFile> Generate(QueryMeta meta)
    {
        var sb = new TabbedStringBuilder();
        if (meta.Namespace.Length > 0)
        {
            sb.AppendLine($"namespace {meta.Namespace}.Queries");
            sb.AppendLineIndent("{");
        }

        sb.AppendLine($@"public class {meta.QueryName}Result");
        sb.AppendLineIndent("{");
        sb.AppendLine($"public IReadOnlyList<{meta.QueryName}ResultItem> Items {{ get; set; }}");
        sb.DedentAppendLine("}");

        if (meta.Namespace.Length > 0)
        {
            sb.DedentAppendLine("}");
        }

        return Task.FromResult(new GeneratedFile()
        {
            Contents = sb.ToString(),
            RelativeDirectory = "Queries\\",
            ClassName = meta.QueryName + "Result"
        });
    }
}

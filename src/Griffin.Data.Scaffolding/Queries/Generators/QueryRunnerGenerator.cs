using Griffin.Data.Helpers;
using Griffin.Data.Scaffolding.Queries.Meta;

namespace Griffin.Data.Scaffolding.Queries.Generators;

public class QueryRunnerGenerator : IQueryGenerator
{
    public Task<GeneratedFile> Generate(QueryMeta meta)
    {
        var sb = new TabbedStringBuilder();
        sb.AppendLine("using System.Data;");
        sb.AppendLine("using Griffin.Data;");
        sb.AppendLine("using Griffin.Data.Helpers;");
        sb.AppendLine("using Griffin.Data.Queries;");
        sb.AppendLine();

        if (meta.Namespace.Length > 0)
        {
            sb.AppendLine($"namespace {meta.Namespace}.Queries.Runners");
            sb.AppendLineIndent("{");
        }

        sb.AppendLine(
            $@"public class {meta.QueryName}Runner :  IQueryRunner<{meta.QueryName}, {meta.QueryName}Result>");
        sb.AppendLineIndent("{");
        sb.AppendLine("private readonly Session _session;");
        sb.AppendLine($"public {meta.QueryName}Runner(Session session)");
        sb.AppendLineIndent("{");
        sb.AppendLine("_session = session;");
        sb.DedentAppendLine("}");
        sb.AppendLine();

        GenerateQueryMethod(meta, sb);
        sb.AppendLine();
        GenerateMapMethod(meta, sb);

        sb.DedentAppendLine("}");
        if (meta.Namespace.Length > 0)
        {
            sb.DedentAppendLine("}");
        }

        return Task.FromResult(new GeneratedFile(meta.QueryName + "Runner", FileType.Data, sb.ToString())
        {
            RelativeDirectory = "QueryRunners\\"
        });
    }

    public static string GetReaderMethod(string sqlType)
    {
        return sqlType.ToLower() switch
        {
            "bigint" => "GetInt64",
            "binary" => "GetBytes",
            "image" => "GetBytes",
            "timestamp" => "GetBytes",
            "varbinary" => "GetBytes",
            "bit" => "GetBoolean",
            "char" => "GetString",
            "nchar" => "GetString",
            "nvarchar" => "GetString",
            "varchar" => "GetString",
            "text" => "GetString",
            "ntext" => "GetString",
            "date" => "GetDateTime",
            "datetime" => "GetDateTime",
            "datetime2" => "GetDateTime",
            "smalldatetime" => "GetDateTime",
            "decimal" => "GetDecimal",
            "money" => "GetDecimal",
            "smallmoney" => "GetDecimal",
            "float" => "GetFloat",
            "real" => "GetFloat",
            "int" => "GetInt32",
            "smallint" => "GetInt16",
            "time" => "GetTimeSpan",
            "uniqueidentifier" => "GetGuid",
            "xml" => "GetString",
            _ => throw new ArgumentException($"Unrecognized MSSQL data type: {sqlType}", nameof(sqlType))
        };
    }

    private static void GenerateMapMethod(QueryMeta meta, TabbedStringBuilder sb)
    {
        sb.AppendLine($"protected override void MapRecord(IDataRecord record, {meta.QueryName}ResultItem item)");
        sb.AppendLineIndent("{");
        var index = 0;
        foreach (var column in meta.Columns)
        {
            sb.AppendLine($"item.{column.Name} = record.{GetReaderMethod(column.SqlDataType)}({index++});");
        }

        sb.DedentAppendLine("}");
    }

    private void GenerateQueryMethod(QueryMeta meta, TabbedStringBuilder sb)
    {
        sb.AppendLine($"public async Task<{meta.QueryName}Result> Execute({meta.QueryName} query)");
        sb.AppendLineIndent("{");
        sb.AppendLine("await using var command = Session.CreateCommand();");
        sb.Append("command.CommandText = @\"");
        var reader = new StringReader(meta.SqlQuery);
        var isFirstLine = true;
        while (true)
        {
            var line = reader.ReadLine();
            if (line == null)
            {
                break;
            }

            if (isFirstLine)
            {
                isFirstLine = false;
            }
            else
            {
                sb.Append("                         ");
            }

            sb.AppendLine(line);
        }

        sb.RemoveLineEnding();
        sb.AppendLine("\";");
        sb.AppendLine();

        foreach (var parameter in meta.Parameters)
        {
            sb.AppendLine(
                $"command.AddParameter(\"{parameter.Name}\", query.{char.ToUpper(parameter.Name[0])}{parameter.Name[1..]});");
        }

        sb.AppendLine();

        if (meta.UsePaging)
        {
            sb.AppendLine("if (query.PageNumber != null)");
            sb.AppendLineIndent("{");
            sb.AppendLine(
                $"Session.Dialect.ApplyPaging(command, \"{meta.Columns[0].Name}\", query.PageNumber.Value, query.PageSize);");
            sb.DedentAppendLine("}");
            sb.AppendLine();
        }

        if (meta.UseSorting)
        {
            sb.AppendLine("if (query.SortEntries.Any())");
            sb.AppendLineIndent("{");
            sb.AppendLine("Session.Dialect.ApplySorting(command, query.SortEntries);");
            sb.DedentAppendLine("}");
            sb.AppendLine();
        }

        sb.AppendLine($"var items = await command.GenerateQueryResult<{meta.QueryName}Result>(MapRecord);");
        sb.AppendLine($"return new {meta.QueryName}Result {{ Items = items }};");
        sb.DedentAppendLine("}");
    }
}

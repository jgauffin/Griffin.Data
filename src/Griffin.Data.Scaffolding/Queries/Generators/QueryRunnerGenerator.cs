using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Griffin.Data.Helpers;
using Griffin.Data.Scaffolding.Helpers;
using Griffin.Data.Scaffolding.Queries.Meta;

namespace Griffin.Data.Scaffolding.Queries.Generators;

public class QueryRunnerGenerator : IQueryGenerator
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

        sb.AppendLine($@"public class {meta.QueryName}Runner :  ListRunner<{meta.QueryName}ResultItem>, IQueryRunner<{meta.QueryName}, {meta.QueryName}Result>");
        sb.AppendLineIndent("{");

        sb.AppendLine($"public {meta.QueryName}Runner(Session session) : base(session)");
        sb.AppendLine("{");
        sb.AppendLine("}");
        sb.AppendLine();

        GenerateQueryMethod(meta, sb);
        GenerateMapMethod(meta, sb);

        sb.DedentAppendLine("}");
        if (meta.Namespace.Length > 0)
        {
            sb.DedentAppendLine("}");
        }

        return Task.FromResult(new GeneratedFile(meta.QueryName + "Runner", sb.ToString())
        {
            RelativeDirectory = "QueryRunners\\",
        });
    }

    public static string GetReaderMethod(string sqlType)
    {
        switch (sqlType.ToLower())
        {
            case "bigint":
                return "GetInt64";
            case "binary":
            case "image":
            case "timestamp":
            case "varbinary":
                return "GetBytes";
            case "bit":
                return "GetBoolean";
            case "char":
            case "nchar":
            case "nvarchar":
            case "varchar":
            case "text":
            case "ntext":
                return "GetString";
            case "date":
            case "datetime":
            case "datetime2":
            case "smalldatetime":
                return "GetDateTime";
            case "decimal":
            case "money":
            case "smallmoney":
                return "GetDecimal";
            case "float":
            case "real":
                return "GetFloat";
            case "int":
                return "GetInt32";
            case "smallint":
                return "GetInt16";
            case "time":
                return "GetTimeSpan";
            case "uniqueidentifier":
                return "GetGuid";
            case "xml":
                return "GetString";
            default:
                throw new ArgumentException($"Unrecognized MSSQL data type: {sqlType}", nameof(sqlType));
        }
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
        bool isFirstLine = true;
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
            sb.AppendLine($"command.AddParameter(\"{parameter.Name}\", query.{char.ToUpper(parameter.Name[0])}{parameter.Name[1..]});");
        }

        if (meta.UsePaging)
        {
            sb.AppendLine("if (query.PageNumber != null)");
            sb.AppendLineIndent("{");
            sb.AppendLine($"Session.Dialect.ApplyPaging(command, \"{meta.Columns[0].Name}\", query.PageNumber.Value, query.PageSize);");
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

        sb.AppendLine($"return new {meta.QueryName}Result {{ Items = await MapRecords(command) }};");
        sb.DedentAppendLine("}");
    }
}

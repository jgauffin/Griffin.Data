using System.Linq;
using Griffin.Data.Scaffolding.Helpers;
using Griffin.Data.Scaffolding.Meta;

namespace Griffin.Data.Scaffolding.Generators;

public class MappingGenerator
{
    public string Generate(Table table)
    {
        var sb = new TabbedStringBuilder();
        sb.AppendLine("using Griffin.Data;");
        sb.AppendLine("using Griffin.Data.Configuration;");
        sb.AppendLine();

        if (table.Namespace.Length > 0)
        {
            sb.AppendLine($"namespace {table.Namespace}.Data.Mappings");
            sb.AppendLineIndent("{");
            sb.AppendLine();
        }

        sb.AppendLine($@"class {table.ClassName}Mapping : IEntityConfigurator<{table.ClassName}>");
        sb.AppendLineIndent("{");
        sb.AppendLine($"public void Configure(IClassMappingConfigurator<{table.ClassName}> config)");
        sb.AppendLineIndent("{");
        sb.AppendLine($"config.TableName(\"{table.TableName}\");");

        foreach (var column in table.Columns)
        {
            if (column.IsPrimaryKey)
            {
                sb.Append($"config.Key(x => x.{column.PropertyName}).AutoIncrement()");
            }
            else
            {
                sb.Append($"config.Property(x => x.{column.PropertyName})");
            }

            if (column.ColumnName != column.PropertyName)
            {
                sb.Append($".ColumnName(\"{column.ColumnName}\")");
            }

            sb.AppendLine(";");
        }

        foreach (var reference in table.References)
        {
            var childColumn = reference.ReferencingTable.Columns.First(x => x.ColumnName == reference.ForeignKeyColumn);
            var propName = reference.ReferencingTable.ClassName.Replace(table.ClassName, "").Pluralize();

            sb.AppendLine($"config.HasMany(x => x.{propName})");
            sb.AppendLine($"      .ForeignKey(x => x.{childColumn.PropertyName})");
            sb.AppendLine($"      .References(x => x.{reference.ReferencedColumn});");

            sb.AppendLine($"//config.HasOneConfigurator(x => x.{reference.ReferencingTable.ClassName})");
            sb.AppendLine($"//      .ForeignKey(x => x.{childColumn.PropertyName})");
            sb.AppendLine($"//      .References(x => x.{reference.ReferencedColumn});");
            sb.AppendLine();
        }

        sb.DedentAppendLine("}");
        sb.DedentAppendLine("}");

        return sb.ToString();
    }
}

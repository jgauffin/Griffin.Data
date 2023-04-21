using Griffin.Data.Helpers;
using Griffin.Data.Scaffolding.Config;
using Griffin.Data.Scaffolding.Meta;

namespace Griffin.Data.Scaffolding.Mapper.Generators;

public class MappingGenerator : GeneratorWithNamespace
{
    protected override GeneratedFile GenerateFile(Table table, GeneratorContext context, string contents)
    {
        return new GeneratedFile($"{table.ClassName}Mapping", FileType.Data, contents);
    }

    protected override void AddUsings(Table table, TabbedStringBuilder sb, GeneratorContext context)
    {
        sb.AppendLine("using Griffin.Data;");
        sb.AppendLine("using Griffin.Data.Configuration;");
        sb.AppendLine($"using {context.Folders.DomainFolder}.{table.RelativeNamespace};");
    }

    protected override void GenerateClass(TabbedStringBuilder sb, Table table, GeneratorContext context)
    {
        sb.AppendLine($@"class {table.ClassName}Mapping : IEntityConfigurator<{table.ClassName}>");
        sb.AppendLineIndent("{");
        sb.AppendLine($"public void Configure(IClassMappingConfigurator<{table.ClassName}> config)");
        sb.AppendLineIndent("{");
        sb.AppendLine($"config.TableName(\"{table.Name}\");");

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

            if (column.Name != column.PropertyName)
            {
                sb.Append($".ColumnName(\"{column.Name}\")");
            }

            sb.AppendLine(";");
        }

        foreach (var reference in table.References)
        {
            var childColumn = reference.ReferencingTable.Columns.First(x => x.Name == reference.ForeignKeyColumn);
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
    }

    protected override string GetNamespaceName(Table table, ProjectFolders projectFolders)
    {
        return $"{projectFolders.DataFolder}.{table.RelativeNamespace}.Mappings";
    }
}

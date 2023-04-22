using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Griffin.Data.Helpers;

namespace Griffin.Data.Scaffolding.Mapper.Generators
{
    internal class RepositoryTestGenerator : GeneratorWithNamespace
    {
        protected override void AddUsings(Table table, TabbedStringBuilder sb, GeneratorContext context)
        {
            sb.AppendLine("using XUnit;");
        }

        protected override void GenerateClass(TabbedStringBuilder sb, Table table, GeneratorContext context)
        {
            sb.AppendLine($"public class {table.ClassName}RepositoryTests : IntegrationTest");
            sb.AppendLineIndent("{");
            sb.AppendLine();
            sb.AppendLine("[Fact]");
            sb.AppendLine("public async Task Should_be_able_to_insert_entity()");
            sb.AppendLineIndent("{");
            sb.AppendLine("var entity = CreateValidEntity();");
            sb.AppendLine();
            sb.AppendLine("await Session.Insert(entity);");
            sb.AppendLine("");

            if (table.Columns.Any(x => x.IsAutoIncrement))
            {
                sb.AppendLine();
            }


            sb.AppendLine();
            CreateEntityFactory(sb, table);
        }

        private static void CreateEntityFactory(TabbedStringBuilder sb, Table table)
        {
            sb.AppendLine($"private {table.ClassName} CreateValidEntity()");
            sb.AppendLineIndent("{");
            sb.Append($"var entity = new {table.ClassName}(");
            var allRequired = table.Columns
                .Where(x => !x.IsAutoIncrement && !x.IsNullable && string.IsNullOrEmpty(x.DefaultValue)).ToList();
            if (allRequired.Any())
            {
                for (var index = 0; index < allRequired.Count; index++)
                {
                    var column = allRequired[index];
                    sb.Append(GetSampleValue(column.CustomPropertyType, column.PropertyType));
                    if (index < allRequired.Count - 1)
                    {
                        sb.Append(", ");
                    }
                }
            }

            sb.AppendLine(");");
            sb.AppendLine("return entity;");
            sb.DedentAppendLine("}");
        }

        private static string GetSampleValue(string? customPropertyType, string propertyType)
        {
            if (customPropertyType != null)
            {
                return $"{customPropertyType}.NotSpecified";
            }

            return propertyType switch
            {
                "string" => "\"\"",
                "int" => "1",
                "long" => "1",
                "byte" => "(byte)1",
                "DateTime" => "DateTime.UtcNow",
                "TimeSpan" => "TimeSpan.FromSeconds(1)",
                _ => "-1"
            };
        }

        protected override GeneratedFile GenerateFile(Table table, GeneratorContext context, string contents)
        {
            return new GeneratedFile(table.ClassName + "RepositoryTests", FileType.DataTest, contents);
        }
    }
}

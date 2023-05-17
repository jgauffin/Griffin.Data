using Griffin.Data.Helpers;
using Griffin.Data.Scaffolding.Config;

namespace Griffin.Data.Scaffolding.Mapper.Generators;

internal class RepositoryTestGenerator : GeneratorWithNamespace
{
    private readonly Random _random = new();

    protected override void AddUsings(Table table, TabbedStringBuilder sb, GeneratorContext context)
    {
        sb.AppendLine("using FluentAssertions;");
        sb.AppendLine("using Griffin.Data.Mapper;");
        sb.AppendLine("using Xunit;");
        sb.AppendLine($"using {context.Folders.DomainNamespace}.{table.RelativeNamespace};");
        sb.AppendLine($"using {context.Folders.DataNamespace}.{table.RelativeNamespace};");
    }

    protected override void GenerateClass(TabbedStringBuilder sb, Table table, GeneratorContext context)
    {
        sb.AppendLine($"public class {table.ClassName}RepositoryTests : IntegrationTest");
        sb.AppendLineIndent("{");
        sb.AppendLine($"private readonly {table.ClassName}Repository _repository;");
        sb.AppendLine();
        sb.AppendLine($"public {table.ClassName}RepositoryTests()");
        sb.AppendLineIndent("{");
        sb.AppendLine($"_repository = new {table.ClassName}Repository(Session);");
        sb.DedentAppendLine("}");
        sb.AppendLine();

        CreateInsertMethod(sb, table);
        sb.AppendLine();
        CreateUpdateMethod(sb, table);
        sb.AppendLine();
        CreateDeleteMethod(sb, table);
        sb.AppendLine();
        CreateEntityFactory(sb, table);
        sb.AppendLine();
        sb.DedentAppendLine("}");
    }

    protected override GeneratedFile GenerateFile(Table table, GeneratorContext context, string contents)
    {
        return new GeneratedFile(table.ClassName + "RepositoryTests", FileType.DataTest, contents);
    }

    protected override string GetNamespaceName(Table table, ProjectFolders projectFolders)
    {
        return $"{projectFolders.DataTestFolder}.{table.RelativeNamespace}";
    }

    private static void CreateDeleteMethod(TabbedStringBuilder sb, Table table)
    {
        sb.AppendLine("[Fact]");
        sb.AppendLine("public async Task Should_be_able_to_delete_entity()");
        sb.AppendLineIndent("{");
        sb.AppendLine("var entity = CreateValidEntity();");
        sb.AppendLine("await Session.Insert(entity);");
        sb.AppendLine("");
        sb.AppendLine("await _repository.Delete(entity);");
        sb.AppendLine();

        FetchActual(sb, table);
        sb.AppendLine("actual.Should().BeNull();");

        sb.DedentAppendLine("}");
    }

    private void CreateEntityFactory(TabbedStringBuilder sb, Table table)
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

    private static void CreateInsertMethod(TabbedStringBuilder sb, Table table)
    {
        sb.AppendLine("[Fact]");
        sb.AppendLine("public async Task Should_be_able_to_insert_entity()");
        sb.AppendLineIndent("{");
        sb.AppendLine("var entity = CreateValidEntity();");
        sb.AppendLine();
        sb.AppendLine("await _repository.Create(entity);");
        sb.AppendLine("");

        var autoColumn = table.Columns.FirstOrDefault(x => x.IsAutoIncrement);
        if (autoColumn != null)
        {
            sb.AppendLine($"entity.{autoColumn.PropertyName}.Should().BeGreaterThan(1);");
        }

        sb.DedentAppendLine("}");
    }

    private void CreateUpdateMethod(TabbedStringBuilder sb, Table table)
    {
        sb.AppendLine("[Fact]");
        sb.AppendLine("public async Task Should_be_able_to_update_entity()");
        sb.AppendLineIndent("{");
        sb.AppendLine("var entity = CreateValidEntity();");
        sb.AppendLine("await Session.Insert(entity);");
        sb.AppendLine("");
        foreach (var column in table.Columns)
        {
            if (column.IsPrimaryKey || !column.IsNullable)
            {
                continue;
            }

            sb.AppendLine(
                $"entity.{column.PropertyName} = {GetSampleValue(column.CustomPropertyType, column.PropertyType)};");
        }

        sb.AppendLine();
        sb.AppendLine("await _repository.Update(entity);");
        sb.AppendLine();

        FetchActual(sb, table);
        sb.AppendLine("actual.Should().NotBeNull();");
        foreach (var column in table.Columns)
        {
            if (column.IsPrimaryKey)
            {
                continue;
            }

            switch (column.PropertyType)
            {
                case "bool":
                    sb.AppendLine($"actual.{column.PropertyName}.Should().BeTrue()");
                    break;
                case "DateTime":
                    sb.AppendLine($"actual.{column.PropertyName}.Should().BeCloseTo(entity.{column.PropertyName}, TimeSpan.FromMilliseconds(100));");
                    break;
                default:
                    sb.AppendLine($"actual.{column.PropertyName}.Should().Be(entity.{column.PropertyName});");
                    break;
            }
        }

        sb.DedentAppendLine("}");
    }

    private static void FetchActual(TabbedStringBuilder sb, Table table)
    {
        sb.Append($"var actual = await Session.FirstOrDefault<{table.ClassName}>(new {{");
        var pks = table.Columns.Where(x => x.IsPrimaryKey).ToList();
        for (var i = 0; i < pks.Count; i++)
        {
            var pk = pks[i];
            sb.Append($"entity.{pk.PropertyName}");
            if (i < pks.Count - 1)
            {
                sb.Append(", ");
            }
        }

        sb.AppendLine("});");
    }

    private string GetSampleValue(string? customPropertyType, string propertyType)
    {
        if (customPropertyType != null)
        {
            return $"{customPropertyType}.NotSpecified";
        }

        return propertyType switch
        {
            "string" => $"\"{_random.Next(0, 9999)}\"",
            "int" => _random.Next(0, int.MaxValue).ToString(),
            "long" => _random.Next(0, int.MaxValue).ToString(),
            "byte" => $"(byte){_random.Next(0, 255)}",
            "bool" => "true",
            "DateTime" => "DateTime.UtcNow",
            "TimeSpan" => "TimeSpan.FromSeconds(1)",
            "Guid" => "Guid.NewGuid()",
            _ => "-1"
        };
    }
}

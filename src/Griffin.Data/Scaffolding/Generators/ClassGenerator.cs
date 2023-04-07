using System;
using System.Collections.Generic;
using Griffin.Data.Scaffolding.Helpers;
using Griffin.Data.Scaffolding.Meta;

namespace Griffin.Data.Scaffolding.Generators;

/// <summary>
///     Generate a entity class.
/// </summary>
public class ClassGenerator
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
    /// <param name="table">Table to generate for.</param>
    /// <param name="allTables">All read tables (required to be able to build relations).</param>
    /// <returns>Generated class (including namespace).</returns>
    public string Generate(Table table, IReadOnlyList<Table> allTables)
    {
        if (table == null)
        {
            throw new ArgumentNullException(nameof(table));
        }

        if (allTables == null)
        {
            throw new ArgumentNullException(nameof(allTables));
        }

        var sb = new TabbedStringBuilder();
        if (table.Namespace.Length > 0)
        {
            sb.AppendLine($"namespace {table.Namespace}.Domain");
            sb.AppendLineIndent("{");
        }

        sb.AppendLine($@"public class {table.ClassName}");
        sb.AppendLineIndent("{");
        foreach (var column in table.Columns)
        {
            var typeName = Aliases.TryGetValue(column.PropertyType, out var a) ? a : column.PropertyType.Name;
            sb.Append($"public {typeName} {column.PropertyName} {{ get; set; }}");
            if (string.IsNullOrEmpty(column.DefaultValue))
            {
                sb.AppendLine();
                continue;
            }

            if (column.PropertyType == typeof(string))
            {
                sb.AppendLine($" = \"{column.DefaultValue}\";");
            }
            else
            {
                sb.AppendLine($" = {column.DefaultValue};");
            }
        }

        foreach (var reference in table.References)
        {
            var propName = reference.ReferencingTable.ClassName.Replace(table.ClassName, "").Pluralize();
            sb.AppendLine(
                $"public IReadOnlyList<{reference.ReferencingTable.ClassName}> {propName} {{ get; private set; }} = new List<{reference.ReferencingTable.ClassName}>();");
            sb.AppendLine(
                $"// public {reference.ReferencingTable.ClassName} {reference.ReferencingTable.ClassName} {{ get; set; }}");
        }

        sb.DedentAppendLine("}");

        if (table.Namespace.Length > 0)
        {
            sb.DedentAppendLine("}");
        }

        return sb.ToString();
    }
}

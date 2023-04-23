using Griffin.Data.Helpers;

namespace Griffin.Data.Scaffolding.Mapper.Analyzers;

/// <summary>
///     Goes through hierarchy to generate namespaces in relation to parent/children.
/// </summary>
internal class NamespaceAnalyzer : IMetaAnalyzer
{
    public int Priority => 1;

    public void Analyze(GeneratorContext context)
    {
        // Try to find hierarchy using names
        var mappings = new List<NamespaceWip>();
        foreach (var table in context.Tables.OrderBy(x => x.ClassName.Length))
        {
            Table? minTable = null;
            foreach (var parentTable in context.Tables)
            {
                if (parentTable.Name.Length >= table.Name.Length || !table.ClassName.StartsWith(parentTable.ClassName))
                {
                    continue;
                }

                if (minTable == null)
                {
                    minTable = parentTable;
                }
                else if (parentTable.ClassName.Length > minTable.ClassName.Length &&
                         parentTable.ClassName.Length < table.ClassName.Length)
                {
                    minTable = parentTable;
                }
            }

            if (minTable != null)
            {
                var parentWip = mappings.First(x => x.Table == minTable);
                parentWip.HasChildren = true;
                mappings.Add(new NamespaceWip(table) { Parent = parentWip });
            }
            else
            {
                mappings.Add(new NamespaceWip(table));
            }
        }

        foreach (var wip in mappings.Where(x => x.Parent == null))
        {
            wip.Table.RelativeNamespace = $"{wip.Table.ClassName.Pluralize()}";
        }

        foreach (var wip in mappings.Where(x => x.Parent != null).OrderBy(x => x.Table.ClassName.Length))
        {
            if (wip.Parent != null)
            {
                wip.Table.RelativeNamespace = wip.HasChildren
                    ? $"{wip.Parent.Table.RelativeNamespace}.{wip.Table.ClassName.Pluralize()}"
                    : $"{wip.Parent.Table.RelativeNamespace}";
            }
            else
            {
                wip.Table.RelativeNamespace = $"{wip.Table.ClassName.Pluralize()}";
            }
        }
    }

    //private string[] HumpName(string tableName)
    //{
    //    List<string> nameParts = new List<string>();
    //    var lastIndex = 0;
    //    for (int i = 0; i < tableName.Length; i++)
    //    {
    //        if (char.IsUpper(tableName[i]))
    //        {
    //            var part = tableName[lastIndex..i];
    //            nameParts.Add(part);
    //            lastIndex = i + 1;
    //        }
    //    }

    //    nameParts.Add(tableName[lastIndex..]);
    //    return nameParts.ToArray();
    //}

    //private string HumpName2(string tableName)
    //{
    //    List<string> nameParts = new List<string>();
    //    var lastIndex = 0;
    //    for (int i = 0; i < tableName.Length; i++)
    //    {
    //        if (char.IsUpper(tableName[i]))
    //        {
    //            var part = tableName[lastIndex..i];
    //            nameParts.Add(part);
    //            lastIndex = i + 1;
    //        }
    //    }

    //    nameParts.Add(tableName[lastIndex..]);
    //    return nameParts.ToArray();
    //}
}

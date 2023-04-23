namespace Griffin.Data.Scaffolding.Mapper.Analyzers;

/// <summary>
///     A shrink for classes ;)
/// </summary>
/// <remarks>
///     <para>
///         Removes the parents name suffix from class names.
///     </para>
/// </remarks>
internal class ClassNameShrinker : IMetaAnalyzer
{
    public int Priority => 2;

    public void Analyze(GeneratorContext context)
    {
        foreach (var table in context.Tables)
        {
            var possibleParents = new List<Table>();
            foreach (var foreignKey in table.ForeignKeys)
            {
                if (table.ClassName.StartsWith(foreignKey.ReferencedTable.ClassName,
                        StringComparison.OrdinalIgnoreCase) && table.ClassName != foreignKey.ReferencedTable.ClassName)
                {
                    possibleParents.Add(foreignKey.ReferencedTable);
                }
            }

            var largestParent = possibleParents.MaxBy(x => x.ClassName.Length);
            if (largestParent != null)
            {
                table.RemoveParentSuffix(largestParent.ClassName);
            }
        }
    }
}

namespace Griffin.Data.Scaffolding.Meta.Analyzers;

internal class NamespaceWip
{
    public NamespaceWip(Table table)
    {
        Table = table ?? throw new ArgumentNullException(nameof(table));
    }

    public bool HasChildren { get; set; }
    public NamespaceWip? Parent { get; set; }

    public Table Table { get; }
}

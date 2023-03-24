namespace Griffin.Data.Scaffolding.Meta;

public class Reference
{
    public string ReferencedColumn { get; }
    public Table ReferencingTable { get; }
    public string ForeignKeyColumn { get; }

    public Reference(string referencedColumn, Table referencingTable, string foreignKeyColumn)
    {
        ReferencedColumn = referencedColumn;
        ReferencingTable = referencingTable;
        ForeignKeyColumn = foreignKeyColumn;
    }
}
namespace Griffin.Data.Scaffolding.Meta;

public class ForeignKeyColumn
{
    public ForeignKeyColumn(string column, Table referencedTable, string referencedColumn)
    {
        Column = column;
        ReferencedTable = referencedTable;
        ReferencedColumn = referencedColumn;
    }

    public string Column { get; }
    public string ReferencedColumn { get; }
    public Table ReferencedTable { get; }
}

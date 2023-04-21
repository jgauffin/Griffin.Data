using System;

namespace Griffin.Data.Scaffolding.Meta;

/// <summary>
/// A reference to the table that contains a list of these ;)
/// </summary>
public class Reference
{
    /// <summary>
    /// Column being referenced.
    /// </summary>
    public string ReferencedColumn { get; }

    /// <summary>
    /// Table referencing this one (the specified table contains the FK).
    /// </summary>
    public Table ReferencingTable { get; }

    /// <summary>
    /// foreign key column.
    /// </summary>
    public string ForeignKeyColumn { get; }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="referencedColumn">Column being referenced.
    /// </param>
    /// <param name="referencingTable">Table referencing this one (the specified table contains the FK).</param>
    /// <param name="foreignKeyColumn">foreign key column.</param>
    public Reference(string referencedColumn, Table referencingTable, string foreignKeyColumn)
    {
        ReferencedColumn = referencedColumn ?? throw new ArgumentNullException(nameof(referencedColumn));
        ReferencingTable = referencingTable ?? throw new ArgumentNullException(nameof(referencingTable));
        ForeignKeyColumn = foreignKeyColumn ?? throw new ArgumentNullException(nameof(foreignKeyColumn));
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{ReferencedColumn} is referenced from {ReferencingTable.Name}.{ForeignKeyColumn}";
    }
}

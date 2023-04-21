using System;

namespace Griffin.Data.Scaffolding;

/// <summary>
///     Foreign key read from the database schema.
/// </summary>
public class ForeignKeyColumn
{
    /// <summary>
    /// </summary>
    /// <param name="column">Name of column.</param>
    /// <param name="referencedTable">Column references in the parent table.</param>
    /// <param name="referencedColumn">Table being referenced from through the foreign key.</param>
    public ForeignKeyColumn(string column, Table referencedTable, string referencedColumn)
    {
        Column = column ?? throw new ArgumentNullException(nameof(column));
        ReferencedTable = referencedTable ?? throw new ArgumentNullException(nameof(referencedTable));
        ReferencedColumn = referencedColumn ?? throw new ArgumentNullException(nameof(referencedColumn));
    }

    /// <summary>
    ///     Name of column.
    /// </summary>
    public string Column { get; }

    /// <summary>
    ///     Column references in the parent table.
    /// </summary>
    public string ReferencedColumn { get; }

    /// <summary>
    ///     Table being referenced from through the foreign key.
    /// </summary>
    public Table ReferencedTable { get; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Column} references {ReferencedTable.Name}.{ReferencedColumn}";
    }
}

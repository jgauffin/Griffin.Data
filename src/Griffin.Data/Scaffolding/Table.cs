using System;
using System.Collections.Generic;
using System.Linq;
using Griffin.Data.Helpers;
using Griffin.Data.Scaffolding.Meta;

namespace Griffin.Data.Scaffolding;

/// <summary>
///     Table meta data.
/// </summary>
public class Table
{
    /// <summary>
    /// </summary>
    /// <param name="tableName">Name of the table.</param>
    /// <exception cref="ArgumentNullException">Name is not specified.</exception>
    public Table(string tableName)
    {
        Name = tableName ?? throw new ArgumentNullException(nameof(tableName));
        ClassName = tableName.ToPascalCase().Singularize();
    }

    /// <summary>
    ///     Suggested class name after the table name has been cleaned and normalized.
    /// </summary>
    public string ClassName { get; set; }

    /// <summary>
    ///     Columns defined in the table.
    /// </summary>
    public List<Column> Columns { get; set; } = new();

    /// <summary>
    ///     Foreign keys defined in this table.
    /// </summary>
    public ICollection<ForeignKeyColumn> ForeignKeys { get; set; } = new List<ForeignKeyColumn>();

    /// <summary>
    ///     Generated from a view.
    /// </summary>
    public bool IsView { get; set; }

    /// <summary>
    ///     Column accessor using the column name.
    /// </summary>
    /// <param name="columnName">Column name</param>
    /// <returns></returns>
    public Column this[string columnName] => GetColumn(columnName);

    /// <summary>
    ///     Name of table (no cleaning have been performed).
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     References to this table (from others).
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Will be generated once foreign key has been analyzed for all tables.
    ///     </para>
    /// </remarks>
    public ICollection<Reference> References { get; set; } = new List<Reference>();

    /// <summary>
    ///     Suggested namespace from the project root.
    /// </summary>
    public string RelativeNamespace { get; set; } = "";

    /// <summary>
    ///     Schema name.
    /// </summary>
    public string? SchemaName { get; set; }

    /// <summary>
    ///     Sequence name used for this table (to generate the primary key).
    /// </summary>
    public string SequenceName { get; set; } = "";

    /// <summary>
    ///     Get a specific column (case insensitive search)..
    /// </summary>
    /// <param name="columnName">Name</param>
    /// <returns></returns>
    public Column GetColumn(string columnName)
    {
        return Columns.Single(x => string.Compare(x.Name, columnName, StringComparison.OrdinalIgnoreCase) == 0);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"Table {Name}";
    }
}

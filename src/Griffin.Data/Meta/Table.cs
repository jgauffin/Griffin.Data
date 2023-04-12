using System;
using System.Collections.Generic;
using System.Linq;

namespace Griffin.Data.Meta;

/// <summary>
///     A class.
/// </summary>
public class Table
{
    /// <summary>
    ///     Class name (generated from a cleaned up version of the column name).
    /// </summary>
    public string ClassName { get; set; } = "";

    /// <summary>
    ///     Cleaned up table name.
    /// </summary>
    public string CleanName { get; set; } = "";

    /// <summary>
    ///     All columns in this table.
    /// </summary>
    public List<Column> Columns { get; set; } = new List<Column>();

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
    public string Name { get; set; } = "";

    /// <summary>
    ///     Primary key columns
    /// </summary>
    public IReadOnlyList<Column> PrimaryKey => Columns.Where(x => x.IsPrimaryKey).ToList();

    /// <summary>
    ///     Schema name (if specified).
    /// </summary>
    public string? Schema { get; set; }

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
}

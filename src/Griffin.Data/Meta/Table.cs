using System.Collections.Generic;
using System.Linq;

namespace Griffin.Data.Meta;

/// <summary>
///     A class.
/// </summary>
public class Table
{
    public string ClassName { get; set; }
    public string CleanName { get; set; }
    public List<Column> Columns { get; set; }
    public bool Ignore { get; set; }
    public bool IsView { get; set; }

    public Column this[string columnName] => GetColumn(columnName);
    public string Name { get; set; }

    public Column PrimaryKey => Columns.SingleOrDefault(x => x.IsPrimaryKey);
    public string? Schema { get; set; }
    public string SequenceName { get; set; }

    public Column GetColumn(string columnName)
    {
        return Columns.Single(x => string.Compare(x.Name, columnName, true) == 0);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace Griffin.Data.Meta;

/// <summary>
/// Collection of tables.
/// </summary>
public class TableCollection : List<Table>
{
    /// <summary>
    /// Get a specific table.
    /// </summary>
    /// <param name="tableName">Name of table.</param>
    /// <returns>Table.</returns>
    public Table this[string tableName] => GetTable(tableName);

    /// <summary>
    /// Get a specific table.
    /// </summary>
    /// <param name="tableName">Name of table.</param>
    /// <returns></returns>
    public Table GetTable(string tableName)
    {
        if (tableName == null)
        {
            throw new ArgumentNullException(nameof(tableName));
        }

        return
            this.Single(
                x => string.Compare(x.Name, tableName, StringComparison.OrdinalIgnoreCase) == 0);
    }
}

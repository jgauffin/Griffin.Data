using System;
using System.Collections.Generic;
using System.Linq;

namespace Griffin.Data.Meta;

public class TableCollection : List<Table>
{
    public Table this[string tableName] => GetTable(tableName);

    public Table GetTable(string tableName)
    {
        return
            this.Single(
                x => string.Compare(x.Name, tableName, StringComparison.OrdinalIgnoreCase) == 0);
    }
}

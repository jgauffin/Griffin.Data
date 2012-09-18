using System;
using System.Collections.Generic;
using System.Linq;

namespace Griffin.Data
{
    public class TableCollection : List<Table>
    {
        public Table this[string tableName]
        {
            get { return GetTable(tableName); }
        }

        public Table GetTable(string tableName)
        {
            return
                this.Single(
                    x => String.Compare(x.Name, tableName, StringComparison.OrdinalIgnoreCase) == 0);
        }
    }
}
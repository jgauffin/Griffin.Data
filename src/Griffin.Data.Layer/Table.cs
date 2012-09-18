using System.Collections.Generic;
using System.Linq;

namespace Griffin.Data
{
    public class Table
    {
        public List<Column> Columns;	
        public string Name;
        public string Schema;
        public bool IsView;
        public string CleanName;
        public string ClassName;
        public string SequenceName;
        public bool Ignore;

        public Column PrimaryKey
        {
            get
            {
                return this.Columns.SingleOrDefault(x=>x.IsPrimaryKey);
            }
        }

        public Column GetColumn(string columnName)
        {
            return Columns.Single(x=>string.Compare(x.Name, columnName, true)==0);
        }

        public Column this[string columnName]
        {
            get
            {
                return GetColumn(columnName);
            }
        }

    }
}
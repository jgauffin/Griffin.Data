using System;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace Griffin.Data
{
    abstract class SchemaReader
    {
        private static Regex rxCleanUp = new Regex(@"[^\w\d_]", RegexOptions.Compiled);

        protected virtual string CleanUp(string name)
        {
            var str = rxCleanUp.Replace(name, "_");
            if (char.IsDigit(str[0])) str = "_" + str;

            return str;
        }

        private string CheckNullable(Column col)
        {
            string result = "";
            if (col.IsNullable &&
                col.PropertyType != "byte[]" &&
                col.PropertyType != "string" &&
                col.PropertyType != "Microsoft.SqlServer.Types.SqlGeography" &&
                col.PropertyType != "Microsoft.SqlServer.Types.SqlGeometry"
                )
                result = "?";
            return result;
        }


        public abstract TableCollection ReadSchema(DbConnection connection, DbProviderFactory factory);
        public void WriteLine(string o)
        {
        }

    }
}
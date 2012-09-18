using System;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace Griffin.Data
{
    public class Helpers
    {
        public string ClassSuffix { get; set; }
        public string ClassPrefix { get; set; }

        TableCollection LoadTables()
        {

            DbProviderFactory _factory;
            _factory = DbProviderFactories.GetFactory(ProviderName);
            TableCollection result;
            using (var conn = _factory.CreateConnection())
            {
                conn.ConnectionString = ConnectionString;
                conn.Open();

                SchemaReader reader = null;

                if (_factory.GetType().Name == "MySqlClientFactory")
                {
                    // MySql
                    reader = new MySqlSchemaReader();
                }
                else if (_factory.GetType().Name == "SqlCeProviderFactory")
                {
                    // SQL CE
                    reader = new SqlServerCeSchemaReader();
                }
                else if (_factory.GetType().Name == "NpgsqlFactory")
                {
                    // PostgreSQL
                    reader = new PostGreSqlSchemaReader();
                }
                else if (_factory.GetType().Name == "OracleClientFactory")
                {
                    // Oracle
                    reader = new OracleSchemaReader();
                }
                else
                {
                    // Assume SQL Server
                    reader = new SqlServerSchemaReader();
                }

                //reader.outer = this;
                result = reader.ReadSchema(conn, _factory);

                // Remove unrequired tables/views
                for (int i = result.Count - 1; i >= 0; i--)
                {
                    /*TODO: Add again
                    if (SchemaName != null && string.Compare(result[i].Schema, SchemaName, true) != 0)
                    {
                        result.RemoveAt(i);
                        continue;
                    }
                    if (!IncludeViews && result[i].IsView)
                    {
                        result.RemoveAt(i);
                        continue;
                    }
                     * */
                }

                conn.Close();


                var rxClean = new Regex("^(Equals|GetHashCode|GetType|ToString|repo|Save|IsNew|Insert|Update|Delete|Exists|SingleOrDefault|Single|First|FirstOrDefault|Fetch|Page|Query)$");
                foreach (var t in result)
                {
                    t.ClassName = ClassPrefix + t.ClassName + ClassSuffix;
                    foreach (var c in t.Columns)
                    {
                        c.PropertyName = rxClean.Replace(c.PropertyName, "_$1");

                        // Make sure property name doesn't clash with class name
                        if (c.PropertyName == t.ClassName)
                            c.PropertyName = "_" + c.PropertyName;
                    }
                }

                return result;
            }



        }

        protected string ConnectionString { get; set; }

        protected string ProviderName { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffin.Data.BasicLayer.Paging
{
    /// <summary>
    /// Helper for adding paging to a SQL statement
    /// </summary>
    public class SqlServerPager
    {
        /// <summary>
        /// Page
        /// </summary>
        /// <param name="originalSql">Original SQL statement</param>
        /// <param name="pageNumber">Page to get</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="pkColumn">The primary key column</param>
        /// <returns>Paged SQL query</returns>
        public string ApplyTo(string originalSql, int pageNumber, int pageSize, string pkColumn)
        {
            const string template = @"WITH PagedEntities As 
(
    SELECT ROW_NUMBER() OVER (ORDER BY {{OrderByColumns}}) As Row,
        {{PrimaryKey}}
    FROM {{TableName}}
)
{{OriginalSql}}
WHERE (c.Row Between {{FirstEntry}} AND {{LastEntry}})
{{OrignalWhere}}
";

            var orderByPos = originalSql.IndexOf("ORDER BY", StringComparison.OrdinalIgnoreCase);
            var orderBy = "";
            orderBy = orderByPos == -1 ? pkColumn : originalSql.Substring(orderByPos + 9);
            var lastPartPos = orderByPos;

             var groupByPos = originalSql.IndexOf("GROUP BY", StringComparison.OrdinalIgnoreCase);
            if (groupByPos != -1)
                lastPartPos = groupByPos;

            var wherePos = originalSql.IndexOf("WHERE", StringComparison.OrdinalIgnoreCase);

            var fromPos = originalSql.IndexOf("FROM", StringComparison.OrdinalIgnoreCase);
            var fromEndPos = wherePos != -1 ? wherePos : lastPartPos;
            var tables = originalSql.Substring(fromPos, fromEndPos - fromPos);
            

            string where = "";
            if (wherePos != -1)
            {
                var startPos = wherePos + 6;
                int endPos;
               
                if (groupByPos != -1)
                {
                    endPos = groupByPos;
                }
                else if (orderByPos != -1)
                {
                    endPos = orderByPos;
                }
                else
                    endPos = originalSql.Length - 1;

                where = originalSql.Substring(startPos, endPos - startPos);
            }

            var firstRow = (pageNumber - 1)*pageSize;

            var sql = template.Replace("{{PrimaryKey}}", pkColumn)
                          .Replace("{{OrderByColumns}}", orderBy)
                          .Replace("{{FirstEntry}}", firstRow.ToString())
                          .Replace("{{LastEntry}}", (firstRow + pageSize).ToString())
                          .Replace("{{OrignalWhere}}", " AND (" + where + ")")
                          .Replace("{{TableName}}", tables)
                      + originalSql.Substring(lastPartPos);

            return sql;


        }
    }
}

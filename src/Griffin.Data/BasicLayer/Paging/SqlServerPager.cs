using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffin.Data.BasicLayer.Paging
{
    public class SqlServerPagerContext : DbPagerContext
    {
        public string PkColumn { get; set; }

        public SqlServerPagerContext(string sql, int pageNumber, int pageSize, string pkColumn)
            : base(sql, pageNumber, pageSize)
        {
            PkColumn = pkColumn;
        }
    }
    /// <summary>
    /// Helper for adding paging to a SQL statement
    /// </summary>
    public class SqlServerPager : IDbPager
    {
        /// <summary>
        /// Page
        /// </summary>
        /// <param name="originalSql">Original SQL statement</param>
        /// <param name="pageNumber">Page to get</param>
        /// <param name="pageSize">Number of items per page</param>
        /// <param name="pkColumn">The primary key column</param>
        /// <returns>Paged SQL query</returns>
        public string ApplyTo(DbPagerContext context)
        {
            var sqlServerContext = (SqlServerPagerContext) context;
            var fromIndex = context.Sql.IndexOf("FROM", StringComparison.OrdinalIgnoreCase);
            var whereIndex = context.Sql.IndexOf("WHERE", fromIndex, StringComparison.OrdinalIgnoreCase);
            var groupByIndex = context.Sql.IndexOf("GROUP BY", StringComparison.OrdinalIgnoreCase);
            var orderByIndex = context.Sql.IndexOf("ORDER BY", StringComparison.OrdinalIgnoreCase);
            
            var selectColumns = context.Sql.Substring(7, fromIndex - 7).TrimEnd();

            string tables = "";
            if (fromIndex == -1)
                throw new InvalidOperationException("Failed to find a FROM clause");
            else
            {
                var startPos = fromIndex + 5;
                var endPos = whereIndex != -1
                    ? whereIndex
                    :groupByIndex != -1
                             ? groupByIndex
                             : orderByIndex != -1
                                   ? orderByIndex
                                   : context.Sql.Length;
                tables = context.Sql.Substring(startPos, endPos - startPos).TrimEnd();
            }

            // where
            var where = "";
            if (whereIndex != -1)
            {
                var startPos = whereIndex + 6;
                var endPos = groupByIndex != -1
                             ? groupByIndex
                             : orderByIndex != -1
                                   ? orderByIndex
                                   : context.Sql.Length;

                where = context.Sql.Substring(startPos, endPos - startPos).TrimEnd();
            }

            // group by
            var groupByColumns = "";
            if (groupByIndex != -1)
            {
                var startPos = groupByIndex + 9;
                var endPos = orderByIndex != -1
                                   ? orderByIndex
                                   : context.Sql.Length;

                groupByColumns = context.Sql.Substring(startPos, endPos - startPos).TrimEnd();
            }

            // order by
            var orderByColumns = "";
            if (orderByIndex == -1)
                orderByColumns = sqlServerContext.PkColumn;
            else
            {
                var startPos = orderByIndex + 9;
                var endPos = context.Sql.Length;
                orderByColumns = context.Sql.Substring(startPos, endPos - startPos);
            }



            var firstRow = (context.PageNumber - 1)*context.PageSize;
            var lastRow = firstRow + context.PageSize;

            var innerFrom = tables;
            if (!string.IsNullOrEmpty(where))
                innerFrom += "\r\n    WHERE " + where + "\r\n";

            /*WITH Members  AS
(
SELECT	M_NAME, M_POSTS, M_LASTPOSTDATE, M_LASTHEREDATE, M_DATE, M_COUNTRY,
ROW_NUMBER() OVER (ORDER BY M_POSTS DESC) AS RowNumber
FROM	dbo.FORUM_MEMBERS
)
SELECT	RowNumber, M_NAME, M_POSTS, M_LASTPOSTDATE, M_LASTHEREDATE, M_DATE, M_COUNTRY
FROM	Members
WHERE	RowNumber BETWEEN 1 AND 20
ORDER BY RowNumber ASC;
*/
            var sql = string.Format(@"WITH Paged AS 
(
    SELECT {0},
    ROW_NUMBER() OVER (ORDER BY {1}) AS RowNumber
    FROM {2}
)
SELECT RowNumber, {0}
FROM Paged
WHERE RowNumber BETWEEN {3} AND {4}
", selectColumns, orderByColumns, innerFrom, firstRow, lastRow);

            if (groupByColumns != "")
                sql += "GROUP BY " + groupByColumns + "\r\n";
            sql += "ORDER BY " + orderByColumns;
            return sql;


        }
    }
}

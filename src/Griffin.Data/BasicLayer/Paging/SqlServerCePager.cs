using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffin.Data.BasicLayer.Paging
{
    public class SqlServerCePager : IDbPager
    {
        public string ApplyTo(DbPagerContext context)
        {
            if (context == null) throw new ArgumentNullException("context");
            /*SELECT 
   * 
FROM Orders 
ORDER BY OrderID 
OFFSET 20 ROWS 
FETCH NEXT 10 ROWS ONLY;*/

            return string.Format("{0} OFFSET {1} ROWS FETCH NEXT {2} ROWS ONLY", context.Sql,
                                 ((context.PageNumber - 1)*context.PageSize), context.PageSize);
        }
    }
}

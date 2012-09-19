using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Griffin.Data.BasicLayer.Paging
{
    public interface IDbPager
    {
        string ApplyTo(DbPagerContext context);
    }

    public class DbPagerContext
    {
        public DbPagerContext(string sql, int pageNumber, int pageSize)
        {
            PageSize = pageSize;
            PageNumber = pageNumber;
            Sql = sql;
        }

        public string Sql { get; private set; }
        public int PageNumber { get; private set; }
        public int PageSize { get; private set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Griffin.Data.BasicLayer.Paging;
using Xunit;

namespace Griffin.Data.Tests.BasicLayer.Paging
{
    public class SqlServerPagerTests
    {
        [Fact]
        public void OnlySelect()
        {
            var sql = "SELECT * FROM Users";
            var pager = new SqlServerPager();

            var actual = pager.ApplyTo(new SqlServerPagerContext(sql, 1, 50, "id"));
            var expected = @"WITH Paged AS 
(
    SELECT *,
    ROW_NUMBER() OVER (ORDER BY id) AS RowNumber
    FROM Users
)
SELECT RowNumber, *
FROM Users
WHERE RowNumber BETWEEN 0 AND 50
ORDER BY id";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Where()
        {
            var sql = "SELECT * FROM Users WHERE a = b OR c = d";
            var pager = new SqlServerPager();

            var actual = pager.ApplyTo(new SqlServerPagerContext(sql, 1, 50, "id"));
            var expected = @"WITH Paged AS 
(
    SELECT *,
    ROW_NUMBER() OVER (ORDER BY id) AS RowNumber
    FROM Users
)
SELECT RowNumber, *
FROM Users
WHERE RowNumber BETWEEN 0 AND 50
AND (a = b OR c = d)
ORDER BY id";

            Console.WriteLine(actual.Substring(99));
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void OnlyOrderBy()
        {
            var sql = "SELECT * FROM Users ORDER BY FirstName, LastName";
            var pager = new SqlServerPager();

            var actual = pager.ApplyTo(new SqlServerPagerContext(sql, 1, 50, "id"));
            var expected = @"WITH Paged AS 
(
    SELECT *,
    ROW_NUMBER() OVER (ORDER BY FirstName, LastName) AS RowNumber
    FROM Users
)
SELECT RowNumber, *
FROM Users
WHERE RowNumber BETWEEN 0 AND 50
ORDER BY FirstName, LastName";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void WhereOrderBy()
        {
            var sql = "SELECT * FROM Users WHERE Abc = 123 ORDER BY FirstName, LastName";
            var pager = new SqlServerPager();

            var actual = pager.ApplyTo(new SqlServerPagerContext(sql, 1, 50, "id"));
            var expected = @"WITH Paged AS 
(
    SELECT *,
    ROW_NUMBER() OVER (ORDER BY FirstName, LastName) AS RowNumber
    FROM Users
)
SELECT RowNumber, *
FROM Users
WHERE RowNumber BETWEEN 0 AND 50
AND (Abc = 123)
ORDER BY FirstName, LastName";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void GroupOrderBy()
        {
            var sql = "SELECT * FROM Users GROUP BY Ida ORDER BY FirstName, LastName";
            var pager = new SqlServerPager();

            var actual = pager.ApplyTo(new SqlServerPagerContext(sql, 1, 50, "id"));
            var expected = @"WITH Paged AS 
(
    SELECT *,
    ROW_NUMBER() OVER (ORDER BY FirstName, LastName) AS RowNumber
    FROM Users
)
SELECT RowNumber, *
FROM Users
WHERE RowNumber BETWEEN 0 AND 50
GROUP BY Ida
ORDER BY FirstName, LastName";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void WhereGroupOrderBy()
        {
            var sql = "SELECT * FROM Users WHERE 1 = 2 OR 2 = 3 GROUP BY Ida ORDER BY FirstName, LastName";
            var pager = new SqlServerPager();

            var actual = pager.ApplyTo(new SqlServerPagerContext(sql, 1, 50, "id"));
            var expected = @"WITH Paged AS 
(
    SELECT *,
    ROW_NUMBER() OVER (ORDER BY FirstName, LastName) AS RowNumber
    FROM Users
)
SELECT RowNumber, *
FROM Users
WHERE RowNumber BETWEEN 0 AND 50
AND (1 = 2 OR 2 = 3)
GROUP BY Ida
ORDER BY FirstName, LastName";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void CompleteWithNewLines()
        {
            var sql = @"SELECT *
FROM Users 
WHERE 1 = 2 OR 2 = 3 
GROUP BY Ida 
ORDER BY FirstName, LastName";
            var pager = new SqlServerPager();

            var actual = pager.ApplyTo(new SqlServerPagerContext(sql, 1, 50, "id"));
            var expected = @"WITH Paged AS 
(
    SELECT *,
    ROW_NUMBER() OVER (ORDER BY FirstName, LastName) AS RowNumber
    FROM Users
)
SELECT RowNumber, *
FROM Users
WHERE RowNumber BETWEEN 0 AND 50
AND (1 = 2 OR 2 = 3)
GROUP BY Ida
ORDER BY FirstName, LastName";

            Assert.Equal(expected, actual);
        }

    }
}

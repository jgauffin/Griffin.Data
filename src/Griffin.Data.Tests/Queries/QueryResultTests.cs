using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Griffin.Data.Queries;
using Xunit;

namespace Griffin.Data.Tests.Queries
{
    public class QueryResultTests
    {
        [Fact]
        public void CorrectlyInitialized()
        {
            var qr = new QueryResult<User>(new User[] {new User() {FirstName = "Arne"}}, 100);

            Assert.Equal("Arne", qr.Items.First().FirstName);
            Assert.Equal(100, qr.TotalCount);
        }

        [Fact]
        public void InvalidTotalCount()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new QueryResult<User>(new User[] { new User() { FirstName = "Arne" } }, -1));
        }

        [Fact]
        public void NullList()
        {
            Assert.Throws<ArgumentNullException>(() => new QueryResult<User>(null, 0));
        }
    }
}

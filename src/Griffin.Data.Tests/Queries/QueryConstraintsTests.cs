using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Griffin.Data.Queries;
using Xunit;

namespace Griffin.Data.Tests.Queries
{
    public class QueryConstraintsTests
    {
        [Fact]
        public void NonExistantProperty()
        {
            var constraints = new QueryConstraints<User>();
            Assert.Throws<ArgumentOutOfRangeException>(() => constraints.SortBy("Arne"));
        }

        [Fact]
        public void SortBy()
        {
            var constraints = new QueryConstraints<User>();

            constraints.SortBy("FirstName");

            Assert.Equal(SortOrder.Ascending, constraints.SortOrder);
            Assert.Equal("FirstName", constraints.SortPropertyName);
        }

        [Fact]
        public void SortByDescending()
        {
            var constraints = new QueryConstraints<User>();

            constraints.SortByDescending("FirstName");

            Assert.Equal(SortOrder.Descending, constraints.SortOrder);
            Assert.Equal("FirstName", constraints.SortPropertyName);
        }

        [Fact]
        public void NoPaging()
        {
            var constraints = new QueryConstraints<User>();

            Assert.Equal(-1, constraints.PageNumber);
            Assert.Equal(-1, constraints.PageSize);
        }

        [Fact]
        public void FirstPage()
        {
            var constraints = new QueryConstraints<User>();

            constraints.Page(1, 50);

            Assert.Equal(1, constraints.PageNumber);
            Assert.Equal(50, constraints.PageSize);
            Assert.Equal(0, constraints.StartRecord);
        }


        [Fact]
        public void TenthPage()
        {
            var constraints = new QueryConstraints<User>();

            constraints.Page(10, 20);

            Assert.Equal(10, constraints.PageNumber);
            Assert.Equal(20, constraints.PageSize);
            Assert.Equal((10 - 1)*20, constraints.StartRecord);
        }

        [Fact]
        public void TypedSortBy()
        {
            var constraints = new QueryConstraints<User>();

            constraints.SortBy(x => x.FirstName);

            Assert.Equal(SortOrder.Ascending, constraints.SortOrder);
            Assert.Equal("FirstName", constraints.SortPropertyName);
        }

        [Fact]
        public void TypedSortByDescending()
        {
            var constraints = new QueryConstraints<User>();

            constraints.SortByDescending(x => x.FirstName);

            Assert.Equal(SortOrder.Descending, constraints.SortOrder);
            Assert.Equal("FirstName", constraints.SortPropertyName);
        }
    }
}

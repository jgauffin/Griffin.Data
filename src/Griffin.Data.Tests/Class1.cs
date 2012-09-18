using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Griffin.Data.Mappings;
using Griffin.Data.Queries;
using NSubstitute;
using Xunit;

namespace Griffin.Data.Tests
{
    public class UserQueries
    {
        public IQueryResult<User> RegisteredButNotLoggedIn(QueryConstraints<User> constraints)
        {
            return new QueryResult<User>(new LinkedList<User>(), 0);
        }
    }

    public class IdDisplayName
    {
        public string Id { get; private set; }
        public string DisplayTitle { get; private set; }
    }

}

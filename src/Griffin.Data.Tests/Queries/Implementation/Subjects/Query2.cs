using Griffin.Data.Queries;

namespace Griffin.Data.Tests.Queries.Implementation.Subjects;

public class Query2 : IQuery<Query2Result>
{
}

public class Query2Result
{
}

public class Query2Handler : IQueryHandler<Query2, Query2Result>
{
    private readonly Session _session;

    public Query2Handler(Session session)
    {
        _session = session;
    }

    public Task<Query2Result> Execute(Query2 query)
    {
        return Task.FromResult(new Query2Result());
    }
}

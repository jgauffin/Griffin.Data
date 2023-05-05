using Griffin.Data.Queries;

namespace Griffin.Data.Tests.Queries.Implementation.Subjects;

public class MyQueryHandler : IQueryHandler<MyQuery, MyQueryResult>
{
    public MyQueryResult? Result = new();
    
    public Task<MyQueryResult> Execute(MyQuery query)
    {
        return Task.FromResult(Result)!;
    }
}

using System.Data;
using Griffin.Data;
using Griffin.Data.Helpers;
using Griffin.Data.Queries;

namespace TestApp.Users.Queries;

public class ListUsersHandler : IQueryHandler<ListUsers, ListUsersResult>
{
    private readonly Session _session;

    public ListUsersHandler(Session session)
    {
        _session = session;
    }

    public async Task<ListUsersResult> Execute(ListUsers query)
    {
        await using var command = _session.CreateCommand();
        command.CommandText = @"select *
                                 from Users";

        command.AddParameter("name", query.NameToFind);
        var items = await command.GenerateQueryResult<ListUsersResultItem>(MapRecord);
        return new ListUsersResult { Items = items };
    }

    protected void MapRecord(IDataRecord record, ListUsersResultItem item)
    {
        item.Id = record.GetInt32(0);
        item.Name = record.GetString(1);
        item.Age = record.GetInt16(2);
        item.Money = record.GetInt64(3);
        item.Rocks = record.GetBoolean(4);
    }
}

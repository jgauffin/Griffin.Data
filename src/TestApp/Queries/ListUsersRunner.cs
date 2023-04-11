using System.Data;
using Griffin.Data;
using Griffin.Data.Helpers;
using Griffin.Data.Queries;

public class ListUsersRunner :  ListRunner<ListUsersResultItem>, IQueryRunner<ListUsers, ListUsersResult>
{
    public ListUsersRunner(Session session) : base(session)
    {
    }

    public async Task<ListUsersResult> Execute(ListUsers query)
    {
        await using var command = Session.CreateCommand();
        command.CommandText = @"select *
                                 from maintable
                                 where name=@name;        ";

        command.AddParameter("name", query.Name);
        if (query.PageNumber != null)
        {
            Session.Dialect.ApplyPaging(command, "Id", query.PageNumber.Value, query.PageSize);
        }

        if (query.SortEntries.Any())
        {
            Session.Dialect.ApplySorting(command, query.SortEntries);
        }

        return new ListUsersResult { Items = await MapRecords(command) };
    }
    protected override void MapRecord(IDataRecord record, ListUsersResultItem item)
    {
        item.Id = record.GetInt32(0);
        item.Name = record.GetString(1);
        item.Age = record.GetInt16(2);
        item.Money = record.GetInt64(3);
        item.Rocks = record.GetBoolean(4);
    }
}

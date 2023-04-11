using Griffin.Data.Queries;

public class ListUsers : IQuery<ListUsersResult>
{
    public string Name { get; set; }
}

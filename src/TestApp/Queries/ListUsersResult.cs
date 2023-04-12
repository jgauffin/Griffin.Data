namespace TestApp.Queries;

/// <summary>
/// Query example.
/// </summary>
public class ListUsersResult
{
    public IReadOnlyList<ListUsersResultItem> Items { get; set; } = new List<ListUsersResultItem>();
}

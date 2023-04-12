using Griffin.Data.Queries;

namespace TestApp.Queries;

public class ListUsers : IQuery<ListUsersResult>, IPagedQuery, ISortedQuery
{
    /// <summary>
    /// 
    /// </summary>
    public string Name { get; set; }
    /// <summary>
    /// One-based page number.
    /// </summary>
    public int? PageNumber { get; set; }
    /// <summary>
    /// Number of items per page.
    /// </summary>
    public int? PageSize { get; set; }
    /// <summary>
    /// Sort result using property names
    /// </summary>
    public IList<SortEntry> SortEntries { get; set; } = new List<SortEntry>();
}

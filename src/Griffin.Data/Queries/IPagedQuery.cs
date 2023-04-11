namespace Griffin.Data.Queries;

/// <summary>
///     Enable paging.
/// </summary>
public interface IPagedQuery
{
    /// <summary>
    ///     One based index of pages.
    /// </summary>
    int? PageNumber { get; set; }

    /// <summary>
    ///     Number of entries per page.
    /// </summary>
    int? PageSize { get; set; }
}

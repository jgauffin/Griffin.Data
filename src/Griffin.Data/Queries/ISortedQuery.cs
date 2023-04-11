using System.Collections.Generic;

namespace Griffin.Data.Queries;

/// <summary>
///     Will apply sorting to queries.
/// </summary>
public interface ISortedQuery
{
    /// <summary>
    ///     All sort entries.
    /// </summary>
    IList<SortEntry> SortEntries { get; set; }
}

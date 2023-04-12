using System;

namespace Griffin.Data.Queries;

/// <summary>
///     Sort entry for <see cref="ISortedQuery" />
/// </summary>
public class SortEntry
{
    /// <summary>
    /// </summary>
    /// <param name="name">Property name to sort by.</param>
    /// <param name="isAscending">Use ascending sort</param>
    public SortEntry(string name, bool isAscending)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        IsAscending = isAscending;
    }

    /// <summary>
    ///     Use ascending sort
    /// </summary>
    public bool IsAscending { get; }

    /// <summary>
    ///     Property to sort by.
    /// </summary>
    public string Name { get; set; }
}

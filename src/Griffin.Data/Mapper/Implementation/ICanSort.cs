using System.Collections.Generic;
using Griffin.Data.Dialects;

namespace Griffin.Data.Mapper.Implementation;

/// <summary>
///     Used to be able to hide the list for a cleaner API while giving <see cref="ISqlDialect" /> implementations a chance
///     to retrieve them from <see cref="QueryOptions" />.
/// </summary>
public interface ICanSort
{
    /// <summary>
    ///     Configured sort columns.
    /// </summary>
    IReadOnlyList<SortInstruction> Sorts { get; }
}

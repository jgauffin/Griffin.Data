using System;

namespace Griffin.Data.Mapper.Implementation;

/// <summary>
///     Sort instruction for <see cref="QueryOptions" />
/// </summary>
/// <remarks>
///     <para>
///         Used in DB engine implementations to be able to create correct sorting statements
///     </para>
/// </remarks>
public class SortInstruction
{
    /// <summary>
    /// </summary>
    /// <param name="name">Property or column name depending on <see cref="IsPropertyName" />.</param>
    /// <param name="isAscending">Sorting on this column is ascending.</param>
    /// <param name="isPropertyName">Specified name is a property name (column name is used when this property is false).</param>
    public SortInstruction(string name, bool isAscending, bool isPropertyName)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        IsAscending = isAscending;
        IsPropertyName = isPropertyName;
    }

    /// <summary>
    ///     Sorting on this column is ascending.
    /// </summary>
    public bool IsAscending { get; set; }

    /// <summary>
    ///     Specified name is a property name (column name is used when this property is false).
    /// </summary>
    public bool IsPropertyName { get; }

    /// <summary>
    ///     Property or column name depending on <see cref="IsPropertyName" />.
    /// </summary>
    public string Name { get; set; }
}

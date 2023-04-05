using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Griffin.Data.Mappings.Properties;

namespace Griffin.Data.Mappings.Relations;

/// <summary>
///     Defines a one to zero or more relationship.
/// </summary>
public interface IHasManyMapping : IFieldAccessor, IRelationShip
{
    /// <summary>
    ///     Create a new (generic) collection for the child property.
    /// </summary>
    /// <returns>Created collection.</returns>
    IList CreateCollection();

    /// <summary>
    /// Used to limit returned rows.
    /// </summary>
    public KeyValuePair<string, string>? SubsetColumn { get; set; }

    /// <summary>
    ///     Visit all items in the collection.
    /// </summary>
    /// <param name="collection">Collection to iterate.</param>
    /// <param name="visitor">Callback to invoke once per element.</param>
    /// <returns></returns>
    Task Visit(object collection, Func<object, Task> visitor);
}
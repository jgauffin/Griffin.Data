using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Griffin.Data.Mappings.Relations;

/// <summary>
///     Defines a one to zero or more relationship.
/// </summary>
public interface IHasManyMapping : IRelationShip
{
    /// <summary>
    ///     Used to limit returned rows.
    /// </summary>
    public KeyValuePair<string, string>? SubsetColumn { get; set; }

    /// <summary>
    ///     Create a new (generic) collection for the child property.
    /// </summary>
    /// <returns>Created collection.</returns>
    IList CreateCollection();

    /// <summary>
    /// Get collection from parent property.
    /// </summary>
    /// <param name="parentEntity">Parent entity</param>
    /// <returns>Collection instance if there is one assigned to the property; otherwise <c>null</c>.</returns>
    IList? GetCollection(object parentEntity);

    /// <summary>
    ///     Assign a collection to the property that this has-many mapping is for.
    /// </summary>
    /// <param name="parentEntity"></param>
    /// <param name="collection"></param>
    void SetCollection(object parentEntity, IList collection);

    /// <summary>
    ///     Visit all items in the collection.
    /// </summary>
    /// <param name="collection">Collection to iterate.</param>
    /// <param name="visitor">Callback to invoke once per element.</param>
    /// <returns></returns>
    Task Visit(object collection, Func<object, Task> visitor);
}

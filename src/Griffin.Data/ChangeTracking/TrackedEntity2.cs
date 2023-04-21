using System;
using System.Collections.Generic;

namespace Griffin.Data.ChangeTracking;

/// <summary>
///     Entity being tracked (for snapshot comparison).
/// </summary>
public class TrackedEntity2
{
    private readonly List<TrackedEntity2> _children = new();

    /// <summary>
    /// </summary>
    /// <param name="key">Generated key (consists of type name and all primary key values)</param>
    /// <param name="entity">Entity to track.</param>
    /// <param name="depth">Depth in hierarchy (1 = root entity).</param>
    public TrackedEntity2(string? key, object entity, int depth)
    {
        Key = key;
        Entity = entity;
        Depth = depth;
    }

    /// <summary>
    ///     Get all children.
    /// </summary>
    public IReadOnlyList<TrackedEntity2> Children => _children;

    /// <summary>
    ///     Required so that inserts and deletes can be done in the correct order.
    /// </summary>
    public int Depth { get; }

    /// <summary>
    ///     Entity being tracked.
    /// </summary>
    public object Entity { get; }

    /// <summary>
    ///     Key string (generated from all keys and the type name).
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Only set for entities that exists in the database.
    ///     </para>
    /// </remarks>
    public string? Key { get; }

    /// <summary>
    ///     Parent entity.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Required so that we can assign the FK value once the parent entity has been INSERTed into the database.
    ///     </para>
    /// </remarks>
    /// <value><c>null</c> for root entities.</value>
    public TrackedEntity2? Parent { get; private set; }

    /// <summary>
    ///     Add a new child.
    /// </summary>
    /// <param name="child">Child entity.</param>
    /// <exception cref="ArgumentNullException">child is not specified.</exception>
    public void AddChild(TrackedEntity2 child)
    {
        if (child == null)
        {
            throw new ArgumentNullException(nameof(child));
        }

        child.Parent = this;
        _children.Add(child);
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{Key} Depth {Depth} {Entity}";
    }
}

using System;
using System.Collections.Generic;

namespace Griffin.Data.ChangeTracking;

public class TrackedEntity2
{
    private readonly List<TrackedEntity2> _children = new();

    public TrackedEntity2(string? key, object entity, int depth)
    {
        Key = key;
        Entity = entity;
        Depth = depth;
    }

    public IReadOnlyList<TrackedEntity2> Children => _children;

    /// <summary>
    ///     Required so that inserts and deletes can be done in the correct order.
    /// </summary>
    public int Depth { get; }

    /// <summary>
    /// Entity being tracked.
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
    public object Parent { get; private set; }

    public void AddChild(TrackedEntity2 child)
    {
        if (child == null)
        {
            throw new ArgumentNullException(nameof(child));
        }

        child.Parent = Entity;
        _children.Add(child);
    }

    public override string ToString()
    {
        return $"{Key} Depth {Depth} {Entity}";
    }
}

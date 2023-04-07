using System;

namespace Griffin.Data.ChangeTracking;

internal class TrackedEntity
{
    public TrackedEntity(object? snapshot, object current, object? parent, int depth)
    {
        Snapshot = snapshot;
        Current = current ?? throw new ArgumentNullException(nameof(current));
        Parent = parent;
        Depth = depth;
    }

    public object Current { get; set; }

    /// <summary>
    ///     One-based index of hierarchy depth (1 = root entity).
    /// </summary>
    public int Depth { get; }

    /// <summary>
    ///     Parent is null for root entities.
    /// </summary>
    public object? Parent { get; set; }

    /// <summary>
    ///     snapshot is null when the entity was added.
    /// </summary>
    public object? Snapshot { get; set; }

    /// <summary>
    ///     State when the last diff was made.
    /// </summary>
    public ChangeState State { get; set; } = ChangeState.Unmodified;
}

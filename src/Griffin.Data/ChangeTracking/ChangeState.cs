namespace Griffin.Data.ChangeTracking;

/// <summary>
///     Current state of an entity.
/// </summary>
public enum ChangeState
{
    /// <summary>
    ///     No changes have been made to the entity.
    /// </summary>
    Unmodified,

    /// <summary>
    ///     Entity was added during this session.
    /// </summary>
    Added,

    /// <summary>
    ///     Entity was modified (the entity itself, child entities do not affect this state).
    /// </summary>
    Modified,

    /// <summary>
    ///     Entity was removed.
    /// </summary>
    Removed
}

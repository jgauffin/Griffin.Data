namespace Griffin.Data.ChangeTracking.Services;

/// <summary>
/// Keeps track of all differences in two copies of the same entity.
/// </summary>
public interface IDiff
{
    /// <summary>
    /// A child entity has been added.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="entity"></param>
    /// <param name="depth"></param>
    void Added(object parent, object entity, int depth);

    /// <summary>
    /// A child entity has been modified.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="entity"></param>
    /// <param name="depth"></param>
    void Modified(object parent, object entity, int depth);

    /// <summary>
    /// A child entity has been removed.
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="snapshot">Only snapshot is available when a root has been removed.</param>
    /// <param name="depth"></param>
    void Removed(object parent, object snapshot, int depth);
}

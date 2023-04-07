namespace Griffin.Data.ChangeTracking.Services;

public interface IDiff
{
    void Added(object parent, object entity, int depth);
    void Modified(object parent, object entity, int depth);

    /// <summary>
    /// </summary>
    /// <param name="parent"></param>
    /// <param name="snapshot">Only snapshot is available when a root has been removed.</param>
    /// <param name="depth"></param>
    void Removed(object parent, object snapshot, int depth);
}

namespace Griffin.Data;

/// <summary>
/// Tracks all loaded entities in a session.
/// </summary>
public interface IEntityTracker
{
    /// <summary>
    /// Track new item.
    /// </summary>
    /// <param name="item">Item to track.</param>
    void Track(object item);
}
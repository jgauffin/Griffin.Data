using System;
using System.Threading.Tasks;

namespace Griffin.Data.ChangeTracking;

/// <summary>
///     Tracks all loaded entities in a session.
/// </summary>
/// <remarks>
///     <para>
///         There is an important limitation that must be taken into account. Entities which are fetched multiple times in
///         the same session are treated as different entities. Thus the will overwrite each others changes. Keep sessions
///         small and try to not fetch the same entity multiple times (or change just one of them).
///     </para>
/// </remarks>
public interface IChangeTracker
{
    /// <summary>
    ///     Apply changes (i.e. invoke INSERT/UPDATE/DELETE).
    /// </summary>
    /// <param name="session">Session to apply changes in.</param>
    Task ApplyChanges(Session session);

    /// <summary>
    ///     Calculate delta and apply changes without registering changes in the internal cache.
    /// </summary>
    /// <param name="session">Session to update the database with.</param>
    /// <param name="current">Custom loaded entity (with changes).</param>
    /// <returns>Task</returns>
    /// <remarks>
    ///     <para>
    ///         The tracker must contain a snapshot for this to work.
    ///     </para>
    /// </remarks>
    Task ApplyIsolated(Session session, object current);

    /// <summary>
    ///     Get state of an object.
    /// </summary>
    /// <param name="entity">Entity to get state for.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    /// <remarks>
    ///     <para>
    ///         If you are fetching a child entity, make sure that you refresh the root entity first.
    ///     </para>
    /// </remarks>
    public ChangeState GetState(object entity);

    /// <summary>
    ///     Refresh state for a entity and all of its children.
    /// </summary>
    /// <param name="entity">Entity to refresh state for.</param>
    void Refresh(object entity);

    /// <summary>
    ///     Track new item.
    /// </summary>
    /// <param name="item">Item to track.</param>
    /// <remarks>
    ///     <para>
    ///         Tracked entities must be root entities. Children should not be added manually.
    ///     </para>
    /// </remarks>
    void Track(object item);
}

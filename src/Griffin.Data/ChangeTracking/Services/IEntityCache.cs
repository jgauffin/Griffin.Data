using System.Collections.Generic;

namespace Griffin.Data.ChangeTracking.Services;

/// <summary>
///     Stores all loaded entities and their snapshots.
/// </summary>
/// <remarks>
///     <para>
///         The purpose is to be able tro generate diffs once <see cref="IEntityCache" /> wants to save (or the user wants
///         to check state of an entity).
///     </para>
/// </remarks>
internal interface IEntityCache
{
    void Insert(TrackedEntity trackedEntity);
    void Append(string key, TrackedEntity entity);

    IEntityCache Clone();
    IReadOnlyList<TrackedEntity> GetDepth(int depth);
    IReadOnlyList<TrackedEntity> ListForState(ChangeState state);
    void MarkAsModified(object entity);
    void MarkAsRemoved(object entity);
    bool TryFind(object entity, out TrackedEntity? trackedEntity);
}

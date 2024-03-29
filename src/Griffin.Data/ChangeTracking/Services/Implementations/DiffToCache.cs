﻿namespace Griffin.Data.ChangeTracking.Services.Implementations;

/// <summary>
///     Diff that updates the snapshot change with the changes.
/// </summary>
internal class DiffToCache : IDiff
{
    private readonly IEntityCache _cache;

    public DiffToCache(IEntityCache cache)
    {
        _cache = cache;
    }

    /// <inheritdoc />
    public void Added(object parent, object current, int depth)
    {
        var tracked = new TrackedEntity(null, current, parent, depth) { State = ChangeState.Added };
        _cache.Insert(tracked);
    }

    /// <inheritdoc />
    public void Modified(object parent, object entity, int depth)
    {
        _cache.MarkAsModified(entity);
    }

    /// <inheritdoc />
    public void Removed(object parent, object entity, int depth)
    {
        _cache.MarkAsRemoved(entity);
    }
}

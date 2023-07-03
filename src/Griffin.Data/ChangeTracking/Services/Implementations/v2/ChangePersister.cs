using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Griffin.Data.Mapper;
using Griffin.Data.Mapper.Mappings;
using Griffin.Data.Mapper.Mappings.Relations;

namespace Griffin.Data.ChangeTracking.Services.Implementations.v2;

/// <summary>
///     Persists changes from a compare result.
/// </summary>
/// <seealso cref="SingleEntityComparer" />
public class ChangePersister
{
    private readonly IMappingRegistry _mappingRegistry;

    /// <summary>
    /// </summary>
    /// <param name="mappingRegistry">Used to find mappings to assign foreign keys.</param>
    /// <exception cref="ArgumentNullException">Arguments are not specified.</exception>
    public ChangePersister(IMappingRegistry mappingRegistry)
    {
        _mappingRegistry = mappingRegistry ?? throw new ArgumentNullException(nameof(mappingRegistry));
    }

    /// <summary>
    ///     Persist changes.
    /// </summary>
    /// <param name="session">Session to perform CRUD operations in.</param>
    /// <param name="items">Items to persist (unmodified items will be ignored).</param>
    /// <returns>Task.</returns>
    /// <exception cref="MappingException"></exception>
    public async Task Persist(Session session, IReadOnlyList<CompareResultItem> items)
    {
        if (session == null)
        {
            throw new ArgumentNullException(nameof(session));
        }

        if (items == null)
        {
            throw new ArgumentNullException(nameof(items));
        }

        var itemsToInsert = items.Where(x => x.State == ChangeState.Added).OrderBy(x => x.Depth);
        foreach (var item in itemsToInsert)
        {
            AssignForeignKeyIfDefined(item);
            var subsetColumn = GetSubsetColumn(item);

            // We must use flat inserts since the change persister will
            // split hierarchies into stand alone objects.
            await session.InsertEntity(item.TrackedItem.Entity, subsetColumn);
        }

        var itemsToRemove = items.Where(x => x.State == ChangeState.Removed).OrderByDescending(x => x.Depth);
        foreach (var item in itemsToRemove)
        {
            if (UpdateForeignKeysInDeletes)
            {
                AssignForeignKeyIfDefined(item);
            }

            await session.Delete(item.TrackedItem.Entity);
        }

        var itemsToUpdate = items.Where(x => x.State == ChangeState.Modified);
        foreach (var item in itemsToUpdate)
        {
            if (UpdateForeignKeysInUpdates)
            {
                AssignForeignKeyIfDefined(item);
            }

            var subsetColumn = GetSubsetColumn(item);
            await session.Update(item.TrackedItem.Entity, subsetColumn);
        }
    }

    /// <summary>
    /// Assign foreign keys in UPDATE statements.
    /// </summary>
    /// <remarks>
    ///<para>
    ///Will assign the FK value from the parent entity when updates are made. This is normally not required, but useful when FKs are not part of DTOs in external  APIs.
    /// </para>
    /// </remarks>
    public bool UpdateForeignKeysInUpdates { get; set; }
    /// <summary>
    /// Assign foreign keys in DELETE statements.
    /// </summary>
    /// <remarks>
    ///<para>
    ///Will assign the FK value from the parent entity when deletes are made. This is normally not required, but useful when FKs are not part of DTOs in external  APIs.
    /// </para>
    /// </remarks>
    public bool UpdateForeignKeysInDeletes { get; set; }

    private void AssignForeignKeyIfDefined(CompareResultItem item)
    {
        if (item.TrackedItem.Parent != null)
        {
            var parent = item.TrackedItem.Parent.Entity;
            var child = item.TrackedItem.Entity;
            var mapping = _mappingRegistry.Get(parent.GetType());
            var relation = mapping.GetRelation(child.GetType());
            if (relation != null)
            {
                var parentId = relation.GetReferencedId(parent);
                if (parentId == null)
                {
                    throw new MappingException(parent,
                        "Failed to find referenced id for " + child);
                }

                relation.SetForeignKey(child, parentId);
            }
        }
    }

    private IDictionary<string, object>? GetSubsetColumn(CompareResultItem item)
    {
        if (item.TrackedItem.Parent == null)
        {
            return null;
        }

        var parent = item.TrackedItem.Parent.Entity;
        var child = item.TrackedItem.Entity;
        var mapping = _mappingRegistry.Get(parent.GetType());
        var relation = mapping.GetRelation(child.GetType());
        if (relation is IHasOneMapping one && one.SubsetColumn != null)
        {
            return new Dictionary<string, object>() { { one.SubsetColumn.Value.Key, one.SubsetColumn.Value.Value } };
        }

        return null;
    }
}

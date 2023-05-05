using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Griffin.Data.Mapper;
using Griffin.Data.Mapper.Mappings;

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

            await session.Insert(item.TrackedItem.Entity);
        }

        var itemsToRemove = items.Where(x => x.State == ChangeState.Removed).OrderByDescending(x => x.Depth);
        foreach (var item in itemsToRemove)
        {
            await session.Delete(item.TrackedItem.Entity);
        }

        var itemsToUpdate = items.Where(x => x.State == ChangeState.Modified);
        foreach (var item in itemsToUpdate)
        {
            await session.Update(item.TrackedItem.Entity);
        }
    }
}

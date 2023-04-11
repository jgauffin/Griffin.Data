using System;
using System.Collections.Generic;
using System.Linq;
using Griffin.Data.Mappings;

namespace Griffin.Data.ChangeTracking.Services.Implementations.v2;

/// <summary>
/// Used to compare two entities and generate a report of added/modified/removed items.
/// </summary>
public class SingleEntityComparer
{
    private readonly IMappingRegistry _mappingRegistry;

    public SingleEntityComparer(IMappingRegistry mappingRegistry)
    {
        _mappingRegistry = mappingRegistry ?? throw new ArgumentNullException(nameof(mappingRegistry));
    }

    /// <summary>
    /// Compare entities.
    /// </summary>
    /// <param name="snapshot"></param>
    /// <param name="current"></param>
    /// <returns></returns>
    public List<CompareResultItem> Compare(object snapshot, object current)
    {
        // Start by generating a structure (flat list with hierarchical entities).
        // to allow us to traverse them and generate a change report.
        var traverser = new EntityTraverser2(_mappingRegistry);
        var snapshots = traverser.Traverse(snapshot);
        var currents = traverser.Traverse(current);

        var existingCurrents = currents.Where(x => x.Key != null).ToDictionary(x => x.Key, x => x);

        var toCompare =
            new List<(TrackedEntity2 snapshot, TrackedEntity2 current)>();
        var result = new List<CompareResultItem>();
        foreach (var snapshotItem in snapshots)
        {
            if (snapshotItem.Key == null)
            {
                //TODO: A bug. Should always have a key
                // since snapshots are always fetched from the db.
                continue;
            }

            if (existingCurrents.TryGetValue(snapshotItem.Key!, out var currentItem))
            {
                toCompare.Add((snapshotItem, currentItem));
            }
            else
            {
                result.Add(new CompareResultItem(snapshotItem, ChangeState.Removed));
            }
        }

        foreach (var currentItem in currents)
        {
            if (currentItem.Key == null)
            {
                result.Add(new CompareResultItem(currentItem, ChangeState.Added));
            }
        }

        foreach (var tuple in toCompare)
        {
            var equals = EntityEquals(tuple.snapshot.Entity, tuple.current.Entity);
            result.Add(new CompareResultItem(tuple.current, equals ? ChangeState.Unmodified : ChangeState.Modified));
        }

        var orderedResult = result.OrderBy(x => x.Depth).ToList();
        foreach (var item in orderedResult)
        {
            if (item.Depth == 1)
            {
                continue;
            }

            if (item.TrackedItem.Key == null)
            {
                var parent2 = result.FirstOrDefault(x => x.TrackedItem.Entity == item.TrackedItem.Parent.Entity);
                parent2?.AppendChild(item);
                continue;
            }

            var parent = result.FirstOrDefault(x => x.TrackedItem.Key == item.TrackedItem.Parent.Key);
            parent?.AppendChild(item);
        }

        return result;
    }

    /// <summary>
    ///     Compare two entities (children are not compared).
    /// </summary>
    /// <param name="snapshot"></param>
    /// <param name="current"></param>
    private bool EntityEquals(object snapshot, object current)
    {
        var mapping = _mappingRegistry.Get(snapshot.GetType());
        foreach (var prop in mapping.Properties)
        {
            var snapShotValue = prop.GetValue(snapshot);
            var currentValue = prop.GetValue(current);
            if (snapShotValue == null && currentValue == null)
            {
                continue;
            }

            if (snapShotValue == null || currentValue == null)
            {
                return false;
            }

            if (Equals(snapShotValue, currentValue))
            {
                continue;
            }

            return false;
        }

        return true;
    }
}

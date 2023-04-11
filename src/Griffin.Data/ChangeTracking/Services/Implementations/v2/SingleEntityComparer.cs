using System.Collections.Generic;
using System.Linq;
using Griffin.Data.Mappings;

namespace Griffin.Data.ChangeTracking.Services.Implementations.v2;

public class SingleEntityComparer
{
    private IMappingRegistry _mappingRegistry;

    public SingleEntityComparer(IMappingRegistry mappingRegistry)
    {
        _mappingRegistry = mappingRegistry;
    }

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
            if (!EntityEquals(tuple.snapshot.Entity, tuple.current.Entity))
            {
                result.Add(new CompareResultItem(tuple.current, ChangeState.Modified));
            }
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

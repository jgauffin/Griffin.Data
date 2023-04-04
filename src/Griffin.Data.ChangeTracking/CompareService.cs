using System.Collections;
using Griffin.Data.Configuration;
using Griffin.Data.Helpers;
using Griffin.Data.Mapper;
using Griffin.Data.Mappings;

namespace Griffin.Data.ChangeTracking;

public class CompareService
{
    private readonly IDiff _diff;
    private readonly IMappingRegistry _registry;

    public CompareService(IMappingRegistry registry, IDiff diff)
    {
        _registry = registry;
        _diff = diff;
    }

    public void Compare(object? snapshot, object? current, ClassMapping mapping, int depth)
    {
        if (snapshot == null && current == null) return;

        if (snapshot == null || current == null)
        {
            if (current != null) _diff.Added(current, depth);

            if (snapshot != null) _diff.Removed(snapshot, depth);

            return;
        }

        if (snapshot.GetType() != current.GetType())
            throw new InvalidOperationException(
                $"Expected to compare two objects of same type. Got {snapshot.GetType()} and {current.GetType()}");

        var isEqual = true;
        foreach (var prop in mapping.Properties)
        {
            if (!prop.CanWriteToDatabase) continue;


            var snapShotValue = prop.GetColumnValue(snapshot);
            var currentValue = prop.GetColumnValue(current);
            if (snapShotValue == null && currentValue == null) continue;

            if (snapShotValue == null || currentValue == null)
            {
                isEqual = false;
                break;
            }

            if (!Equals(snapShotValue, currentValue))
            {
                isEqual = false;
                break;
            }
        }

        if (!isEqual) _diff.Modified(current, depth);

        CompareCollections(snapshot, current, mapping, depth + 1);
        CompareChildren(snapshot, current, mapping, depth + 1);
    }

    private void CompareChildren(object snapshot, object current, ClassMapping parentMapping, int depth)
    {
        foreach (var child in parentMapping.Children)
        {
            var snapshotValue = child.GetColumnValue(snapshot);
            var currentValue = child.GetColumnValue(current);

            var entityType = child.HaveDiscriminator
                ? child.GetTypeUsingDiscriminator(snapshot)
                : child.ChildEntityType;
            if (entityType == null) continue;

            var mapping = _registry.Get(entityType);
            Compare(snapshotValue, currentValue, mapping, depth + 1);
        }
    }

    private void CompareCollections(object snapshot, object current, ClassMapping parentMapping, int depth)
    {
        foreach (var collection in parentMapping.Collections)
        {
            var snapshotValue = (IEnumerable?)collection.GetColumnValue(snapshot) ??
                                (IEnumerable)Activator.CreateInstance(
                                    typeof(List<>).MakeGenericType(collection.ChildEntityType))!;
            var currentValue = (IEnumerable?)collection.GetColumnValue(current) ??
                               (IEnumerable)Activator.CreateInstance(
                                   typeof(List<>).MakeGenericType(collection.ChildEntityType))!;

            var snapshotIndex = new Dictionary<object, object>();
            var currentIndex = new Dictionary<object, object>();
            var mapping = _registry.Get(collection.ChildEntityType);
            if (mapping.Keys.Count == 0)
                throw new MappingConfigurationException(collection.ChildEntityType, "Entity does not have a key.");

            Func<object, object?> keyGetter =
                mapping.Keys.Count > 1
                    ? entity => string.Join(";", mapping.Keys.Select(x => x.GetColumnValue(entity) ?? ""))
                    : entity => mapping.Keys[0].GetColumnValue(entity);


            foreach (var item in snapshotValue)
            {
                var key = keyGetter(item);
                if (key == null) throw new MappingException(item, "Expected existing child to have an identity.");
                snapshotIndex[key] = item;
            }

            foreach (var item in currentValue)
            {
                var key = keyGetter(item);
                if (key == null)
                {
                    _diff.Added(item, depth);
                    continue;
                }

                if (key.GetType().IsSimpleType() && Activator.CreateInstance(key.GetType())!.Equals(key))
                {
                    _diff.Added(item, depth);
                    continue;
                }

                currentIndex[key] = item;
            }

            var snapshotEquals = new List<KeyValuePair<object, object>>();
            foreach (var kvp in snapshotIndex)
                if (currentIndex.ContainsKey(kvp.Key))
                    snapshotEquals.Add(kvp);
                else
                    _diff.Removed(kvp.Value, depth);

            foreach (var kvp in currentIndex)
                if (!snapshotIndex.ContainsKey(kvp.Key))
                    _diff.Added(kvp.Value, depth);

            var childMapping = _registry.Get(collection.ChildEntityType);
            foreach (var equal in snapshotEquals)
            {
                var currentChild = currentIndex[equal.Key];
                Compare(equal.Value, currentChild, childMapping, depth + 1);
            }
        }
    }
}
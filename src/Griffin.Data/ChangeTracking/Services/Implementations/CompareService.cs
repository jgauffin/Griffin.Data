using System;
using System.Collections;
using System.Collections.Generic;
using Griffin.Data.Configuration;
using Griffin.Data.Mapper;
using Griffin.Data.Mappings;

namespace Griffin.Data.ChangeTracking.Services.Implementations;

/// <summary>
///     This service takes two objects and compare them.
/// </summary>
/// <remarks>
///     <para>
///         Useful when using snap shot change tracking.
///     </para>
/// </remarks>
public class CompareService
{
    private readonly IDiff _diff;
    private readonly IMappingRegistry _registry;

    /// <summary>
    /// </summary>
    /// <param name="registry">Mapping registry (to find what to compare).</param>
    /// <param name="diff">Diff to fill with all changes.</param>
    public CompareService(IMappingRegistry registry, IDiff diff)
    {
        _registry = registry;
        _diff = diff;
    }

    /// <summary>
    /// </summary>
    /// <param name="snapshot">Snapshot (original copy without modifications).</param>
    /// <param name="current">Changed version of the object.</param>
    /// <exception cref="InvalidOperationException">One or more arguments are null.</exception>
    public void Compare(object? snapshot, object? current)
    {
        var mapping = _registry.Get(current?.GetType() ??
                                    snapshot?.GetType() ??
                                    throw new InvalidOperationException("Failed to get entity type."));
        Compare(null, snapshot, current, mapping, 1);
    }

    /// <summary>
    /// </summary>
    /// <param name="parent">Parent entity (use current parent if it exists, fallback to snapshot parent).</param>
    /// <param name="snapshot"></param>
    /// <param name="current"></param>
    /// <param name="mapping"></param>
    /// <param name="depth"></param>
    /// <exception cref="InvalidOperationException"></exception>
    private void Compare(
        object parent,
        object? snapshot,
        object? current,
        ClassMapping mapping,
        int depth)
    {
        if (snapshot == null && current == null)
        {
            return;
        }

        if (snapshot == null || current == null)
        {
            if (current != null)
            {
                _diff.Added(parent, current, depth);
            }

            else if (snapshot != null)
            {
                _diff.Removed(parent, snapshot, depth);
            }

            return;
        }

        if (snapshot.GetType() != current.GetType())
        {
            throw new InvalidOperationException(
                $"Expected to compare two objects of same type. Got {snapshot.GetType()} and {current.GetType()}");
        }

        var isEqual = true;
        foreach (var prop in mapping.Properties)
        {
            if (!prop.CanWriteToDatabase)
            {
                continue;
            }

            var snapShotValue = prop.GetColumnValue(snapshot);
            var currentValue = prop.GetColumnValue(current);
            if (snapShotValue == null && currentValue == null)
            {
                continue;
            }

            if (snapShotValue == null || currentValue == null)
            {
                isEqual = false;
                break;
            }

            if (Equals(snapShotValue, currentValue))
            {
                continue;
            }

            isEqual = false;
            break;
        }

        if (!isEqual)
        {
            _diff.Modified(parent, current, depth);
        }

        CompareCollections(snapshot, current, mapping, depth + 1);
        CompareChildren(snapshot, current, mapping, depth + 1);
    }

    private void CompareChildren(
        object snapshot,
        object current,
        ClassMapping parentMapping,
        int depth)
    {
        var parent = current ?? snapshot;
        foreach (var child in parentMapping.Children)
        {
            var snapshotValue = child.GetColumnValue(snapshot);
            var currentValue = child.GetColumnValue(current);

            var entityType = child.HaveDiscriminator
                ? child.GetTypeUsingDiscriminator(snapshot)
                : child.ChildEntityType;
            if (entityType == null)
            {
                continue;
            }

            var mapping = _registry.Get(entityType);
            Compare(parent, snapshotValue, currentValue, mapping, depth + 1);
        }
    }

    private void CompareCollections(
        object snapshot,
        object current,
        ClassMapping parentMapping,
        int depth)
    {
        var parent = current ?? snapshot;
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
            {
                throw new MappingConfigurationException(collection.ChildEntityType, "Entity does not have a key.");
            }

            foreach (var item in snapshotValue)
            {
                var key = mapping.GenerateKey(item);
                if (key == null)
                {
                    throw new MappingException(item, "Expected existing child to have an identity.");
                }

                snapshotIndex[key] = item;
            }

            foreach (var item in currentValue)
            {
                var key = mapping.GenerateKey(item);
                if (key == null)
                {
                    _diff.Added(parent, item, depth);
                    continue;
                }

                currentIndex[key] = item;
            }

            var snapshotEquals = new List<KeyValuePair<object, object>>();
            foreach (var kvp in snapshotIndex)
            {
                if (currentIndex.ContainsKey(kvp.Key))
                {
                    snapshotEquals.Add(kvp);
                }
                else
                {
                    _diff.Removed(parent, kvp.Value, depth);
                }
            }

            foreach (var kvp in currentIndex)
            {
                if (!snapshotIndex.ContainsKey(kvp.Key))
                {
                    _diff.Added(parent, kvp.Value, depth);
                }
            }

            var childMapping = _registry.Get(collection.ChildEntityType);
            foreach (var equal in snapshotEquals)
            {
                var currentChild = currentIndex[equal.Key];
                Compare(parent, equal.Value, currentChild, childMapping, depth + 1);
            }
        }
    }
}

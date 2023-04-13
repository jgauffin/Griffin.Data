using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Griffin.Data.Helpers;
using Griffin.Data.Mappings;
using Griffin.Data.Mappings.Relations;

namespace Griffin.Data.Mapper.Helpers;

internal static class FetchChildrenOperations
{
    internal static async Task GetChildren<TParent>(this Session session, [DisallowNull] TParent parentEntity)
    {
        if (parentEntity == null)
        {
            throw new ArgumentNullException(nameof(parentEntity));
        }

        if (typeof(TParent) == typeof(object))
        {
            throw new ArgumentException("Entity type cannot be 'object'.");
        }

        var parentMapping = session.GetMapping<TParent>();

        var options = new QueryOptions();
        foreach (var hasManyMapping in parentMapping.Collections)
        {
            options.DbParameters = hasManyMapping.CreateDbConstraints(new[] { parentEntity });
            var collection = hasManyMapping.CreateCollection();

            await session.Query(hasManyMapping.ChildEntityType, options, collection);
            hasManyMapping.SetCollection(parentEntity, collection);
        }

        foreach (var hasOneMapping in parentMapping.Children)
        {
            options.DbParameters = hasOneMapping.CreateDbConstraints(new[] { parentEntity });

            var childType = hasOneMapping.ChildEntityType;
            if (hasOneMapping.HaveDiscriminator)
            {
                childType = hasOneMapping.GetTypeUsingDiscriminator(parentEntity) ?? hasOneMapping.ChildEntityType;
            }

            var child = await session.FirstOrDefault(childType, options);
            if (child != null)
            {
                hasOneMapping.SetPropertyValue(parentEntity, child);
            }
        }
    }

    internal static async Task GetChildren(this Session session, Type parentType, [DisallowNull] object parentEntity)
    {
        if (parentEntity == null)
        {
            throw new ArgumentNullException(nameof(parentEntity));
        }

        if (parentType == typeof(object))
        {
            throw new ArgumentException("Entity type cannot be 'object'.");
        }

        var parentMapping = session.GetMapping(parentType);

        var options = new QueryOptions();
        foreach (var hasManyMapping in parentMapping.Collections)
        {
            options.DbParameters = hasManyMapping.CreateDbConstraints(new[] { parentEntity });
            var collection = hasManyMapping.CreateCollection();
            await session.Query(hasManyMapping.ChildEntityType, options, collection);
            hasManyMapping.SetCollection(parentEntity, collection);
        }

        foreach (var hasOneMapping in parentMapping.Children)
        {
            options.DbParameters = hasOneMapping.CreateDbConstraints(new[] { parentEntity });

            var childType = hasOneMapping.ChildEntityType;
            if (hasOneMapping.HaveDiscriminator)
            {
                childType = hasOneMapping.GetTypeUsingDiscriminator(parentEntity) ?? hasOneMapping.ChildEntityType;
            }

            var child = await session.FirstOrDefault(childType, options);
            if (child != null)
            {
                hasOneMapping.SetPropertyValue(parentEntity, child);
            }
        }
    }

    internal static async Task GetChildrenForMany(this Session session, Type parentType, IList parents)
    {
        if (parents == null)
        {
            throw new ArgumentNullException(nameof(parents));
        }

        if (parents.Count == 0)
        {
            return;
        }

        var options = new QueryOptions();
        var parentMapping = session.GetMapping(parentType);
        foreach (var hasManyMapping in parentMapping.Collections)
        {
            var childCollections = new Dictionary<object, IList>();
            foreach (var parent in parents)
            {
                var parentId = hasManyMapping.GetReferencedId(parent!);
                if (parentId == null)
                {
                    throw new MappingException(parent,
                        $"Failed to get referenced column for child {hasManyMapping.ChildEntityType.Name} using FK {hasManyMapping.ForeignKeyColumnName}. Cannot load child entities.");
                }

                var col = hasManyMapping.CreateCollection();
                hasManyMapping.SetCollection(parent, col);
                childCollections[parentId] = col;
            }

            options.DbParameters = hasManyMapping.CreateDbConstraints(parents);

            IList allChildrenToGetChildrenFor = (IList)Activator.CreateInstance
                                                (typeof(List<>).MakeGenericType(hasManyMapping.ChildEntityType));
            var childMapping = session.GetMapping(hasManyMapping.ChildEntityType);
            await using var cmd = session.CreateQueryCommand(hasManyMapping.ChildEntityType, options);
            try
            {
                await using var reader = await cmd.ExecuteReaderAsync();
                await reader.MapAll(childMapping, x =>
                {
                    allChildrenToGetChildrenFor.Add(x);
                    var fk = hasManyMapping.GetForeignKeyValue(x);
                    if (fk == null)
                    {
                        throw new MappingException(x,
                            "Failed to lookup parent using foreign key, cannot attach child.");
                    }

                    childCollections[fk].Add(x);
                });
            }
            catch (Exception ex)
            {
                throw cmd.CreateDetailedException(ex, parentType, hasManyMapping.ChildEntityType);
            }

            // Now load our children
            await session.GetChildrenForMany(hasManyMapping.ChildEntityType, allChildrenToGetChildrenFor);
        }

        foreach (var hasOneMapping in parentMapping.Children)
        {
            if (hasOneMapping.HaveDiscriminator)
            {
                await FetchUsingDiscriminator(session, parentMapping, hasOneMapping, parents);
                continue;
            }

            var parentIndex = new Dictionary<object, object>();
            foreach (var parent in parents)
            {
                var parentId = hasOneMapping.GetReferencedId(parent!);
                if (parentId == null)
                {
                    throw new MappingException(parent,
                        $"Failed to get referenced column for child {hasOneMapping.ChildEntityType.Name} using FK {hasOneMapping.ForeignKeyColumnName}. Cannot load child entities.");
                }

                parentIndex[parentId] = parent;
            }

            options.DbParameters = hasOneMapping.CreateDbConstraints(parents);

            var childMapping = session.GetMapping(hasOneMapping.ChildEntityType);

            

            await using var cmd = session.CreateQueryCommand(hasOneMapping.ChildEntityType, options);
            try
            {
                IList allChildrenToGetChildrenFor = (IList)Activator.CreateInstance
                    (typeof(List<>).MakeGenericType(hasOneMapping.ChildEntityType));

                await using var reader = await cmd.ExecuteReaderAsync();
                await reader.MapAll(childMapping, x =>
                {
                    allChildrenToGetChildrenFor.Add(x);
                    var fkValue = hasOneMapping.GetForeignKeyValue(x);
                    if (fkValue == null)
                    {
                        throw new MappingException(x, "Failed to lookup parent using foreign key, cannot attach child.");
                    }

                    hasOneMapping.SetPropertyValue(parentIndex[fkValue], x);
                });

                await session.GetChildrenForMany(hasOneMapping.ChildEntityType, allChildrenToGetChildrenFor);
            }
            catch (Exception ex)
            {
                throw cmd.CreateDetailedException(ex, parentType, hasOneMapping.ChildEntityType);
            }
        }
    }

    /// <summary>
    ///     Used to load values for multiple parents at the same time using a discriminator (i.e. child entities have sub
    ///     classes).
    /// </summary>
    /// <param name="session"></param>
    /// <param name="parentMapping"></param>
    /// <param name="hasOneMapping"></param>
    /// <param name="parents"></param>
    /// <returns></returns>
    /// <exception cref="MappingException"></exception>
    private static async Task FetchUsingDiscriminator(
        Session session,
        ClassMapping parentMapping,
        IHasOneMapping hasOneMapping,
        IEnumerable parents)
    {
        var discriminatorIndex = new Dictionary<Type, IList>();
        var parentIndex = new Dictionary<object, object>();
        Type parentType =typeof(object);

        foreach (var parent in parents)
        {
            var fk = hasOneMapping.GetReferencedId(parent);
            if (fk == null)
            {
                throw new MappingException(parent,
                    $"Failed to get referenced column for child {hasOneMapping.ChildEntityType.Name} using FK {hasOneMapping.ForeignKeyColumnName}. Cannot load child entities.");
            }

            parentType = parent.GetType();

            parentIndex[fk] = parent;

            var den = hasOneMapping.GetTypeUsingDiscriminator(parent);
            if (den == null)
            {
                // Not getting a type simply means that no child should be loaded.
                continue;
            }

            if (!discriminatorIndex.TryGetValue(den, out var items))
            {
                items = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(parentMapping.EntityType));
                discriminatorIndex[den] = items;
            }

            items.Add(parent);
        }

        var options = new QueryOptions();
        foreach (var kvp in discriminatorIndex)
        {
            options.DbParameters = hasOneMapping.CreateDbConstraints(kvp.Value);

            var childMapping = session.GetMapping(kvp.Key);
            IList allChildrenToGetChildrenFor = (IList)Activator.CreateInstance
                (typeof(List<>).MakeGenericType(kvp.Key));

            await using var cmd = session.CreateQueryCommand(kvp.Key, options);
            try
            {
                await using var reader = await cmd.ExecuteReaderAsync();
                await reader.MapAll(childMapping, x =>
                {
                    allChildrenToGetChildrenFor.Add(x);
                    var fkValue = hasOneMapping.GetForeignKeyValue(x);
                    if (fkValue == null)
                    {
                        throw new MappingException(x,
                            "Failed to lookup parent using foreign key, cannot attach child.");
                    }

                    hasOneMapping.SetPropertyValue(parentIndex[fkValue], x);
                });

                await session.GetChildrenForMany(kvp.Key, allChildrenToGetChildrenFor);

            }
            catch (Exception ex)
            {
                throw cmd.CreateDetailedException(ex, parentType, kvp.Key);
            }
        }
    }
}

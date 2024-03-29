﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Griffin.Data.Helpers;

namespace Griffin.Data.Mapper.Helpers;

internal static class QueryExtensions
{
    /// <summary>
    ///     Load a collection (for like HasMany), will also load children.
    /// </summary>
    /// <param name="session"></param>
    /// <param name="entityType"></param>
    /// <param name="options"></param>
    /// <param name="collection"></param>
    /// <returns></returns>
    public static async Task Query(
        this Session session,
        Type entityType,
        QueryOptions options,
        IList collection)
    {
        var mapping = session.GetMapping(entityType);
        await using (var cmd = session.CreateQueryCommand(entityType, options))
        {
            try
            {
                await using var reader = await cmd.ExecuteReaderAsync();
                await reader.MapAll(mapping, x => { collection.Add(x); });
            }
            catch (DbException ex)
            {
                throw new MapperException($"Could not fetch entities of type {entityType.Name}.", cmd, entityType, ex);
            }
        }

        if (collection.Count > 0 && options.LoadChildren)
        {
            await session.GetChildrenForMany(entityType, collection);
        }
    }

    public static async Task<List<TEntity>> QueryInternal<TEntity>(this Session session, QueryOptions<TEntity> options)
    {
        if (typeof(TEntity) == typeof(object) || typeof(TEntity).IsCollection())
        {
            throw new InvalidOperationException("bug bug bug.,");
        }

        var items = new List<TEntity>();
        var mapping = session.GetMapping(typeof(TEntity));
        await using (var cmd = session.CreateQueryCommand(typeof(TEntity), options.Options))
        {
            try
            {
                await using var reader = await cmd.ExecuteReaderAsync();
                await reader.MapAll<TEntity>(mapping, x => { items.Add(x); });
            }
            catch (DbException ex)
            {
                var entityType = typeof(TEntity);
                throw new MapperException($"Could not fetch entities of type {entityType.Name}.", cmd, entityType, ex);
            }
        }

        if (options.Options.LoadChildren)
        {
            await session.GetChildrenForMany(typeof(TEntity), items);
        }

        return items;
    }

    public static async Task<List<TEntity>> QueryInternal<TEntity>(this Session session, QueryOptions options)
    {
        if (typeof(TEntity) == typeof(object) || typeof(TEntity).IsCollection())
        {
            throw new InvalidOperationException("bug bug bug.,");
        }

        var items = new List<TEntity>();
        var mapping = session.GetMapping(typeof(TEntity));
        await using (var cmd = session.CreateQueryCommand(typeof(TEntity), options))
        {
            try
            {
                await using var reader = await cmd.ExecuteReaderAsync();
                await reader.MapAll<TEntity>(mapping, x => { items.Add(x); });
            }
            catch (DbException ex)
            {
                var entityType = typeof(TEntity);
                throw new MapperException($"Could not fetch entities of type {entityType.Name}.", cmd, entityType, ex);
            }
        }

        if (options.LoadChildren)
        {
            await session.GetChildrenForMany(typeof(TEntity), items);
        }

        return items;
    }
}

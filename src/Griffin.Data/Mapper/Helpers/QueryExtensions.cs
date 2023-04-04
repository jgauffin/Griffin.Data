using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Griffin.Data;
using Griffin.Data.Helpers;

namespace Griffin.Data.Mapper.Helpers;

internal static class QueryExtensions
{
    public static async Task<List<TEntity>> Query<TEntity>(this Session session, QueryOptions<TEntity> options)
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
                throw cmd.CreateDetailedException(ex);
            }
        }

        if (options.Options.LoadChildren)
        {
            await session.GetChildrenForMany(typeof(TEntity), items);
        }

        return items;
    }

    public static async Task Query(this Session session, Type entityType, QueryOptions options,
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
                throw cmd.CreateDetailedException(ex);
            }
        }

        if (collection.Count > 0)
        {
            await session.GetChildrenForMany(entityType, collection);
        }

    }


}
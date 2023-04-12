﻿using System;
using System.Threading.Tasks;
using Griffin.Data.Mapper.Helpers;

namespace Griffin.Data.Mapper;

/// <summary>
///     Extensions to fetch a single entity.
/// </summary>
public static class GetOneExtensions
{
    /// <summary>
    ///     Check if an entity exists in the database.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity.</typeparam>
    /// <param name="session">Session to make the query in.</param>
    /// <param name="constraints">Parameters used to limit the search result.</param>
    /// <returns></returns>
    public static async Task<bool> Exists<TEntity>(this Session session, object constraints)
    {
        await using var cmd = session.CreateQueryCommand(typeof(TEntity), QueryOptions.Where(constraints));
        await using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync();
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="session"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static async Task<TEntity> First<TEntity>(this Session session, QueryOptions options) where TEntity : notnull
    {
        options.PageSize = 1;

        var mapping = session.GetMapping<TEntity>();
        TEntity entity;
        await using (var cmd = session.CreateQueryCommand(typeof(TEntity), options))
        {
            entity = await cmd.GetSingle<TEntity>(mapping);
        }

        if (options.LoadChildren)
        {
            await session.GetChildren(entity);
        }

        session.Track(entity);
        return entity;
    }

    /// <summary>
    ///     Find first entity.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity.</typeparam>
    /// <param name="session">Session to use for the SQL query.</param>
    /// <param name="constraints">Parameters to build a WHERE clause from.</param>
    /// <returns></returns>
    /// <exception cref="EntityNotFoundException">Entity was not found.</exception>
    public static Task<TEntity> First<TEntity>(this Session session, object constraints) where TEntity : notnull
    {
        var options = new QueryOptions(null, constraints);
        return session.First<TEntity>(options);
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="session"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static async Task<TEntity?> FirstOrDefault<TEntity>(this Session session, QueryOptions options)
    {
        options.PageSize = 1;

        var mapping = session.GetMapping<TEntity>();
        TEntity? entity;
        await using (var cmd = session.CreateQueryCommand(typeof(TEntity), options))
        {
            entity = await cmd.GetSingleOrDefault<TEntity>(mapping);
        }

        if (entity == null)
        {
            return default;
        }

        if (options.LoadChildren)
        {
            await session.GetChildren(entity);
        }

        session.Track(entity);
        return entity;
    }

    /// <summary>
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <param name="session"></param>
    /// <param name="constraints"></param>
    /// <returns></returns>
    public static Task<TEntity?> FirstOrDefault<TEntity>(this Session session, object constraints)
    {
        var options = new QueryOptions { PageSize = 1 };

        return session.FirstOrDefault<TEntity>(options);
    }

    /// <summary>
    /// </summary>
    /// <param name="session"></param>
    /// <param name="entityType"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static async Task<object?> FirstOrDefault(this Session session, Type entityType, QueryOptions options)
    {
        if (entityType == null)
        {
            throw new ArgumentNullException(nameof(entityType));
        }

        if (entityType == typeof(object))
        {
            throw new ArgumentException("Entity type cannot be 'object'.");
        }

        options.PageSize = 1;

        var mapping = session.GetMapping(entityType);
        object? entity;
        await using (var cmd = session.CreateQueryCommand(entityType, options))
        {
            entity = await cmd.GetSingleOrDefault(mapping, options);
        }

        if (entity == null)
        {
            return default;
        }

        if (options.LoadChildren)
        {
            await session.GetChildren(entityType, entity);
        }

        session.Track(entity);
        return entity;
    }
}

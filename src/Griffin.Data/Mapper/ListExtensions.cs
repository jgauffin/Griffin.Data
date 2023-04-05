using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Griffin.Data.Mapper.Helpers;

namespace Griffin.Data.Mapper;

/// <summary>
///     Functions to load multiple entities at once.
/// </summary>
public static class ListExtensions
{
    /// <summary>
    ///     List entities.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity.</typeparam>
    /// <param name="session">Session to load in.</param>
    /// <param name="options">Options used to limit the search result.</param>
    /// <returns>Found entities.</returns>
    public static async Task<List<TEntity>> List<TEntity>(this Session session, QueryOptions<TEntity> options) where TEntity: notnull
    {
        var entities = await session.Query(options);
        foreach (var entity in entities) session.Track(entity);

        return entities;
    }

    /// <summary>
    ///     List entities.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity.</typeparam>
    /// <param name="session">Session to load in.</param>
    /// <param name="constraints">Constrains with property names.</param>
    /// <returns>Found entities.</returns>
    public static Task<List<TEntity>> List<TEntity>(this Session session, object constraints) where TEntity : notnull
    {
        return session.List(new QueryOptions<TEntity>(constraints));
    }

    /// <summary>
    ///     List entities.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity.</typeparam>
    /// <param name="session">Session to load in.</param>
    /// <param name="query">SQL query (partial or complete, refer to the wiki documentation).</param>
    /// <param name="constraints">
    ///     Parameters used in the query. Can either be used stand alone or to specify the parameters
    ///     used in the SQL query.
    /// </param>
    /// <returns>Found entities.</returns>
    public static async Task<List<TEntity>> List<TEntity>(this Session session, string query,
        object constraints) where TEntity : notnull
    {
        return await session.List(new QueryOptions<TEntity>(query, constraints));
    }

    /// <summary>
    ///     List entities.
    /// </summary>
    /// <param name="entityType">Type of query to list.</param>
    /// <param name="session">Session to load in.</param>
    /// <param name="options">Options used to limit the search result.</param>
    /// <returns>A generic list (for the specified entity type), but returned as <c>IList</c>.</returns>
    public static async Task<IList> List(this Session session, Type entityType, QueryOptions options)
    {
        var collection = (IList)Activator.CreateInstance(typeof(IList<>).MakeGenericType(entityType));
        await session.Query(entityType, options, collection);
        foreach (var entity in collection) session.Track(entity);

        return collection;
    }

    /// <summary>
    ///     List entities.
    /// </summary>
    /// <param name="session">Session to load in.</param>
    /// <param name="entityType">Type of query to list.</param>
    /// <param name="query">SQL query (partial or complete, refer to the wiki documentation).</param>
    /// <param name="constraints">
    ///     Parameters used in the query. Can either be used stand alone or to specify the parameters
    ///     used in the SQL query.
    /// </param>
    /// <returns>A generic list (for the specified entity type), but returned as <c>IList</c>.</returns>
    public static async Task<IList> List(this Session session, Type entityType, string? query = null,
        object? constraints = null)
    {
        var collection = (IList)Activator.CreateInstance(typeof(IList<>).MakeGenericType(entityType));
        await session.Query(entityType, new QueryOptions(query, constraints), collection);
        foreach (var entity in collection) session.Track(entity);

        return collection;
    }
}
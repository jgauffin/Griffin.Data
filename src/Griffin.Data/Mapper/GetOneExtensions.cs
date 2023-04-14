using System;
using System.Data.Common;
using System.Threading.Tasks;
using Griffin.Data.Helpers;
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
        try
        {

            await using var reader = await cmd.ExecuteReaderAsync();
            return await reader.ReadAsync();
        }
        catch (DbException ex)
        {
            var our = cmd.CreateDetailedException(ex, typeof(TEntity));
            our.Data["Constraints"] = constraints;
            throw our;
        }
    }

    /// <summary>
    ///     Find first entity.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity.</typeparam>
    /// <param name="session">Session to use for the SQL query.</param>
    /// <param name="constraints">Parameters to build a WHERE clause from.</param>
    /// <returns></returns>
    /// <exception cref="EntityNotFoundException">Entity was not found.</exception>
    public static async Task<TEntity> First<TEntity>(this Session session, object constraints) where TEntity : notnull
    {
        var options = GetQueryOptionsFromConstraints<TEntity>(null, constraints);
        var entity = await session.FirstOrDefault<TEntity>(options);
        if (entity == null)
            throw new EntityNotFoundException(typeof(TEntity), constraints);

        return entity;
    }

    /// <summary>
    ///     Find first entity.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity.</typeparam>
    /// <param name="session">Session to use for the SQL query.</param>
    /// <param name="options">Query to use..</param>
    /// <returns></returns>
    /// <exception cref="EntityNotFoundException">Entity was not found.</exception>
    public static async Task<TEntity> First<TEntity>(this Session session, QueryOptions<TEntity> options) where TEntity : notnull
    {
        var entity = await session.FirstOrDefault<TEntity>(options.Options);
        if (entity == null)
            throw new EntityNotFoundException(typeof(TEntity), options);

        return entity;
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
        var options = GetQueryOptionsFromConstraints<TEntity>(null, constraints);
        options.PageSize = 1;
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

    internal static QueryOptions GetQueryOptionsFromConstraints<TEntity>(string? sql, object? constraints)
    {
        return constraints switch
        {
            QueryOptions options => options,
            QueryOptions<TEntity> gen => gen.Options,
            _ => new QueryOptions(sql, constraints)
        };
    }

    internal static QueryOptions GetQueryOptionsFromConstraints(string? sql, object? constraints)
    {
        return constraints switch
        {
            QueryOptions options => options,
            _ => new QueryOptions(sql, constraints)
        };
    }
}

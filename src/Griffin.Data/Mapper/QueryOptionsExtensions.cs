using System.Collections.Generic;
using System.Threading.Tasks;
using Griffin.Data.Mapper.Mappings;

namespace Griffin.Data.Mapper;

/// <summary>
///     Extensions for queries.
/// </summary>
public static class QueryOptionsExtensions
{
    /// <summary>
    ///     Fetch a single entity.
    /// </summary>
    /// <typeparam name="T">Type of entity.</typeparam>
    /// <param name="options">Options to apply.</param>
    /// <returns>entity.</returns>
    /// <exception cref="MappingException">Failed to find a mapping for the requested entity type.</exception>
    /// <exception cref="EntityNotFoundException">Entity was not found.</exception>
    public static Task<T> First<T>(this QueryOptions<T> options) where T : notnull
    {
        if (options.Session == null)
        {
            throw new MappingException(typeof(T), "!oops");
        }

        return options.Session.First<T>(options.Options);
    }

    /// <summary>
    ///     Fetch a single entity.
    /// </summary>
    /// <typeparam name="T">Type of entity.</typeparam>
    /// <param name="options">Options to apply.</param>
    /// <returns>Entity if found; otherwise <c>null</c>.</returns>
    /// <exception cref="MappingException">Failed to find a mapping for the requested entity type.</exception>
    public static Task<T?> FirstOrDefault<T>(this QueryOptions<T> options) where T : notnull
    {
        if (options.Session == null)
        {
            throw new MappingException(typeof(T), "!oops");
        }

        return options.Session.FirstOrDefault<T>(options.Options);
    }

    /// <summary>
    ///     List all entities that matches the query.
    /// </summary>
    /// <typeparam name="T">Type of entity.</typeparam>
    /// <param name="options">Options to apply.</param>
    /// <returns>Entity if found; otherwise an empty list.</returns>
    /// <exception cref="MappingException">Failed to find a mapping for the requested entity type.</exception>
    public static Task<List<T>> List<T>(this QueryOptions<T> options) where T : notnull
    {
        if (options.Session == null)
        {
            throw new MappingException(typeof(T), "!oops");
        }

        return options.Session.List(options);
    }

    /// <summary>
    ///     List all entities that matches the query.
    /// </summary>
    /// <typeparam name="T">Type of entity.</typeparam>
    /// <returns>Entity if found; otherwise an empty list.</returns>
    /// <exception cref="MappingException">Failed to find a mapping for the requested entity type.</exception>
    public static QueryOptions<T> Query<T>(this Session session)
    {
        return new QueryOptions<T>(session);
    }

    /// <summary>
    ///     List all entities that matches the query.
    /// </summary>
    /// <typeparam name="T">Type of entity.</typeparam>
    /// <param name="session">Session to use for the query.</param>
    /// <param name="propertyConstraints">Parameters with property names. Use '%' in the value for a LIKE condition.</param>
    /// <returns>Entity if found; otherwise an empty list.</returns>
    /// <exception cref="MappingException">Failed to find a mapping for the requested entity type.</exception>
    public static QueryOptions<T> Query<T>(this Session session, object propertyConstraints)
    {
        return new QueryOptions<T>(session, propertyConstraints);
    }

    /// <summary>
    ///     List all entities that matches the query.
    /// </summary>
    /// <typeparam name="T">Type of entity.</typeparam>
    /// <param name="session">Session to use for the query.</param>
    /// <param name="sql">SQL query. Either short form = everything behind WHERE, or a complete SQL statement.</param>
    /// <param name="parameters">Parameters, keys must match those in the query. Use '%' in the value for a LIKE condition.</param>
    /// <returns>Entity if found; otherwise an empty list.</returns>
    /// <exception cref="MappingException">Failed to find a mapping for the requested entity type.</exception>
    public static QueryOptions<T> Query<T>(this Session session, string sql, object parameters)
    {
        return new QueryOptions<T>(session, sql, parameters);
    }
}

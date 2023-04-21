using System;
using System.Threading.Tasks;

namespace Griffin.Data.Mapper;

/// <summary>
///     Extensions methods for <c>GetById()</c>.
/// </summary>
public static class GetByIdExtensions
{
    /// <summary>
    ///     Get by id.
    /// </summary>
    /// <typeparam name="T">Type of entity to fetch.</typeparam>
    /// <param name="session">Session to fetch entity in.</param>
    /// <param name="id">Primary key.</param>
    /// <returns>Found entity.</returns>
    /// <exception cref="MappingException">Entity has more than one primary key.</exception>
    public static async Task<T> GetById<T>(this Session session, int id) where T : notnull
    {
        var mapping = session.GetMapping<T>();

        if (mapping.Keys.Count != 1)
        {
            throw new MappingException(typeof(T), "GetById requires a single key.");
        }

        var key = mapping.Keys[0];

        var item = await session.First<T>(new QueryOptions($"{key.PropertyName} = @id", new { id }));
        session.Track(item);
        return item;
    }

    /// <summary>
    ///     Get by id.
    /// </summary>
    /// <typeparam name="T">Type of entity to fetch.</typeparam>
    /// <param name="session">Session to fetch entity in.</param>
    /// <param name="id">Primary key.</param>
    /// <returns>Found entity.</returns>
    /// <exception cref="MappingException">Entity has more than one primary key.</exception>
    public static async Task<T> GetById<T>(this Session session, string id) where T : notnull
    {
        var mapping = session.GetMapping<T>();

        if (mapping.Keys.Count != 1)
        {
            throw new MappingException(typeof(T), "GetById requires a single key.");
        }

        var key = mapping.Keys[0];

        var item = await session.First<T>(new QueryOptions($"{key.PropertyName} = @id", new { id }));
        session.Track(item);
        return item;
    }

    /// <summary>
    ///     Get by id.
    /// </summary>
    /// <typeparam name="T">Type of entity to fetch.</typeparam>
    /// <param name="session">Session to fetch entity in.</param>
    /// <param name="id">Primary key.</param>
    /// <returns>Found entity.</returns>
    /// <exception cref="MappingException">Entity has more than one primary key.</exception>
    public static async Task<T> GetById<T>(this Session session, Guid id) where T : notnull
    {
        var mapping = session.GetMapping<T>();

        if (mapping.Keys.Count != 1)
        {
            throw new MappingException(typeof(T), "GetById requires a single key.");
        }

        var key = mapping.Keys[0];

        var item = await session.First<T>(new QueryOptions($"{key.PropertyName} = @id", new { id }));
        session.Track(item);
        return item;
    }

    /// <summary>
    ///     Get by id.
    /// </summary>
    /// <typeparam name="T">Type of entity to fetch.</typeparam>
    /// <typeparam name="TKey">Type of primary key.</typeparam>
    /// <param name="session">Session to fetch entity in.</param>
    /// <param name="id">Primary key.</param>
    /// <returns>Found entity.</returns>
    /// <exception cref="MappingException">Entity has more than one primary key.</exception>
    public static async Task<T> GetById<T, TKey>(this Session session, TKey id) where T : notnull
    {
        var mapping = session.GetMapping<T>();

        if (mapping.Keys.Count != 1)
        {
            throw new MappingException(typeof(T), "GetById requires a single key.");
        }

        var key = mapping.Keys[0];

        var item = await session.First<T>(new QueryOptions($"{key.PropertyName} = @id", new { id }));
        session.Track(item);
        return item;
    }
}

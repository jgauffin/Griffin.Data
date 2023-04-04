using System;
using System.Threading.Tasks;

namespace Griffin.Data.Mapper;

public static class GetByIdExtensions
{
    public static async Task<T> GetById<T>(this Session session, int id) where T : notnull
    {
        var mapping = session.GetMapping<T>();

        if (mapping.Keys.Count != 1) throw new MappingException(typeof(T), "GetById requires a single key.");

        var key = mapping.Keys[0];

        var item = await session.First<T>(new QueryOptions($"{key.PropertyName} = @id", new { id }));
        session.Track(item);
        return item;
    }

    public static async Task<T> GetById<T>(this Session session, string id) where T : notnull
    {
        var mapping = session.GetMapping<T>();

        if (mapping.Keys.Count != 1) throw new MappingException(typeof(T), "GetById requires a single key.");

        var key = mapping.Keys[0];

        var item = await session.First<T>(new QueryOptions($"{key.PropertyName} = @id", new { id }));
        session.Track(item);
        return item;
    }

    public static async Task<T> GetById<T>(this Session session, Guid id) where T : notnull
    {
        var mapping = session.GetMapping<T>();

        if (mapping.Keys.Count != 1) throw new MappingException(typeof(T), "GetById requires a single key.");

        var key = mapping.Keys[0];

        var item = await session.First<T>(new QueryOptions($"{key.PropertyName} = @id", new { id }));
        session.Track(item);
        return item;
    }

    public static async Task<T> GetById<T, TKey>(this Session session, TKey id) where T : notnull
    {
        var mapping = session.GetMapping<T>();

        if (mapping.Keys.Count != 1) throw new MappingException(typeof(T), "GetById requires a single key.");

        var key = mapping.Keys[0];

        var item = await session.First<T>(new QueryOptions($"{key.PropertyName} = @id", new { id }));
        session.Track(item);
        return item;
    }
}
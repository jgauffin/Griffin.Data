using System.Collections.Generic;
using System.Threading.Tasks;

namespace Griffin.Data.Mapper;

/// <summary>
///     Extensions for queries.
/// </summary>
public static class SessionExtensions
{
    public static Task<T> First<T>(this QueryOptions<T> options) where T : notnull
    {
        if (options.Session == null)
        {
            throw new MappingException(typeof(T), "!oops");
        }

        return options.Session.First<T>(options.Options);
    }

    public static Task<T?> FirstOrDefault<T>(this QueryOptions<T> options) where T : notnull
    {
        if (options.Session == null)
        {
            throw new MappingException(typeof(T), "!oops");
        }

        return options.Session.FirstOrDefault<T>(options.Options);
    }

    public static Task<List<T>> List<T>(this QueryOptions<T> options) where T : notnull
    {
        if (options.Session == null)
        {
            throw new MappingException(typeof(T), "!oops");
        }

        return options.Session.List(options);
    }

    public static QueryOptions<T> Query<T>(this Session session)
    {
        return new QueryOptions<T> { Session = session };
    }

    public static QueryOptions<T> Query<T>(this Session session, object propertyConstraints)
    {
        return new QueryOptions<T> { Session = session };
    }

    public static QueryOptions<T> Query<T>(this Session session, string sql, object parameters)
    {
        return new QueryOptions<T> { Session = session };
    }
}

using System;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Griffin.Data.Mapper;

namespace Griffin.Data.Queries;

/// <summary>
///     Base class for queries that returns a single entity.
/// </summary>
/// <typeparam name="TResult">Type of result to return.</typeparam>
public abstract class GetHandler<TResult>
    where TResult : new()

{
    /// <summary>
    /// </summary>
    /// <param name="session">Session to fetch entity in.</param>
    protected GetHandler(QuerySession session)
    {
        Session = session;
    }

    /// <summary>
    ///     Session to fetch entity in.
    /// </summary>
    protected QuerySession Session { get; }

    /// <summary>
    ///     Execute reader and map an entity.
    /// </summary>
    /// <param name="command">Command ready to be executed.</param>
    /// <returns>Entity if found; otherwise <c>null</c>.</returns>
    protected async Task<TResult?> FetchRecord(DbCommand command)
    {
        try
        {
            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return default;
            }

            var item = new TResult();
            MapRecord(reader, item);

            return item;
        }
        catch (GriffinException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new MapperException($"Failed to fetch record of type {typeof(TResult)}.", command, typeof(TResult), ex);
        }
    }

    /// <summary>
    ///     Map record to the entity.
    /// </summary>
    /// <param name="record">Row in the record set result.</param>
    /// <param name="item">Entity to assign values to.</param>
    /// <remarks>
    ///     <para>
    ///         Assign all properties from the record in this method.
    ///     </para>
    /// </remarks>
    protected abstract void MapRecord(IDataRecord record, TResult item);
}

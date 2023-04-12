using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Griffin.Data.Queries;

/// <summary>
///     Base class for queries that returns a single entity.
/// </summary>
/// <typeparam name="TResult">Type of result to return.</typeparam>
public abstract class GetRunner<TResult>
    where TResult : new()

{
    /// <summary>
    /// </summary>
    /// <param name="session">Session to fetch entity in.</param>
    protected GetRunner(Session session)
    {
        Session = session;
    }

    /// <summary>
    ///     Session to fetch entity in.
    /// </summary>
    protected Session Session { get; }

    /// <summary>
    ///     Execute reader and map an entity.
    /// </summary>
    /// <param name="command">Command ready to be executed.</param>
    /// <returns>Entity if found; otherwise <c>null</c>.</returns>
    protected async Task<TResult?> FetchRecord(DbCommand command)
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

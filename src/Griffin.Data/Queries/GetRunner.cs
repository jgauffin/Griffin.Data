using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Griffin.Data.Queries;

public abstract class GetRunner<TResult>
    where TResult : new()

{
    protected GetRunner(Session session)
    {
        Session = session;
    }

    protected Session Session { get; }

    protected abstract void MapRecord(IDataRecord record, TResult item);

    protected async Task<TResult?> MapRecords(DbCommand command)
    {
        await using var reader = await command.ExecuteReaderAsync();
        if (!await reader.ReadAsync())
            return default;

        var item = new TResult();
        MapRecord(reader, item);

        return item;
    }
}

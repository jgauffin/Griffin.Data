using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;

namespace Griffin.Data.Queries;

public abstract class ListRunner<TResultItem>
    where TResultItem : new()

{
    protected ListRunner(Session session)
    {
        Session = session;
    }

    protected Session Session { get; }

    protected abstract void MapRecord(IDataRecord record, TResultItem item);

    protected async Task<List<TResultItem>> MapRecords(DbCommand command)
    {
        await using var reader = await command.ExecuteReaderAsync();
        var collection = new List<TResultItem>();
        while (await reader.ReadAsync())
        {
            var item = new TResultItem();
            MapRecord(reader, item);
            collection.Add(item);
        }

        return collection;
    }
}

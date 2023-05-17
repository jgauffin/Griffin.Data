using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Griffin.Data.Helpers;
using Griffin.Data.Mapper;

namespace Griffin.Data.Queries;

/// <summary>
///     Extensions for making it easier to use queries.
/// </summary>
public static class QueryExtensions
{
    /// <summary>
    ///     Executes the reader all builds a collection from all rows.
    /// </summary>
    /// <param name="command">Command that is ready to be executed.</param>
    /// <param name="itemFiller">Callback used to assign values to the entity properties.</param>
    /// <returns>Collection of entities.</returns>
    public static async Task<List<TResultItem>> GenerateQueryResult<TResultItem>(
        this DbCommand command,
        Action<IDataRecord, TResultItem> itemFiller) where TResultItem : new()
    {
        try
        {
            await using var reader = await command.ExecuteReaderAsync();
            var collection = new List<TResultItem>();
            while (await reader.ReadAsync())
            {
                var item = new TResultItem();
                itemFiller(reader, item);
                collection.Add(item);
            }

            return collection;
        }
        catch (GriffinException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new MapperException($"Failed to create query result of items type {typeof(TResultItem)}.",
                command, typeof(TResultItem), ex);
        }
    }
}

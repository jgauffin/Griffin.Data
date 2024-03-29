﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Griffin.Data.Mapper;

namespace Griffin.Data.Queries;

/// <summary>
///     Runner used to fetch a result set.
/// </summary>
/// <typeparam name="TResultItem">Type if entity that the record set should be applied to.</typeparam>
public abstract class ListHandler<TResultItem>
    where TResultItem : new()

{
    /// <summary>
    /// </summary>
    /// <param name="session">Session to use.</param>
    /// <exception cref="ArgumentNullException">session is null.</exception>
    protected ListHandler(QuerySession session)
    {
        Session = session ?? throw new ArgumentNullException(nameof(session));
    }

    /// <summary>
    ///     Session to use.
    /// </summary>
    protected QuerySession Session { get; }

    /// <summary>
    ///     Map a row to a specific entity.
    /// </summary>
    /// <param name="record">Row to use.</param>
    /// <param name="item">Item to assign values to.</param>
    protected abstract void MapRecord(IDataRecord record, TResultItem item);

    /// <summary>
    ///     Executes the reader all builds a collection from all rows.
    /// </summary>
    /// <param name="command">Command that is ready to be executed.</param>
    /// <returns>Collection of entities.</returns>
    protected async Task<List<TResultItem>> MapRecords(DbCommand command)
    {
        try
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
        catch (GriffinException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new MapperException($"Failed to fetch records of type {typeof(TResultItem)}.", command, typeof(TResultItem), ex);
        }
    }
}

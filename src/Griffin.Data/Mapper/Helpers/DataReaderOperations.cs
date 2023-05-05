using System;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Griffin.Data.Mapper.Mappings;

namespace Griffin.Data.Mapper.Helpers;

internal static class DataReaderOperations
{
    /// <summary>
    ///     Map a record to an entity.
    /// </summary>
    /// <typeparam name="T">Type of entity.</typeparam>
    /// <param name="record">Data record (db row)</param>
    /// <param name="entity">Entity to assign values to.</param>
    /// <param name="mapping">Mapping used to lookup columns/properties.</param>
    /// <exception cref="ArgumentNullException">Any of the fields are null.</exception>
    public static void Map<T>(this IDataRecord record, [DisallowNull] T entity, ClassMapping mapping)
    {
        if (record == null)
        {
            throw new ArgumentNullException(nameof(record));
        }

        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        if (mapping == null)
        {
            throw new ArgumentNullException(nameof(mapping));
        }

        var values = new object[record.FieldCount];
        record.GetValues(values);
        var names = new string[record.FieldCount];
        for (var i = 0; i < record.FieldCount; i++)
        {
            names[i] = record.GetName(i);
        }

        foreach (var property in mapping.Keys)
        {
            property.MapRecord(record, entity);
        }

        foreach (var property in mapping.Properties)
        {
            property.MapRecord(record, entity);
        }
    }

    /// <summary>
    ///     Map a record to an entity.
    /// </summary>
    /// <typeparam name="T">Type of entity.</typeparam>
    /// <param name="reader">Record reader</param>
    /// <param name="addMethod">Method to invoke for each generated entity.</param>
    /// <param name="mapping">Mapping used to lookup columns/properties.</param>
    /// <exception cref="ArgumentNullException">Any of the fields are null.</exception>
    public static async Task MapAll<T>(this DbDataReader reader, ClassMapping mapping, Action<T> addMethod)
    {
        if (mapping == null)
        {
            throw new ArgumentNullException(nameof(mapping));
        }

        //TODO: Create a list of property accessors here
        // and loop through them using the int index in the record
        // for faster processing.
        //for (var i = 0; i < reader.FieldCount; i++) names[i] = reader.GetName(i);

        while (await reader.ReadAsync())
        {
            var entity = mapping.CreateInstance(reader);
            addMethod((T)entity);

            //var values = new object[reader.FieldCount];
            //reader.GetValues(values);

            foreach (var property in mapping.Keys)
            {
                property.MapRecord(reader, entity);
            }

            foreach (var property in mapping.Properties)
            {
                property.MapRecord(reader, entity);
            }
        }
    }

    /// <summary>
    ///     Map a record to an entity.
    /// </summary>
    /// <param name="reader">Record reader</param>
    /// <param name="addMethod">Method to invoke for each generated entity.</param>
    /// <param name="mapping">Mapping used to lookup columns/properties.</param>
    /// <param name="itemFactory">Factory which can be used to create sub classes for each given row.</param>
    /// <exception cref="ArgumentNullException">Any of the fields are null.</exception>
    public static async Task MapAll(
        this DbDataReader reader,
        ClassMapping mapping,
        Action<object> addMethod,
        Func<IDataRecord, object>? itemFactory = null)
    {
        if (mapping == null)
        {
            throw new ArgumentNullException(nameof(mapping));
        }

        if (addMethod == null)
        {
            throw new ArgumentNullException(nameof(addMethod));
        }

        //TODO: Create a list of property accessors here
        // and loop through them using the int index in the record
        // for faster processing.
        //for (var i = 0; i < reader.FieldCount; i++) names[i] = reader.GetName(i);

        var factory = itemFactory ?? mapping.CreateInstance;

        while (await reader.ReadAsync())
        {
            var entity = factory(reader);

            //var values = new object[reader.FieldCount];
            //reader.GetValues(values);

            foreach (var property in mapping.Keys)
            {
                property.MapRecord(reader, entity);
            }

            foreach (var property in mapping.Properties)
            {
                property.MapRecord(reader, entity);
            }

            addMethod(entity);
        }
    }
}

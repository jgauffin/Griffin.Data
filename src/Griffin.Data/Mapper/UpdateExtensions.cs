using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading.Tasks;
using Griffin.Data.Mappings;

namespace Griffin.Data.Mapper;

/// <summary>
///     Extensions used to update a single entity.
/// </summary>
/// <remarks>
///     <para>
///         Children are NOT updated since they can have been added, updated or removed. Those changes cannot be detected
///         without change tracking. Therefore, changes for children must always be explicitly done (either through the
///         built in change tracker or by calling CRUD manually for each child).
///     </para>
/// </remarks>
public static class UpdateExtensions
{
    /// <summary>
    ///     Update an entity.
    /// </summary>
    /// <param name="session">Session to do updates in.</param>
    /// <param name="entity">Entity to </param>
    /// <param name="extraUpdateColumns">Columns which should be included in the SET part of the update statement.</param>
    /// <param name="extraDbConstraints">Columns which should be included in the WHERE part of the update statement.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <remarks>
    ///     <para>
    ///         Children are NOT updated since they can have been added, updated or removed. Those changes cannot be detected
    ///         without change tracking. Therefore, changes for children must always be explicitly done (either through the
    ///         built in change tracker or by calling CRUD manually for each child).
    ///     </para>
    /// </remarks>
    public static async Task Update(this Session session, object entity,
        IDictionary<string, object>? extraUpdateColumns = null,
        IDictionary<string, object>? extraDbConstraints = null)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        var mapping = session.GetMapping(entity.GetType());
        await using var command = session.CreateCommand();
        await session.UpdateEntity(mapping, entity, command, extraUpdateColumns, extraDbConstraints);
    }


    private static async Task UpdateEntity(this Session session, ClassMapping mapping, object entity, DbCommand command,
        IDictionary<string, object>? extraUpdateColumns, IDictionary<string, object>? extraDbConstraints)
    {
        var columns = "";
        var where = "";

        foreach (var key in mapping.Keys)
        {
            var value = key.GetColumnValue(entity);
            if (value == null)
                throw new MappingException(entity,
                    $"Property '{key.PropertyName}' is a key and may not be null.");

            where += $"{key.ColumnName} = @{key.PropertyName}, ";
            command.AddParameter(key.PropertyName, value);
        }

        foreach (var property in mapping.Properties)
        {
            var value = property.GetColumnValue(entity);
            if (value == null) continue;

            columns += $"{property.ColumnName} = @{property.PropertyName}, ";
            command.AddParameter(property.PropertyName, value);
        }

        if (extraUpdateColumns != null)
            foreach (var extraColumn in extraUpdateColumns)
            {
                columns += $"{extraColumn.Key} = @{extraColumn.Key}, ";
                command.AddParameter(extraColumn.Key, extraColumn.Value);
            }

        if (extraDbConstraints != null)
            foreach (var extraColumn in extraDbConstraints)
            {
                where += $"{extraColumn.Key} = @{extraColumn.Key}, ";
                command.AddParameter(extraColumn.Key, extraColumn.Value);
            }

        columns = columns.Remove(columns.Length - 2, 2);
        where = where.Remove(where.Length - 2, 2);

        command.CommandText = $"UPDATE {mapping.TableName} SET {columns} WHERE {where};";
        await session.Dialect.Update(mapping, entity, command);
    }
}
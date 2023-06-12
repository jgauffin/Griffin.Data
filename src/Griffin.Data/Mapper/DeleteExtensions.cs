using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Griffin.Data.Helpers;
using Griffin.Data.Logging;
using Griffin.Data.Mapper.Mappings;
using Griffin.Data.Mapper.Mappings.Relations;

namespace Griffin.Data.Mapper;

/// <summary>
///     Methods to delete entities.
/// </summary>
/// <remarks>
///     <para>
///         All child entities are deleted in reverse order so that foreign keys are removed first.
///     </para>
/// </remarks>
public static class DeleteExtensions
{
    /// <summary>
    ///     Delete a single entity.
    /// </summary>
    /// <param name="session">Session to delete in.</param>
    /// <param name="entity">Entity to delete.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">Any of the arguments was not specified.</exception>
    public static async Task Delete(this Session session, object entity)
    {
        if (session == null)
        {
            throw new ArgumentNullException(nameof(session));
        }

        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        var mapping = session.GetMapping(entity.GetType());

        await DeleteChildren(session, entity, mapping);

        await using var command = session.CreateCommand();
        try
        {
            command.CommandText = $"DELETE FROM {mapping.TableName}";
            command.ApplyKeyWhere(mapping, entity);
            Log.Crud(command);
            await command.ExecuteNonQueryAsync();
        }
        catch (DbException ex)
        {
            throw new MapperException("Failed to DELETE entity", command, entity, ex);
        }
    }

    /// <summary>
    ///     Delete a single entity.
    /// </summary>
    /// <param name="session">Session to delete in.</param>
    /// <param name="key"></param>
    /// <returns>task.</returns>
    /// <exception cref="ArgumentNullException">Any of the arguments was not specified.</exception>
    public static async Task DeleteByKey<T>(this Session session, object key)
    {
        if (session == null)
        {
            throw new ArgumentNullException(nameof(session));
        }

        if (key == null)
        {
            throw new ArgumentNullException(nameof(key));
        }

        var mapping = session.GetMapping<T>();

        if (mapping.Keys.Count != 1)
        {
            throw new MappingException(mapping.EntityType, "This method requires a single key.");
        }

        var keyProperty = mapping.Keys[0];

        foreach (var childMapping in mapping.Children.Reverse())
        {
            await session.DeleteHasOneChildren(mapping, childMapping);
        }

        foreach (var childMapping in mapping.Collections.Reverse())
        {
            await session.DeleteHasManyChildren(mapping, childMapping);
        }

        await using var command = session.CreateCommand();
        command.CommandText =
            $"DELETE FROM {mapping.TableName} WHERE {keyProperty.ColumnName} = @{keyProperty.PropertyName}";
        command.AddParameter(keyProperty.PropertyName, key);
        Log.Crud(command);

        try
        {
            await command.ExecuteNonQueryAsync();
        }
        catch (DbException ex)
        {
            var our = new MapperException("Failed to DELETE entity", command, typeof(T), ex)
            {
                Data = { ["Key"] = key }
            };
            throw our;
        }
    }

    private static async Task DeleteChildren(Session session, object entity, ClassMapping mapping)
    {
        foreach (var childMapping in mapping.Children.Reverse())
        {
            await session.DeleteHasOneChildren(entity, childMapping);
        }

        foreach (var childMapping in mapping.Collections.Reverse())
        {
            await session.DeleteHasManyChildren(entity, childMapping);
        }
    }

    private static async Task DeleteHasOneChildren(this Session session, object parentEntity, IHasOneMapping hasOne)
    {
        if (session == null)
        {
            throw new ArgumentNullException(nameof(session));
        }

        if (parentEntity == null)
        {
            throw new ArgumentNullException(nameof(parentEntity));
        }

        if (hasOne == null)
        {
            throw new ArgumentNullException(nameof(hasOne));
        }

        var entity = hasOne.GetColumnValue(parentEntity);
        if (entity == null)
        {
            return;
        }

        // We must use the concrete type since the configured data type might be a base class.
        var mapping = session.GetMapping(entity.GetType());

        if (mapping.Children.Any() || mapping.Collections.Any())
        {
            var instance = hasOne.GetColumnValue(parentEntity);
            if (instance == null)
            {
                return;
            }

            await DeleteChildren(session, entity, mapping);
        }

        await using var command = session.CreateCommand();
        command.CommandText =
            $"DELETE FROM {mapping.TableName} WHERE {hasOne.ForeignKeyColumnName} = @{hasOne.ForeignKeyColumnName}";

        if (hasOne.SubsetColumn != null)
        {
            command.CommandText += $" AND {hasOne.SubsetColumn.Value.Key}=@subsetValue";
            command.AddParameter("subsetValue", hasOne.SubsetColumn.Value.Value);
        }

        var fkValue = hasOne.GetReferencedId(parentEntity);
        command.AddParameter(hasOne.ForeignKeyColumnName, fkValue);
        Log.Crud(command);

        try
        {
            await command.ExecuteNonQueryAsync();
        }
        catch (DbException ex)
        {
            throw new MapperException("Failed to DELETE entity.", command, entity, ex);
        }
    }

    /// <summary>
    ///     Delete a child collection.
    /// </summary>
    /// <param name="session"></param>
    /// <param name="parentEntity"></param>
    /// <param name="hasMany"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <remarks>
    ///     <para>
    ///         But before we can delete it, we must check so that the elements do not have any children. If they do, they must
    ///         be deleted first.
    ///     </para>
    /// </remarks>
    private static async Task DeleteHasManyChildren(this Session session, object parentEntity, IHasManyMapping hasMany)
    {
        if (session == null)
        {
            throw new ArgumentNullException(nameof(session));
        }

        if (parentEntity == null)
        {
            throw new ArgumentNullException(nameof(parentEntity));
        }

        if (hasMany == null)
        {
            throw new ArgumentNullException(nameof(hasMany));
        }
        
        var mapping = session.GetMapping(hasMany.ChildEntityType);
        var collection = hasMany.GetCollection(parentEntity);

        if ((mapping.Collections.Any() || mapping.Children.Any()) && collection != null)
        {
            await hasMany.Visit(collection, async element =>
            {
                foreach (var child in mapping.Children.Reverse())
                {
                    var value = child.GetColumnValue(element);
                    if (value == null)
                    {
                        continue;
                    }

                    // Action
                    await session.DeleteHasOneChildren(element, child);
                }

                foreach (var childHasMany in mapping.Collections.Reverse())
                {
                    var value = childHasMany.GetCollection(element);
                    if (value == null)
                    {
                        continue;
                    }

                    await session.DeleteHasManyChildren(value, childHasMany);
                }
            });
        }

        await using var command = session.CreateCommand();
        command.CommandText =
            $"DELETE FROM {mapping.TableName} WHERE {hasMany.ForeignKeyColumnName} = @{hasMany.ForeignKeyColumnName}";

        if (hasMany.SubsetColumn != null)
        {
            command.CommandText += $" AND {hasMany.SubsetColumn.Value.Key}=@subsetValue";
            command.AddParameter("subsetValue", hasMany.SubsetColumn.Value.Value);
        }

        var fkValue = hasMany.GetReferencedId(parentEntity);
        command.AddParameter(hasMany.ForeignKeyColumnName, fkValue);
        Log.Crud(command);
        try
        {
            await command.ExecuteNonQueryAsync();
        }
        catch (DbException ex)
        {
            var our = new MapperException("Failed to DELETE child entities using FK from parent.", command,
                hasMany.ChildEntityType, ex) { Data = { ["ParentEntity"] = parentEntity } };
            throw our;
        }
    }
}

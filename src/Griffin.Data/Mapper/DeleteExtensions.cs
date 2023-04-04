using System;
using System.Linq;
using System.Threading.Tasks;
using Griffin.Data.Mappings.Relations;

namespace Griffin.Data.Mapper;

/// <summary>
/// Methods to delete entities.
/// </summary>
internal static class DeleteExtensions
{
    /// <summary>
    /// Delete a single entity.
    /// </summary>
    /// <param name="session">Session to delete in.</param>
    /// <param name="entity">Entity to delete.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">Any of the arguments was not specified.</exception>
    public static async Task Delete(this Session session, object entity)
    {
        if (session == null) throw new ArgumentNullException(nameof(session));
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        var mapping = session.GetMapping(entity.GetType());

        foreach (var childMapping in mapping.Children) await session.DeleteChildren(entity, childMapping);
        foreach (var childMapping in mapping.Collections) await session.DeleteCollection(entity, childMapping);

        await using var command = session.CreateCommand();
        command.CommandText = $"DELETE FROM {mapping.TableName}";
        command.ApplyKeyWhere(mapping, entity);
        await command.ExecuteNonQueryAsync();
    }

    /// <summary>
    /// Delete a single entity.
    /// </summary>
    /// <param name="session">Session to delete in.</param>
    /// <param name="key"></param>
    /// <returns>task.</returns>
    /// <exception cref="ArgumentNullException">Any of the arguments was not specified.</exception>
    public static async Task DeleteByKey<T>(this Session session, object key)
    {
        if (session == null) throw new ArgumentNullException(nameof(session));
        if (key == null) throw new ArgumentNullException(nameof(key));

        var mapping = session.GetMapping<T>();

        if (mapping.Keys.Count != 1)
        {
            throw new MappingException(mapping.EntityType, "This method requires a single key.");
        }

        var keyProperty = mapping.Keys[0];

        foreach (var childMapping in mapping.Children) await session.DeleteChildren(mapping, childMapping);
        foreach (var childMapping in mapping.Collections) await session.DeleteCollection(mapping, childMapping);

        await using var command = session.CreateCommand();
        command.CommandText = $"DELETE FROM {mapping.TableName} WHERE {keyProperty.ColumnName} = @{keyProperty.PropertyName}";
        command.AddParameter(keyProperty.PropertyName, key);
        await command.ExecuteNonQueryAsync();
    }

    private static async Task DeleteChildren(this Session session, object parentEntity, IHasOneMapping hasOne)
    {
        if (session == null) throw new ArgumentNullException(nameof(session));
        if (parentEntity == null) throw new ArgumentNullException(nameof(parentEntity));
        if (hasOne == null) throw new ArgumentNullException(nameof(hasOne));

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

            foreach (var child in mapping.Children)
            {
                await session.DeleteChildren(instance, child);
            }

            foreach (var collection in mapping.Collections)
            {
                await session.DeleteCollection(instance, collection);
            }
        }


        await using var command = session.CreateCommand();
        command.CommandText =
            $"DELETE FROM {mapping.TableName} WHERE {hasOne.ForeignKeyColumnName} = @{hasOne.ForeignKeyColumnName}";

        var fkValue = hasOne.GetReferencedId(parentEntity);
        command.AddParameter(hasOne.ForeignKeyColumnName, fkValue);

        await command.ExecuteNonQueryAsync();
    }


    /// <summary>
    /// Delete a child collection.
    /// </summary>
    /// <param name="session"></param>
    /// <param name="parentEntity"></param>
    /// <param name="hasMany"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <remarks>
    ///<para>
    ///But before we can delete it, we must check so that the elements do not have any children. If they do, they must be deleted first.
    /// </para>
    /// </remarks>
    private static async Task DeleteCollection(this Session session, object parentEntity, IHasManyMapping hasMany)
    {
        if (session == null) throw new ArgumentNullException(nameof(session));
        if (parentEntity == null) throw new ArgumentNullException(nameof(parentEntity));
        if (hasMany == null) throw new ArgumentNullException(nameof(hasMany));

        var mapping = session.GetMapping(hasMany.ChildEntityType);
        var collection = hasMany.GetColumnValue(parentEntity);

        if ((mapping.Collections.Any() || mapping.Children.Any()) && collection != null)
        {
            await hasMany.Visit(collection, async element =>
            {
                foreach (var child in mapping.Children)
                {
                    var value = child.GetColumnValue(element);
                    if (value == null)
                    {
                        continue;
                    }

                    // Action
                    await session.DeleteChildren(element, child);
                }

                foreach (var childHasMany in mapping.Collections)
                {
                    var value = childHasMany.GetColumnValue(element);
                    if (value == null)
                    {
                        continue;
                    }


                    await session.DeleteCollection(value, childHasMany);
                }
            });
        }

        

        await using var command = session.CreateCommand();
        command.CommandText =
            $"DELETE FROM {mapping.TableName} WHERE {hasMany.ForeignKeyColumnName} = @{hasMany.ForeignKeyColumnName}";

        var fkValue = hasMany.GetReferencedId(parentEntity);
        command.AddParameter(hasMany.ForeignKeyColumnName, fkValue);

        await command.ExecuteNonQueryAsync();
    }
}
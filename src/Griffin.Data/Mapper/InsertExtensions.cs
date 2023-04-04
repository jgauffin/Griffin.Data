using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Griffin.Data.Mappings;

namespace Griffin.Data.Mapper;

/// <summary>
/// Functions used to insert entities to the DB.
/// </summary>
public static class InsertExtensions
{
    /// <summary>
    /// Insert an entity into the database.
    /// </summary>
    /// <param name="session">DB session.</param>
    /// <param name="entity">Entity to insert.</param>
    /// <param name="extraColumns">Extra columns which exists in the table but not in the class.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public static async Task Insert(this Session session, object entity,
        IDictionary<string, object>? extraColumns = null)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        var mapping = session.GetMapping(entity.GetType());
        await using var command = session.CreateCommand();
        await session.InsertEntity(mapping, entity, command);
        await session.InsertOneRelationShip(entity, mapping);
        await session.InsertManyRelationShip(entity, mapping);
    }

    private static async Task InsertEntity(this Session session, ClassMapping mapping, object entity, IDbCommand command)
    {
        if (session == null) throw new ArgumentNullException(nameof(session));
        if (mapping == null) throw new ArgumentNullException(nameof(mapping));
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        if (command == null) throw new ArgumentNullException(nameof(command));

        var columns = "";
        var values = "";

        foreach (var key in mapping.Keys)
        {
            if (key.IsAutoIncrement) continue;

            var value = key.GetColumnValue(entity);
            if (value == null)
            {
                var ex = new MappingException(entity,
                    $"Property '{key.PropertyName}' is a key and may not be null.");
                throw ex;
            }

            columns += $"{key.ColumnName}, ";
            values += $"@{key.PropertyName}, ";

            command.AddParameter(key.PropertyName, value);
        }

        foreach (var property in mapping.Properties)
        {
            var value = property.GetColumnValue(entity);
            if (value == null) continue;

            columns += $"{property.ColumnName}, ";
            values += $"@{property.PropertyName}, ";

            command.AddParameter(property.PropertyName, value);
        }

        columns = columns.Remove(columns.Length - 2, 2);
        values = values.Remove(values.Length - 2, 2);

        command.CommandText = $"INSERT INTO {mapping.TableName} ({columns}) VALUES({values});";
        await session.Dialect.Insert(mapping, entity, command);
    }

    private static async Task InsertOneRelationShip(this Session session, object parent, ClassMapping parentMapping)
    {
        foreach (var childMapping in parentMapping.Children)
        {
            var childEntity = childMapping.GetColumnValue(parent);
            if (childEntity == null) continue;

            var referencedValue = childMapping.GetReferencedId(parent);
            if (referencedValue == null) throw new MappingException(parent,
                $"Referenced property did not have a value. Cannot create child of type '{childMapping.ChildEntityType}'.");

            var extraColumns = new Dictionary<string, object>();
            if (childMapping.SubsetColumn != null)
            {
                extraColumns.Add(childMapping.SubsetColumn.Value.Key, childMapping.SubsetColumn.Value.Value);
            }

            if (childMapping.HasForeignKeyProperty)
            {
                childMapping.SetForeignKey(childEntity, referencedValue);
            }
            else
            {
                extraColumns.Add(childMapping.ForeignKeyColumnName, referencedValue);
            }

            await session.Insert(childEntity, extraColumns);
        }
    }

    private static async Task InsertManyRelationShip(this Session session, object parentEntity,
        ClassMapping parentMapping)
    {
        foreach (var childMapping in parentMapping.Collections)
        {
            var childCollection = childMapping.GetColumnValue(parentEntity);
            if (childCollection == null) continue;

            var referencedId = childMapping.GetReferencedId(parentEntity);

            await childMapping.Visit(childCollection, async childEntity =>
            {
                if (referencedId == null)
                {
                    await session.Insert(childEntity);
                }
                else if (childMapping.HasForeignKeyProperty)
                {
                    childMapping.SetForeignKey(childEntity, referencedId);
                    await session.Insert(childEntity);
                }
                else
                {
                    await session.Insert(childEntity,
                        new Dictionary<string, object> { { childMapping.ForeignKeyColumnName, referencedId } });
                }
            });
        }
    }
}
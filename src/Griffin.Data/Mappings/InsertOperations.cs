using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Griffin.Data.Dialects;

namespace Griffin.Data.Mappings;

public class InsertOperations
{
    private readonly IMappingRegistry _mappingRegistry;
    private readonly ISqlDialect _dialect;

    public InsertOperations(IMappingRegistry mappingRegistry, ISqlDialect dialect)
    {
        _mappingRegistry = mappingRegistry;
        _dialect = dialect;
    }

    public async Task Insert(IDbTransaction transaction, object entity,
        IDictionary<string, object>? extraColumns = null)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        var mapping = _mappingRegistry.Get(entity.GetType());
        await using var command = transaction.CreateCommand();
        await InsertEntity(mapping, entity, command);
        await InsertOneRelationShip(transaction, entity, mapping);
        await InsertManyRelationShip(transaction, entity, mapping);
    }

    private async Task InsertEntity(ClassMapping mapping, object entity, DbCommand command)
    {
        var columns = "";
        var values = "";

        foreach (var key in mapping.Keys)
        {
            if (key.IsAutoIncrement) continue;

            var value = key.GetColumnValue(entity);
            if (value == null)
            {
                var ex = new InvalidOperationException(
                    $"Property '{key.PropertyName}' is a key and may not be null.")
                {
                    Data =
                    {
                        ["entity"] = entity
                    }
                };
                throw ex;
            }

            columns += $"{key.ColumnName}, ";
            values += $"@{key.PropertyName}, ";

            command.AddParameter(key.PropertyName, value);
        }

        foreach (var property in mapping.Properties)
        {
            columns += $"{property.ColumnName}, ";
            values += $"@{property.PropertyName}, ";
            var value = property.GetColumnValue(entity);
            if (value == null) continue;

            command.AddParameter(property.PropertyName, value);
        }

        columns = columns.Remove(columns.Length - 2, 2);
        values = values.Remove(columns.Length - 2, 2);

        command.CommandText = $"INSERT INTO {mapping.TableName} ({columns}) VALUES(${values});";
        await _dialect.Insert(mapping, entity, command);
    }

    private async Task InsertOneRelationShip(IDbTransaction transaction, object entity, ClassMapping parentMapping)
    {
        foreach (var childMapping in parentMapping.Children)
        {
            var childEntity = childMapping.GetColumnValue(entity);
            if (childEntity == null) continue;

            var parentKeyProperty = parentMapping.GetProperty(childMapping.ForeignKey.ReferencedPropertyName);
            var keyValue = parentKeyProperty.GetColumnValue(entity);
            if (keyValue == null) throw new InvalidOperationException("Cannor");


            if (childMapping.ForeignKey.ForeignKeyColumnName == null)
            {
                childMapping.ForeignKey.SetColumnValue(childEntity, keyValue);
                await Insert(transaction, childEntity);
            }
            else
            {
                await Insert(transaction, childEntity,
                    new Dictionary<string, object> { { childMapping.ForeignKey.ForeignKeyColumnName, keyValue } });
            }
        }
    }

    private async Task InsertManyRelationShip(IDbTransaction transaction, object entity, ClassMapping parentMapping)
    {
        foreach (var propertyMapping in parentMapping.Collections)
        {
            var childEntity = propertyMapping.GetColumnValue(entity);
            if (childEntity == null) continue;

            var parentKeyProperty = parentMapping.GetProperty(propertyMapping.ForeignKey.ReferencedPropertyName);
            var keyValue = parentKeyProperty.GetColumnValue(entity);
            if (keyValue == null) throw new InvalidOperationException("Cannor");

            await propertyMapping.Visit(childEntity, async item =>
            {
                if (propertyMapping.ForeignKey.ForeignKeyColumnName == null)
                {
                    propertyMapping.ForeignKey.SetColumnValue(childEntity, keyValue);
                    await Insert(transaction, childEntity);
                }
                else
                {
                    await Insert(transaction, childEntity,
                        new Dictionary<string, object>
                            { { propertyMapping.ForeignKey.ForeignKeyColumnName, keyValue } });
                }
            });
        }
    }
}
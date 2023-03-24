using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Griffin.Data.Dialects;

namespace Griffin.Data.Mappings;

public class UpdateOperations
{
    private readonly ISqlDialect _dialect;
    private readonly IMappingRegistry _mappingRegistry;

    public UpdateOperations(IMappingRegistry mappingRegistry, ISqlDialect dialect)
    {
        _mappingRegistry = mappingRegistry;
        _dialect = dialect;
    }

    public async Task Update(IDbTransaction transaction, object entity,
        IDictionary<string, object>? extraColumns = null)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        var mapping = _mappingRegistry.Get(entity.GetType());
        await using var command = transaction.CreateCommand();
        await UpdateEntity(mapping, entity, command);
    }


    private async Task UpdateEntity(ClassMapping mapping, object entity, DbCommand command)
    {
        var columns = "";
        var where = "";

        foreach (var key in mapping.Keys)
        {
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

        columns = columns.Remove(columns.Length - 2, 2);
        where = where.Remove(columns.Length - 2, 2);

        command.CommandText = $"UPDATE {mapping.TableName} SET {columns} WHERE ${where};";
        await _dialect.Update(mapping, entity, command);
    }
}

public class DeleteOperations
{
    private readonly ISqlDialect _dialect;
    private readonly IMappingRegistry _mappingRegistry;

    public DeleteOperations(IMappingRegistry mappingRegistry, ISqlDialect dialect)
    {
        _mappingRegistry = mappingRegistry;
        _dialect = dialect;
    }

    public async Task Delete(IDbTransaction transaction, object entity,
        IDictionary<string, object>? extraColumns = null)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));

        var mapping = _mappingRegistry.Get(entity.GetType());
        await using var command = transaction.CreateCommand();
        command.CommandText = $"DELETE FROM {mapping.TableName} WHERE ";

        var keys = "";
        foreach (var key in mapping.Keys)
        {
            keys += $"{key.ColumnName} = @{key.PropertyName}";
            var value = key.GetColumnValue(entity);
            if (value == null) throw new InvalidOperationException("kddkd");

            command.AddParameter(key.PropertyName, value);
        }

        keys = keys.Remove(keys.Length - 2, 2);
        command.CommandText += keys;
        await command.ExecuteNonQueryAsync();
    }

    private async Task InsertOneRelationShip(IDbTransaction transaction, object entity, ClassMapping parentMapping)
    {
        foreach (var childMapping in parentMapping.Children)
        {
            var childEntity = childMapping.GetColumnValue(entity);
            if (childEntity == null) continue;

            var mapping = _mappingRegistry.Get(childEntity.GetType());
            await using var command = transaction.CreateCommand();
            command.CommandText = $"DELETE FROM {mapping.TableName} WHERE ";

            var keys = "";
            foreach (var key in mapping.Keys)
            {
                keys += $"{key.ColumnName} = @{key.PropertyName}";
                var value = key.GetColumnValue(entity);
                if (value == null) throw new InvalidOperationException("kddkd");

                command.AddParameter(key.PropertyName, value);
            }

            keys = keys.Remove(keys.Length - 2, 2);
            command.CommandText += keys;
            await command.ExecuteNonQueryAsync();
        }
    }

    private async Task InsertManyRelationShip(IDbTransaction transaction, object entity, ClassMapping parentMapping)
    {
    }
}
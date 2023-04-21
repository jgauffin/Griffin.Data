using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Griffin.Data.Helpers;
using Griffin.Data.Mappings;

namespace Griffin.Data.Mapper.Helpers;

internal static class SessionExtensions
{
    public static DbCommand CreateQueryCommand(this Session session, Type entityType, QueryOptions options)
    {
        var mapping = session.GetMapping(entityType);

        var sql = options.Sql ?? "";
        var cmd = session.Transaction.CreateCommand();

        if (string.IsNullOrEmpty(sql))
        {
            cmd.CommandText = $"SELECT * FROM {mapping.TableName}";
        }
        else if (!sql.Contains("SELECT", StringComparison.OrdinalIgnoreCase))
        {
            cmd.CommandText = $"SELECT * FROM {mapping.TableName} WHERE " + sql;
        }
        else
        {
            cmd.CommandText = sql;
        }

        if (!cmd.CommandText.Contains("WHERE", StringComparison.OrdinalIgnoreCase) && options.Parameters != null)
        {
            if (options.Parameters.Count > 0)
            {
                cmd.ApplyConstraints(mapping, options.Parameters);
            }
        }
        else if (options.Parameters != null)
        {
            // the SQL statement contained our names which mean that we cannot modify them (i.e. change property to column names).
            foreach (var parameter in options.Parameters)
            {
                cmd.AddParameter(parameter.Key, parameter.Value);
            }
        }

        if (options.DbParameters != null)
        {
            if (string.IsNullOrEmpty(options.Sql))
            {
                cmd.AddWhereColumnsAndParameters(options.DbParameters);
            }
            else
            {
                foreach (var parameter in options.DbParameters)
                {
                    cmd.AddParameter(parameter.Key, parameter.Value);
                }
            }
        }

        session.Dialect.ApplyQueryOptions(mapping, cmd, options);
        return cmd;
    }

    [return: NotNull]
    public static async Task<TEntity> GetSingle<TEntity>(this DbCommand command, ClassMapping mapping)
        where TEntity : notnull
    {
        try
        {
            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                throw new EntityNotFoundException(typeof(TEntity), command);
            }

            var entity = (TEntity)mapping.CreateInstance(reader);
            reader.Map(entity, mapping);
            return entity;
        }
        catch (Exception ex)
        {
            throw command.CreateDetailedException(ex, typeof(TEntity));
        }
    }

    public static async Task<object> GetSingle(this DbCommand command, ClassMapping mapping)
    {
        try
        {
            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                throw new EntityNotFoundException(mapping.EntityType, command);
            }

            var entity = mapping.CreateInstance(reader);
            reader.Map(entity, mapping);
            return entity;
        }
        catch (Exception ex)
        {
            throw command.CreateDetailedException(ex, mapping.EntityType);
        }
    }

    public static async Task<object> GetSingle(this DbCommand command, ClassMapping mapping, QueryOptions options)
    {
        try
        {
            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                throw new EntityNotFoundException(mapping.EntityType, command);
            }

            var factory = options.Factory ?? mapping.CreateInstance;
            var entity = factory(reader);
            reader.Map(entity, mapping);
            return entity;
        }
        catch (Exception ex)
        {
            throw command.CreateDetailedException(ex, mapping.EntityType);
        }
    }

    public static async Task<TEntity?> GetSingleOrDefault<TEntity>(this DbCommand command, ClassMapping mapping)
    {
        try
        {
            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
            {
                return default;
            }

            var entity = (TEntity)mapping.CreateInstance(reader)!;
            reader.Map(entity, mapping);
            return entity;
        }
        catch (Exception ex)
        {
            throw command.CreateDetailedException(ex, typeof(TEntity));
        }
    }

    public static async Task<object?> GetSingleOrDefault(
        this DbCommand command,
        ClassMapping mapping,
        QueryOptions options)
    {
        try
        {
            await using var reader = await command.ExecuteReaderAsync();
            if (!await reader.ReadAsync())
            {
                return null;
            }

            var factory = options.Factory ?? mapping.CreateInstance;
            var entity = factory(reader);
            reader.Map(entity, mapping);
            return entity;
        }
        catch (Exception ex)
        {
            throw command.CreateDetailedException(ex, mapping.EntityType);
        }
    }

    private static void AddWhereColumnsAndParameters(this IDbCommand cmd, IDictionary<string, object> parameters)
    {
        if (parameters.Count == 0)
        {
            return;
        }

        var sql = " ";
        if (!cmd.CommandText.Contains("WHERE", StringComparison.OrdinalIgnoreCase))
        {
            sql += "WHERE ";
        }

        foreach (var kvp in parameters)
        {
            if (kvp.Value.GetType().IsCollection())
            {
                //TODO: support non digit types.
                var values = string.Join(", ", (IEnumerable<object>)kvp.Value);
                sql += $"{kvp.Key} IN ({values}) AND ";
            }
            else
            {
                sql += $"{kvp.Key} = @{kvp.Key} AND ";
                cmd.AddParameter(kvp.Key, kvp.Value);
            }
        }

        cmd.CommandText += sql.Remove(sql.Length - 4, 4);
    }
}

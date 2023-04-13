using System;
using System.Collections.Generic;
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
        var cmd = CommandExtensions.CreateCommand(session.Transaction);

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
                CommandExtensions.AddParameter(cmd, parameter.Key, parameter.Value);
            }
        }

        if (options.DbParameters != null)
        {
            cmd.AddConstraintDbParameters(options.DbParameters);
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
            throw command.CreateDetailedException(ex);
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
            throw command.CreateDetailedException(ex);
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
            throw command.CreateDetailedException(ex);
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
            throw command.CreateDetailedException(ex);
        }
    }

    public static async Task<object?> GetSingleOrDefault(
        this DbCommand command,
        ClassMapping mapping,
        QueryOptions options)
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

    private static void AddConstraintDbParameters(this DbCommand cmd, IDictionary<string, object> parameters)
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
                CommandExtensions.AddParameter(cmd, kvp.Key, kvp.Value);
            }
        }

        cmd.CommandText += sql.Remove(sql.Length - 4, 4);
    }
}

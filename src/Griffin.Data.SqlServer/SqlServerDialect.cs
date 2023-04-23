using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Griffin.Data.Dialects;
using Griffin.Data.Helpers;
using Griffin.Data.Mapper;
using Griffin.Data.Mapper.Implementation;
using Griffin.Data.Mappings;
using Griffin.Data.Queries;
using Griffin.Data.Scaffolding;

namespace Griffin.Data.SqlServer;

/// <summary>
///     Adapter for Microsoft SQL Server.
/// </summary>
[DbEngineName("mssql")]
public class SqlServerDialect : ISqlDialect
{
    /// <summary>
    ///     SQL Server engine name.
    /// </summary>
    public const string EngineName = "mssql";

    /// <inheritdoc />
    public IDbConnection CreateConnection()
    {
        return new SqlConnection();
    }

    /// <inheritdoc />
    public ISchemaReader CreateSchemaReader()
    {
        return new SqlServerSchemaReader();
    }

    /// <inheritdoc />
    public async Task Insert(ClassMapping mapping, object entity, IDbCommand command)
    {
        if (mapping == null)
        {
            throw new ArgumentNullException(nameof(mapping));
        }

        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        if (command == null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        var auto = mapping.Keys.FirstOrDefault(x => x.IsAutoIncrement);
        if (auto == null)
        {
            try
            {
                await ((DbCommand)command).ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                var our = command.CreateDetailedException(ex, entity.GetType());
                our.Data["Entity"] = entity;
                throw our;
            }

            return;
        }

        try
        {
            command.CommandText += ";SELECT cast(SCOPE_IDENTITY() as int);";
            var result = await ((DbCommand)command).ExecuteScalarAsync();
            if (result == null)
            {
                throw new MappingException(entity, "Could not get identity value after insert.");
            }

            auto.SetPropertyValue(entity, result);
        }
        catch (DbException ex)
        {
            var our = command.CreateDetailedException(ex, entity.GetType());
            our.Data["Entity"] = entity;
            throw our;
        }
    }

    /// <inheritdoc />
    public void ApplyPaging(IDbCommand command, string keyColumn, int pageNumber, int? pageSize)
    {
        if (command.CommandText.Contains("TOP("))
        {
            return;
        }

        var ps = pageSize ?? 100;
        if (pageNumber == 1)
        {
            var pos = command.CommandText.IndexOf("SELECT", StringComparison.OrdinalIgnoreCase);
            command.CommandText = command.CommandText.Insert(pos + 7, $"TOP({ps}) ");
        }
        else
        {
            // SELECT * FROM TableName ORDER BY id OFFSET 10 ROWS FETCH NEXT 10 ROWS ONLY;
            if (!command.CommandText.Contains("ORDER BY", StringComparison.OrdinalIgnoreCase))
            {
                command.CommandText += $" ORDER BY {keyColumn}";
            }

            command.CommandText +=
                $" OFFSET {ps * (pageNumber - 1)} ROWS FETCH NEXT {ps} ROWS ONLY";
        }
    }

    /// <inheritdoc />
    public void ApplySorting(IDbCommand command, IList<SortEntry> entries)
    {
        command.CommandText += " ORDER BY ";
        foreach (var sort in entries)
        {
            command.CommandText += $"{sort.Name} {(sort.IsAscending ? "ASC" : "DESC")}, ";
        }

        command.CommandText = command.CommandText.Remove(command.CommandText.Length - 2, 2);
    }

    /// <inheritdoc />
    public async Task Update(ClassMapping mapping, object entity, DbCommand command)
    {
        if (mapping == null)
        {
            throw new ArgumentNullException(nameof(mapping));
        }

        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        if (command == null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        try
        {
            await command.ExecuteNonQueryAsync();
        }
        catch (DbException ex)
        {
            var our = command.CreateDetailedException(ex, entity.GetType());
            our.Data["Entity"] = entity;
            throw our;
        }
    }

    /// <inheritdoc />
    public void ApplyQueryOptions(ClassMapping mapping, DbCommand command, QueryOptions options)
    {
        if (mapping == null)
        {
            throw new ArgumentNullException(nameof(mapping));
        }

        if (command == null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        if (options == null)
        {
            throw new ArgumentNullException(nameof(options));
        }

        if (options is ICanSort sorts && sorts.Sorts.Any() &&
            !command.CommandText.Contains("ORDER BY", StringComparison.OrdinalIgnoreCase))
        {
            command.CommandText += " ORDER BY ";
            foreach (var sort in sorts.Sorts)
            {
                var name = sort.IsPropertyName ? mapping.GetProperty(sort.Name).ColumnName : sort.Name;
                command.CommandText += $"{name} {(sort.IsAscending ? "ASC" : "DESC")}, ";
            }

            command.CommandText = command.CommandText.Remove(command.CommandText.Length - 2, 2);
        }

        if (command.CommandText.Contains("TOP(", StringComparison.OrdinalIgnoreCase)
            || command.CommandText.Contains(" OFFSET "))
        {
            return;
        }

        if (options.PageNumber == 1)
        {
            var pos = command.CommandText.IndexOf("SELECT", StringComparison.OrdinalIgnoreCase);
            command.CommandText = command.CommandText.Insert(pos + 7, $"TOP({options.PageSize}) ");
        }
        else
        {
            // SELECT * FROM TableName ORDER BY id OFFSET 10 ROWS FETCH NEXT 10 ROWS ONLY;
            if (!command.CommandText.Contains("ORDER BY", StringComparison.OrdinalIgnoreCase))
            {
                command.CommandText += $" ORDER BY {mapping.Keys[0].ColumnName}";
            }

            command.CommandText +=
                $" OFFSET {options.PageSize * (options.PageNumber - 1)} ROWS FETCH NEXT {options.PageSize} ROWS ONLY";
        }
    }
}

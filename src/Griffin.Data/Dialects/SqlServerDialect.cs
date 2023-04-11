using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Griffin.Data.Mapper;
using Griffin.Data.Mappings;

namespace Griffin.Data.Dialects;

/// <summary>
///     Adapter for Microsoft SQL Server.
/// </summary>
public class SqlServerDialect : ISqlDialect
{
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
            await ((DbCommand)command).ExecuteNonQueryAsync();
            return;
        }

        try
        {
            command.CommandText += ";SELECT cast(SCOPE_IDENTITY() as int);";
            var result = await ((DbCommand)command).ExecuteScalarAsync();
            auto.SetPropertyValue(entity, result);
        }
        catch (DbException ex)
        {
            throw command.CreateDetailedException(ex);
        }
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

        await command.ExecuteNonQueryAsync();
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

        if (options.Sorts.Any())
        {
            command.CommandText += " ORDER BY ";
            foreach (var sort in options.Sorts)
            {
                var name = sort.IsPropertyName ? mapping.GetProperty(sort.Name).ColumnName : sort.Name;
                command.CommandText += $"{name} {(sort.IsAscending ? "ASC" : "DESC")}, ";
            }

            command.CommandText = command.CommandText.Remove(command.CommandText.Length - 2, 2);
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

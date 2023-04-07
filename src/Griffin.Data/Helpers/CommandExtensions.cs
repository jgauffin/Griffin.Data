using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;
using Griffin.Data.Helpers;
using Griffin.Data.Mappings;

namespace Griffin.Data;

/// <summary>
///     Helper extensions for commands.
/// </summary>
internal static class CommandExtensions
{
    public static void AddParameter(this IDbCommand cmd, string name, object? value)
    {
        var p = cmd.CreateParameter();
        p.ParameterName = name;
        p.Value = value ?? DBNull.Value;
        cmd.Parameters.Add(p);
    }

    /// <summary>
    ///     Apply where from an anonymous object.
    /// </summary>
    /// <param name="command">Command to apply WHERE statement to.</param>
    /// <param name="mapping">Mapping used to translate between properties and columns.</param>
    /// <param name="propertyConstraints">Property/Value pairs in an anonymous object.</param>
    public static void ApplyConstraints(this IDbCommand command, ClassMapping mapping, object propertyConstraints)
    {
        var sql = " WHERE ";
        var dict = propertyConstraints.ToDictionary();
        foreach (var pair in dict)
        {
            if (pair.Value == null)
            {
                throw new InvalidOperationException($"Constraint '{pair.Key}' does not a value.");
            }

            var columnName = pair.Key;
            var propertyName = pair.Key;

            var property = mapping.Properties.FirstOrDefault(x => x.PropertyName == pair.Key);
            if (property != null)
            {
                columnName = property.ColumnName;
                propertyName = property.PropertyName;
            }

            if (pair.Value.GetType().IsCollection())
            {
                var values = string.Join(", ", ((IEnumerable)pair.Value).Cast<object>());
                sql += $" {columnName} IN ({values}) AND";
            }
            else
            {
                sql += $" {columnName}=@{propertyName} AND";
                var value = pair.Value;
                if (property?.PropertyToColumnConverter != null)
                {
                    value = property.PropertyToColumnConverter(pair.Value);
                }

                AddParameter(command, propertyName, value);
            }
        }

        sql = sql.Remove(sql.Length - 4, 4);
        command.CommandText += sql;
    }

    /// <summary>
    ///     Read keys from the entity and apply them to the SQL statement ("WHERE" should not have been specified).
    /// </summary>
    /// <param name="command">Command to add "WHERE xxx" to.</param>
    /// <param name="mapping">Mapping used to translate between properties and columns.</param>
    /// <param name="entity">Entity to fetch information from.</param>
    public static void ApplyKeyWhere(this IDbCommand command, ClassMapping mapping, object entity)
    {
        var sql = " WHERE ";
        foreach (var key in mapping.Keys)
        {
            sql += $" {key.ColumnName}=@{key.PropertyName} AND";
            AddParameter(command, key.PropertyName, key.GetColumnValue(entity));
        }

        sql = sql.Remove(sql.Length - 4, 4);
        command.CommandText += sql;
    }

    /// <summary>
    ///     Create a new database command (and enlist it in the transaction).
    /// </summary>
    /// <param name="transaction">Transaction to create command on.</param>
    /// <returns>Created command.</returns>
    public static DbCommand CreateCommand(this IDbTransaction transaction)
    {
        if (transaction.Connection == null)
        {
            throw new InvalidOperationException(
                "The transaction has been committed. You may not use the session any more.");
        }

        var cmd = transaction.Connection!.CreateCommand();
        cmd.Transaction = transaction;
        return (DbCommand)cmd;
    }

    public static InvalidDataException CreateDetailedException(this IDbCommand command, Exception ex)
    {
        var ps = command.Parameters.Cast<IDataParameter>().Select(x => $"{x.ParameterName}={x.Value}");
        var e = new InvalidDataException(
            $"{ex.Message}\r\n  SQL: '{command.CommandText}'\r\n  Parameters: {string.Join(", ", ps)}", ex);
        return e;
    }

    /// <summary>
    ///     Convert an anonymous object to a dictionary.
    /// </summary>
    /// <param name="content">Object to convert.</param>
    /// <returns>dictionary.</returns>
    /// <exception cref="ArgumentNullException">Content is null.</exception>
    public static IDictionary<string, object> ToDictionary(this object content)
    {
        switch (content)
        {
            case null:
                throw new ArgumentNullException(nameof(content));
            case IDictionary<string, object> objects:
                return objects;
        }

        var props = content.GetType().GetProperties();
        var pairDictionary = props.ToDictionary(x => x.Name, x => x.GetValue(content, null));
        return pairDictionary;
    }
}

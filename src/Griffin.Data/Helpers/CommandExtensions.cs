using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Griffin.Data.Mapper.Mappings;

namespace Griffin.Data.Helpers;

/// <summary>
///     Helper extensions for commands.
/// </summary>
internal static class CommandExtensions
{
    /// <summary>
    ///     Adds "WHERE [options]" to the CommandText
    /// </summary>
    /// <param name="command">Command to apply WHERE statement to.</param>
    /// <param name="mapping">Mapping used to translate between properties and columns.</param>
    /// <param name="propertyConstraints">Property/Value pairs in an anonymous object.</param>
    internal static void ApplyConstraints(this IDbCommand command, ClassMapping mapping, object propertyConstraints)
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
                var value = property?.ConvertToColumnValue(pair.Value) ?? pair.Value;
                command.AddParameter(propertyName, value);
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
    internal static void ApplyKeyWhere(this IDbCommand command, ClassMapping mapping, object entity)
    {
        var sql = " WHERE ";
        foreach (var key in mapping.Keys)
        {
            sql += $" {key.ColumnName}=@{key.PropertyName} AND";
            command.AddParameter(key.PropertyName, key.GetColumnValue(entity));
        }

        sql = sql.Remove(sql.Length - 4, 4);
        command.CommandText += sql;
    }

    /// <summary>
    ///     Convert an anonymous object to a dictionary.
    /// </summary>
    /// <param name="content">Object to convert.</param>
    /// <returns>dictionary.</returns>
    /// <exception cref="ArgumentNullException">Content is null.</exception>
    internal static IDictionary<string, object> ToDictionary(this object content)
    {
        switch (content)
        {
            case null:
                throw new ArgumentNullException(nameof(content));
            case IDictionary<string, object> objects:
                return objects;
        }

        var props = content.GetType().GetProperties();
        var pairDictionary = props.ToDictionary(x => x.Name, x => x.GetValue(content, null)!);
        return pairDictionary;
    }
}

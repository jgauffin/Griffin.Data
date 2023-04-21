using System;

namespace Griffin.Data.Mappings.Properties;

/// <summary>
///     Mapping of a field.
/// </summary>
/// <remarks>
///     <para>
///         Base interface for when working with single value fields (i.e. property or a key).
///     </para>
/// </remarks>
public interface IFieldMapping : IFieldAccessor
{
    /// <summary>
    ///     Name of column.
    /// </summary>
    string ColumnName { get; }

    /// <summary>
    ///     Name of property.
    /// </summary>
    string PropertyName { get; }

    /// <summary>
    ///     Property type.
    /// </summary>
    public Type PropertyType { get; }

    /// <summary>
    ///     Convert a property value to a column value.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <returns>Converted value.</returns>
    object ConvertToColumnValue(object value);
}

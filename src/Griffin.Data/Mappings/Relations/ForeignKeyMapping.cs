using System;
using Griffin.Data.Configuration;
using Griffin.Data.Mappings.Properties;

namespace Griffin.Data.Mappings.Relations;

/// <summary>
///     Mapping for a has one/many FK.
/// </summary>
public class ForeignKeyMapping : IPropertyAccessor
{
    /// <summary>
    /// </summary>
    /// <param name="configuredType">Type that the has many/one configuration is for (that this FK is part of).</param>
    /// <param name="propertyName">FK property in the child table</param>
    /// <param name="columnName">Column name (specified when there are no FK property in the child table).</param>
    public ForeignKeyMapping(Type configuredType, string? propertyName = null, string? columnName = null)
    {
        if (configuredType == null) throw new ArgumentNullException(nameof(configuredType));
        ForeignKeyPropertyName = propertyName;
        ForeignKeyColumnName = columnName;
        if (columnName == null && propertyName == null) throw new MappingConfigurationException(configuredType, "FK must specify either property or column name in the child entity.");
    }

    /// <summary>
    ///     Property in the child entity that contains the FK.
    /// </summary>
    public IPropertyAccessor ForeignKey { get; set; } = null!;

    /// <summary>
    ///     Property in the parent table that the FK is referencing.
    /// </summary>
    public IPropertyAccessor ReferencedProperty { get; set; }=null!;

    /// <summary>
    ///     Column to insert the foreign key in.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Specified instead of the property if the child entity do not contain a property for the FK.
    ///     </para>
    /// </remarks>
    public string? ForeignKeyColumnName { get; set; }

    /// <summary>
    ///     Specified when the child table has a property for the FK.
    /// </summary>
    public string? ForeignKeyPropertyName { get; set; }

    /// <summary>
    ///     Specified when property binding is made.
    /// </summary>
    public string? ReferencedPropertyName { get; set; }

    /// <inheritdoc />
    public void SetColumnValue(object instance, object value)
    {
        if (instance == null) throw new ArgumentNullException(nameof(instance));
        if (value == null) throw new ArgumentNullException(nameof(value));
        ForeignKey.SetColumnValue(instance, value);
    }

    /// <inheritdoc />
    public object? GetColumnValue(object entity)
    {
        if (entity == null) throw new ArgumentNullException(nameof(entity));
        return ForeignKey.GetColumnValue(entity);
    }
}
using System;
using System.Data;
using Griffin.Data.Converters;
using Griffin.Data.Converters.Enums;
using Griffin.Data.Mappings.Properties;

namespace Griffin.Data.Configuration;

/// <summary>
///     Used to configure a property in a class mapping.
/// </summary>
/// <typeparam name="TEntity">Type of entity.</typeparam>
/// <typeparam name="TProperty">Type of property in the entity.</typeparam>
public class PropertyConfigurator<TEntity, TProperty> where TProperty : notnull
{
    private readonly PropertyMapping _mapping;

    /// <summary>
    /// </summary>
    /// <param name="mapping">Mapping to fill with information.</param>
    /// <exception cref="ArgumentNullException">mapping is null.</exception>
    public PropertyConfigurator(PropertyMapping mapping)
    {
        _mapping = mapping ?? throw new ArgumentNullException(nameof(mapping));
        if (typeof(TProperty).IsEnum)
        {
            Converter(new IntToEnum<TProperty>());
        }
    }

    /// <summary>
    ///     Specify column name (used when the name differs from the property name).
    /// </summary>
    /// <param name="name">Table column name.</param>
    /// <exception cref="ArgumentNullException">name is null.</exception>
    public PropertyConfigurator<TEntity, TProperty> ColumnName(string name)
    {
        _mapping.ColumnName = name ?? throw new ArgumentNullException(nameof(name));
        return this;
    }

    /// <summary>
    ///     Handle conversions between the column and property types.
    /// </summary>
    /// <typeparam name="TColumn">Column data type.</typeparam>
    /// <param name="converter">Converter to use.</param>
    /// <remarks>
    ///     <para>
    ///         The column type and the property type differs and there is no automatic conversion between them.
    ///     </para>
    /// </remarks>
    public void Converter<TColumn>(ISingleValueConverter<TColumn, TProperty> converter) where TColumn : notnull
    {
        if (converter == null)
        {
            throw new ArgumentNullException(nameof(converter));
        }

        _mapping.ColumnToPropertyConverter = columnValue => converter.ColumnToProperty((TColumn)columnValue);
        _mapping.PropertyToColumnConverter = propertyValue => converter.PropertyToColumn((TProperty)propertyValue);
    }

    /// <summary>
    ///     Specify column name (used when the name differs from the property name).
    /// </summary>
    /// <exception cref="ArgumentNullException">name is null.</exception>
    public PropertyConfigurator<TEntity, TProperty> Ignore()
    {
        _mapping.IsIgnored = true;
        return this;
    }

    /// <summary>
    ///     Converter used to convert multiple columns to the property.
    /// </summary>
    /// <param name="recordToPropertyConverter">Convert to use.</param>
    /// <exception cref="ArgumentNullException">The converter is null.</exception>
    /// <remarks>
    ///     <para>
    ///         Sometimes, more than one column is required to fetch a property value. For instance if you have serialized a
    ///         property you need to be able to tell the type. In that case, there might be a "DataType" and a "Data" columns
    ///         which both in combination is used to create the property value.
    ///     </para>
    /// </remarks>
    public void RecordToProperty(Func<IDataRecord, TProperty> recordToPropertyConverter)
    {
        if (recordToPropertyConverter == null)
        {
            throw new ArgumentNullException(nameof(recordToPropertyConverter));
        }

        _mapping.RecordToPropertyConverter = record => recordToPropertyConverter(record)!;
    }
}

using System;
using System.Data;
using Griffin.Data.Configuration;
using Griffin.Data.Converters;
using Griffin.Data.Converters.Enums;
using Griffin.Data.Mapper;

namespace Griffin.Data.Mappings.Properties;

/// <summary>
///     Maps a property, which are not in a relationship ("boho") and not a key.
/// </summary>
public class PropertyMapping<TEntity, TProperty> : IPropertyMapping, IGotColumnToPropertyConverter<TProperty>
{
    private readonly Type _entityType;
    private readonly Func<TEntity, TProperty?>? _getter;
    private readonly Action<TEntity, TProperty>? _setter;
    private bool _canReadFromDatabase;
    private bool _canWriteToDatabase;
    private bool _enumIsConfigured;

    /// <summary>
    /// </summary>
    /// <param name="propertyName"></param>
    /// <param name="getter"></param>
    /// <param name="setter"></param>
    public PropertyMapping(
        string propertyName,
        Func<TEntity, TProperty?>? getter,
        Action<TEntity, TProperty>? setter)
    {
        PropertyName = propertyName;
        PropertyType = typeof(TProperty);
        _entityType = typeof(TEntity);
        ColumnName = propertyName;
        _getter = getter;
        _setter = setter;
        CanWriteToDatabase = _getter != null;
        CanReadFromDatabase = _setter != null;
        var nullableType = Nullable.GetUnderlyingType(PropertyType);
        if (!PropertyType.IsEnum && nullableType?.IsEnum != true)
        {
            return;
        }

        PropertyToColumnConverter = x =>
        {
            if (x == null)
            {
                throw new MappingConfigurationException(_entityType,
                    "A converter should not get invoked for null values.");
            }

            if (!_enumIsConfigured)
            {
                return (int)(object)x;
            }

            return PropertyToColumnConverter!(x);
        };
        ColumnToPropertyConverter = x =>
        {
            if (x == null)
            {
                throw new MappingConfigurationException(_entityType,
                    "A converter should not get invoked for null values.");
            }

            SelectEnumConverter(x);
            return ColumnToPropertyConverter!(x);
        };
    }

    /// <summary>
    ///     Specifies if this property can be used when reading from the database.
    /// </summary>
    public bool CanReadFromDatabase
    {
        get => _canReadFromDatabase;
        set
        {
            if (value && _setter == null)
            {
                throw new MappingConfigurationException(_entityType,
                    $"Cannot mark property '${PropertyName}' as readable when there is no setter.");
            }

            _canReadFromDatabase = value;
        }
    }

    /// <summary>
    ///     Specifies if this property can be used in CRUD statements.
    /// </summary>
    public bool CanWriteToDatabase
    {
        get => _canWriteToDatabase;
        set
        {
            if (value && _getter == null)
            {
                throw new MappingConfigurationException(_entityType,
                    $"Cannot mark property '${PropertyName}' as writable when there is no getter.");
            }

            _canWriteToDatabase = value;
        }
    }

    /// <summary>
    ///     Converts from a column value to a property value.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Used when the type differ, otherwise <c>null</c>.
    ///     </para>
    /// </remarks>
    public Func<object, TProperty>? ColumnToPropertyConverter { get; set; }

    /// <summary>
    ///     Converter used when this property requires multiple columns to generate a property value.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         One use case if a child entity is serialized and stored as a column. In that case the child type is required
    ///         (and therefore stored in another column).
    ///     </para>
    /// </remarks>
    public IRecordToValueConverter<TProperty>? RecordToPropertyConverter { get; set; }

    /// <summary>
    ///     Converts from a property value to a column value.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Used when the type differ, otherwise <c>null</c>.
    ///     </para>
    /// </remarks>
    public Func<TProperty, object>? PropertyToColumnConverter { get; set; }

    /// <summary>
    ///     Type of property.
    /// </summary>
    public Type PropertyType { get; }

    /// <summary>
    ///     do not include in reads and writes. Remove once mapping is generated.
    /// </summary>
    public bool IsIgnored { get; set; }

    ///// <inheritdoc />
    //public void SetColumnValue([NotNull] object instance, object value)
    //{
    //    if (instance == null)
    //    {
    //        throw new ArgumentNullException(nameof(instance));
    //    }

    //    if (_setter == null)
    //    {
    //        throw new MappingException(instance,
    //            $"No setter has been defined for property ${PropertyName}.");
    //    }

    //    if (ColumnToPropertyConverter != null)
    //    {
    //        value = ColumnToPropertyConverter(value);
    //    }
    //    else if (_checkConverters)
    //    {
    //        _checkConverters = false;
    //    }

    //    //CreateConverters(value.GetType());
    //    _setter((TEntity)instance, (TProperty)value);
    //}

    /// <inheritdoc />
    public object? GetValue(object entity)
    {
        if (_getter == null)
        {
            return null;
        }

        return _getter.Invoke((TEntity)entity);
    }

    /// <inheritdoc />
    public object? GetColumnValue(object entity)
    {
        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        if (_getter == null)
        {
            throw new MappingException(entity,
                $"No getter has been defined for property ${PropertyName}.");
        }

        var propertyValue = _getter((TEntity)entity);
        return propertyValue == null ? null : ConvertToColumnValue(propertyValue);
    }

    void IFieldAccessor.SetPropertyValue(object instance, object value)
    {
        if (_setter == null)
        {
            throw new MappingException(typeof(TEntity), $"No setter for property '{PropertyName}'.");
        }

        _setter((TEntity)instance, (TProperty)value);
    }

    /// <inheritdoc />
    public void MapRecord(IDataRecord record, object entity)
    {
        if (record == null)
        {
            throw new ArgumentNullException(nameof(record));
        }

        if (entity == null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        if (_setter == null || !CanReadFromDatabase)
        {
            return;
        }

        if (RecordToPropertyConverter != null)
        {
            var generatedValue = RecordToPropertyConverter.Convert(record);
            _setter((TEntity)entity, generatedValue);
        }

        var value = record[ColumnName];
        if (value is DBNull or null)
        {
            return;
        }

        if (ColumnToPropertyConverter != null)
        {
            value = ColumnToPropertyConverter((TEntity)value)!;
        }

        _setter((TEntity)entity, (TProperty)value);
    }

    /// <inheritdoc />
    public string ColumnName { get; set; }

    /// <inheritdoc />
    public string PropertyName { get; set; }

    /// <inheritdoc />
    public object ConvertToColumnValue(object value)
    {
        return PropertyToColumnConverter != null ? PropertyToColumnConverter((TProperty)value) : value;
    }

    private void SelectEnumConverter(object value)
    {
        _enumIsConfigured = true;
        if (value is short)
        {
            var converter = new GenericToEnumConverter<short, TProperty>();
            PropertyToColumnConverter = x => converter.PropertyToColumn(x!);
            ColumnToPropertyConverter = x => converter.ColumnToProperty((short)x);
        }
        else if (value is byte)
        {
            var converter = new GenericToEnumConverter<byte, TProperty>();
            PropertyToColumnConverter = x => converter.PropertyToColumn(x!);
            ColumnToPropertyConverter = x => converter.ColumnToProperty((byte)x);
        }
        else if (value is string)
        {
            PropertyToColumnConverter = y => y?.ToString() ?? throw new MappingException(typeof(TEntity),
                $"Failed to convert property value '{y}'. (null values are not supported by converters)");
            ColumnToPropertyConverter = y =>
            {
                if (!Enum.TryParse(typeof(TProperty), (string)y, true, out var enumValue) || enumValue == null)
                {
                    throw new InvalidOperationException($"Failed to convert '{y}' to enum '{typeof(TProperty)}'");
                }

                return (TProperty)enumValue;
            };
        }
        else
        {
            var converter2 = new GenericToEnumConverter<int, TProperty>();
            PropertyToColumnConverter = x => converter2.PropertyToColumn(x!);
            ColumnToPropertyConverter = x => converter2.ColumnToProperty((int)x);
        }
    }
}

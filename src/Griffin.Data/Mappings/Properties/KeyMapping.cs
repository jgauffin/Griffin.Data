using System;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using Griffin.Data.Mapper;

namespace Griffin.Data.Mappings.Properties;

/// <summary>
///     Mapping for a key column.
/// </summary>
public class KeyMapping<TEntity, TProperty> : IKeyMapping
{
    private readonly TProperty? _defaultValue = default;
    private readonly Type _entityType;
    private readonly Func<TEntity, TProperty?>? _getter;
    private readonly Action<TEntity, TProperty>? _setter;

    /// <summary>
    /// </summary>
    /// <param name="entityType">Entity that this mapping belongs to.</param>
    /// <param name="getter">Method used to read from value from the entity property.</param>
    /// <param name="setter">Method used to write a value to the entity property.</param>
    public KeyMapping(Type entityType, Func<TEntity, TProperty?>? getter, Action<TEntity, TProperty>? setter)
    {
        _entityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
        _getter = getter;
        _setter = setter;
    }

    /// <summary>
    ///     This column is auto incremented (i.e. should not be specified in insert statements).
    /// </summary>
    public bool IsAutoIncrement { get; internal set; }

    /// <inheritdoc />
    public void MapRecord(IDataRecord record, object entity)
    {
        if (_setter == null)
        {
            return;
        }

        var value = record[ColumnName];
        _setter((TEntity)entity, (TProperty)value);
    }

    /// <inheritdoc />
    public void SetPropertyValue(object entity, object value)
    {
        _setter?.Invoke((TEntity)entity, (TProperty)value);
    }

    /// <summary>
    ///     Get a value from the property.
    /// </summary>
    /// <param name="entity">Entity to read from.</param>
    /// <returns>Value read from the property.</returns>
    /// <exception cref="InvalidOperationException"></exception>
    public object? GetColumnValue([NotNull] object entity)
    {
        if (_getter == null)
        {
            throw new MappingException(entity,
                $"No getter has been defined for property ${PropertyName}.");
        }

        var value = _getter((TEntity)entity);
        if (value == null || value.Equals(_defaultValue))
        {
            return null;
        }

        return value;
    }

    /// <inheritdoc />
    public Type PropertyType => typeof(TProperty);

    /// <summary>
    ///     Column name.
    /// </summary>
    /// <value>
    ///     Default is the same as the property name.
    /// </value>
    public string ColumnName { get; internal set; } = "";

    /// <summary>
    ///     Property name.
    /// </summary>
    public string PropertyName { get; internal set; } = "";

    /// <summary>
    ///     NoOp - Keys must be the same in the DB and the class.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <returns>Same value as given.</returns>
    public object ConvertToColumnValue([NotNull] object value)
    {
        return value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <summary>
    ///     Used to read from value from the entity property
    /// </summary>
    /// <param name="instance">entity.</param>
    /// <param name="value">Value to set.</param>
    /// <exception cref="InvalidOperationException"></exception>
    public void SetColumnValue([NotNull] object instance, object value)
    {
        if (_setter == null)
        {
            throw new MappingException(instance,
                $"No setter has been defined for property ${PropertyName}.");
        }

        _setter((TEntity)instance, (TProperty)value);
    }
}

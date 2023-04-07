using System;
using System.Diagnostics.CodeAnalysis;
using Griffin.Data.Mapper;

namespace Griffin.Data.Mappings.Properties;

public interface IKeyMapping : IFieldMapping
{
    /// <summary>
    ///     This column is auto incremented (i.e. should not be specified in insert statements).
    /// </summary>
    bool IsAutoIncrement { get; }
}

/// <summary>
///     Mapping for a key column.
/// </summary>
public class KeyMapping<TEntity, TPropertyType> : IKeyMapping
{
    private readonly Type _entityType;
    private readonly Func<TEntity, TPropertyType?>? _getter;
    private readonly Action<TEntity, TPropertyType>? _setter;
    private readonly TPropertyType? _defaultValue = default;

    /// <summary>
    /// </summary>
    /// <param name="entityType">Entity that this mapping belongs to.</param>
    /// <param name="getter">Method used to read from value from the entity property.</param>
    /// <param name="setter">Method used to write a value to the entity property.</param>
    public KeyMapping(Type entityType, Func<TEntity, TPropertyType?>? getter, Action<TEntity, TPropertyType>? setter)
    {
        _entityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
        _getter = getter;
        _setter = setter;
    }

    /// <summary>
    ///     This column is auto incremented (i.e. should not be specified in insert statements).
    /// </summary>
    public bool IsAutoIncrement { get; internal set; }

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

        _setter((TEntity)instance, (TPropertyType)value);
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
    public object ToColumnValue([NotNull] object value)
    {
        return value ?? throw new ArgumentNullException(nameof(value));
    }
}

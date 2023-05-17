using System;
using System.Diagnostics.CodeAnalysis;
using Griffin.Data.Configuration;
using Griffin.Data.Mapper.Mappings.Properties;

namespace Griffin.Data.Mapper.Mappings.Relations;

/// <summary>
///     Mapping for a has one/many FK.
/// </summary>
public class ForeignKeyMapping<TParentEntity, TChildEntity> : IForeignKey
{
    private readonly IFieldAccessor? _foreignKey;
    private readonly IFieldAccessor? _referencedProperty;

    /// <summary>
    /// </summary>
    /// <param name="columnName">Column name (specified when there are no FK property in the child table).</param>
    /// <param name="foreignKey">Foreign key property accessor (in the child table).</param>
    /// <param name="referencedProperty">Referenced property accessor (in the parent table).</param>
    public ForeignKeyMapping(string columnName, IFieldAccessor? foreignKey, IFieldAccessor? referencedProperty)
    {
        if (string.IsNullOrEmpty(columnName) && foreignKey == null)
        {
            throw new MappingConfigurationException(typeof(TParentEntity),
                "Both FK column and FK property cannot be empty.");
        }

        _foreignKey = foreignKey;
        ForeignKeyColumnName = columnName;
        _referencedProperty = referencedProperty;
    }

    /// <summary>
    ///     Column to insert the foreign key in.
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         Specified instead of the property if the child entity do not contain a property for the FK.
    ///     </para>
    /// </remarks>
    public string ForeignKeyColumnName { get; }

    /// <summary>
    ///     Has a foreign key property configured.
    /// </summary>
    public bool HasProperty => _foreignKey != null;

    /// <inheritdoc />
    public object? GetColumnValue([NotNull] object childEntity)
    {
        if (childEntity == null)
        {
            throw new ArgumentNullException(nameof(childEntity));
        }

        if (_foreignKey == null)
        {
            throw new MappingException(childEntity,
                "FK property has not been configured. Configure it or use the FK column name instead.");
        }

        return _foreignKey.GetColumnValue(childEntity);
    }

    /// <summary>
    ///     Get id from the parent entity.
    /// </summary>
    /// <param name="parentEntity">Entity to fetch id from</param>
    /// <returns>id if found; otherwise <c>null</c>.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    /// <exception cref="MappingException"></exception>
    public object? GetReferencedId([DisallowNull] TParentEntity parentEntity)
    {
        if (parentEntity == null)
        {
            throw new ArgumentNullException(nameof(parentEntity));
        }

        if (_referencedProperty == null)
        {
            throw new MappingException(parentEntity,
                "A parent property reference has not been configured.");
        }

        return _referencedProperty.GetColumnValue(parentEntity);
    }

    /// <inheritdoc />
    public void SetColumnValue([NotNull] object childEntity, object value)
    {
        if (childEntity == null)
        {
            throw new ArgumentNullException(nameof(childEntity));
        }

        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        if (_foreignKey == null)
        {
            throw new MappingException(childEntity,
                "FK property has not been configured. Configure it or use the FK column name instead.");
        }

        _foreignKey.SetPropertyValue(childEntity, value);
    }
}

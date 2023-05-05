using System;
using System.Linq.Expressions;
using System.Reflection;
using Griffin.Data.Helpers;
using Griffin.Data.Mapper.Mappings;
using Griffin.Data.Mapper.Mappings.Properties;
using Griffin.Data.Mapper.Mappings.Relations;

namespace Griffin.Data.Configuration;

/// <summary>
///     Foreign key configuration for a has one/many mapping.
/// </summary>
/// <typeparam name="TParentEntity">Entity that the foreign key in the child entity points at.</typeparam>
/// <typeparam name="TChildEntity">Child entity type.</typeparam>
public class ForeignKeyConfiguration<TParentEntity, TChildEntity>
{
    private readonly string? _columnName;
    private readonly string? _fkPropertyName;
    private string? _referencedPropertyName;

    /// <summary>
    /// </summary>
    /// <param name="propertyInfo">Property info for the FK in the child class.</param>
    public ForeignKeyConfiguration(PropertyInfo propertyInfo)
    {
        if (propertyInfo == null)
        {
            throw new ArgumentNullException(nameof(propertyInfo));
        }

        _fkPropertyName = propertyInfo.Name;
    }

    /// <summary>
    /// </summary>
    /// <param name="columnName">FK column</param>
    /// <remarks>
    ///     <para>This constructor should only be used if the child entity do not have a property for the FK column.</para>
    /// </remarks>
    public ForeignKeyConfiguration(string columnName)
    {
        _columnName = columnName ?? throw new ArgumentNullException(nameof(columnName));
    }

    /// <summary>
    ///     Build a foreign key mapping from the configuration.
    /// </summary>
    /// <param name="registry">Mapping registry (should contain all mappings when this method is invoked).</param>
    /// <returns>Generated mapping.</returns>
    public ForeignKeyMapping<TParentEntity, TChildEntity> Build(IMappingRegistry registry)
    {
        var referencedMapping = registry.Get<TParentEntity>();
        var childMapping = registry.Get<TChildEntity>();

        IFieldAccessor? fk = _fkPropertyName == null ? null : childMapping.GetProperty(_fkPropertyName);
        if (_referencedPropertyName == null)
        {
            throw new MappingConfigurationException(typeof(TChildEntity),
                "A reference property to the parent class was not specified.");
        }

        var referenced = referencedMapping.GetProperty(_referencedPropertyName);

        var columnName = _columnName ?? _fkPropertyName ??
            throw new MappingConfigurationException(typeof(TChildEntity), "A foreign key has not been configured.");
        var mapping = new ForeignKeyMapping<TParentEntity, TChildEntity>(columnName, fk, referenced);
        return mapping;
    }

    /// <summary>
    ///     Property in the parent entity that the FK points at.
    /// </summary>
    /// <typeparam name="TReferencedProperty">Referenced property type.</typeparam>
    /// <param name="referencedPropertySelector">Expression used to select the property.</param>
    public void References<TReferencedProperty>(
        Expression<Func<TParentEntity, TReferencedProperty>> referencedPropertySelector)
    {
        _referencedPropertyName = referencedPropertySelector.GetMemberName();
    }
}

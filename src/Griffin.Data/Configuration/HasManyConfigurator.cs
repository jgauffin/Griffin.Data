using System;
using System.Linq.Expressions;
using System.Reflection;
using Griffin.Data.Helpers;
using Griffin.Data.Mappings;
using Griffin.Data.Mappings.Relations;

namespace Griffin.Data.Configuration;

/// <summary>
///     Configures a has many relationship between a parent (one) and the child (many).
/// </summary>
/// <typeparam name="TParentEntity">Type of parent entity (that contains a collection property).</typeparam>
/// <typeparam name="TChildEntity">Type of child (that is in the collection property).</typeparam>
public class HasManyConfigurator<TParentEntity, TChildEntity> : IHasManyConfigurator where TParentEntity : notnull where TChildEntity : notnull
{
    private readonly PropertyInfo _propertyInfo;
    private ForeignKeyConfiguration<TParentEntity, TChildEntity>? _fkConfigurator;

    /// <summary>
    /// </summary>
    /// <param name="propertyInfo">Property that contains the child collection.</param>
    /// <param name="elementType"></param>
    /// <exception cref="ArgumentNullException">hasManyMapping is null.</exception>
    public HasManyConfigurator(PropertyInfo propertyInfo, Type elementType)
    {
        _propertyInfo = propertyInfo;
    }

    string IHasManyConfigurator.PropertyName => _propertyInfo.Name;

    IHasManyMapping IHasManyConfigurator.Build(IMappingRegistry mappingRegistry)
    {
        if (_fkConfigurator == null)
        {
            throw new MappingConfigurationException(typeof(TParentEntity),
                $"A foreign key was not specified for '{typeof(TChildEntity).Name}'.");
        }

        var fk = _fkConfigurator.Build(mappingRegistry);

        // Need to be object since 
        // we can use different types of lists.
        var setter = _propertyInfo.GenerateSetterDelegate<TParentEntity, object>();
        var getter = _propertyInfo.GenerateGetterDelegate<TParentEntity, object>();
        if (setter == null)
        {
            throw new MappingConfigurationException(typeof(TParentEntity), "A HasMany property must have a setter or a backing field with the same name ('Users' => '_users').");
        }
        if (getter == null)
        {
            throw new MappingConfigurationException(typeof(TParentEntity), "A HasMany property must have a getter or a backing field with the same name ('Users' => '_users').");
        }


        var mapping = new HasManyMapping<TParentEntity, TChildEntity>(fk, getter, setter);
        return mapping;
    }


    /// <summary>
    ///     FK in the child entity.
    /// </summary>
    /// <typeparam name="TForeignKeyProperty">Property selector</typeparam>
    /// <param name="referencedPropertySelector"></param>
    /// <returns>Configuration.</returns>
    public ForeignKeyConfiguration<TParentEntity, TChildEntity> ForeignKey<TForeignKeyProperty>(
        Expression<Func<TChildEntity, TForeignKeyProperty>> referencedPropertySelector)
    {
        if (_fkConfigurator != null)
            return _fkConfigurator;

        _fkConfigurator =
            new ForeignKeyConfiguration<TParentEntity, TChildEntity>(referencedPropertySelector.GetPropertyInfo());
        return _fkConfigurator;
    }


    /// <summary>
    ///     Point at the column in the table.
    /// </summary>
    /// <param name="columnName">Column containing the property.</param>
    /// <returns>Configuration.</returns>
    /// <remarks>
    ///     <para>
    ///         This option should be used when the child entity do not have a property for the FK in the class.
    ///     </para>
    /// </remarks>
    public ForeignKeyConfiguration<TParentEntity, TChildEntity> ForeignKeyColumn(string columnName)
    {
        if (_fkConfigurator != null)
            throw new MappingConfigurationException(typeof(TParentEntity),
                "FK has already been configured for " + _propertyInfo.Name);

        _fkConfigurator = new ForeignKeyConfiguration<TParentEntity, TChildEntity>(columnName);
        return _fkConfigurator;
    }
}
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Griffin.Data.Helpers;
using Griffin.Data.Mapper.Mappings;
using Griffin.Data.Mapper.Mappings.Relations;

namespace Griffin.Data.Configuration;

/// <summary>
///     Configures a one to one relationship (1 to 0..1).
/// </summary>
/// <typeparam name="TParentEntity">Parent entity (the one being referenced by the FK).</typeparam>
/// <typeparam name="TChildEntity">Child (contains the FK).</typeparam>
public class HasOneConfigurator<TParentEntity, TChildEntity> : IHasOneConfigurator where TParentEntity : notnull
{
    private readonly string _propertyName;
    private readonly Expression<Func<TParentEntity, TChildEntity>> _selector;
    private Discriminator<TParentEntity, TChildEntity>? _discriminator;
    private ForeignKeyConfiguration<TParentEntity, TChildEntity>? _fk;
    private KeyValuePair<string, string>? _subsetColumn;

    /// <summary>
    /// </summary>
    /// <param name="selector">Property selector</param>
    public HasOneConfigurator(Expression<Func<TParentEntity, TChildEntity>> selector)
    {
        _selector = selector ?? throw new ArgumentNullException(nameof(selector));
        _propertyName = selector.GetMemberName();
    }

    /// <inheritdoc />
    string IHasOneConfigurator.PropertyName => _propertyName;

    /// <inheritdoc />
    IHasOneMapping IHasOneConfigurator.Build(IMappingRegistry mappingRegistry)
    {
        if (_fk == null)
        {
            throw new MappingConfigurationException(typeof(TChildEntity), "Foreign key was not configured.");
        }

        var fk = _fk.Build(mappingRegistry);
        var getter = _selector.Compile();
#pragma warning disable CS8714
        var setter = _selector.GetPropertyInfo().GenerateSetterDelegate<TParentEntity, TChildEntity>();
#pragma warning restore CS8714
        if (setter == null)
        {
            throw new MappingConfigurationException(typeof(TParentEntity),
                $"Property {_propertyName} must have a setter or a backing field with the same name as the property ('Users' => '_users').");
        }

        var mapping = new HasOneMapping<TParentEntity, TChildEntity>(fk, getter, setter)
        {
            SubsetColumn = _subsetColumn
        };

        if (_discriminator == null)
        {
            return mapping;
        }

        mapping.DiscriminatorProperty =
            mappingRegistry.Get(typeof(TParentEntity)).GetProperty(_discriminator.PropertyName);
        mapping.DiscriminatorTypeSelector = _discriminator.TypeSelector;
        return mapping;
    }

    /// <summary>
    ///     A discriminator is used to tell which type of child type to load (in case of inheritance).
    /// </summary>
    /// <typeparam name="TDiscriminatorProperty">Property used to determine child type.</typeparam>
    /// <param name="propertySelector">Expression used to selected discriminator property.</param>
    /// <param name="factory">
    ///     Factory used to construct types based on the discriminator property value. Contains discriminator
    ///     property, the column value and should return the created entity.
    /// </param>
    /// <returns>Config.</returns>
    public HasOneConfigurator<TParentEntity, TChildEntity> Discriminator<TDiscriminatorProperty>(
        Expression<Func<TParentEntity, TDiscriminatorProperty>> propertySelector,
        Func<TDiscriminatorProperty, object, TChildEntity?> factory)
    {
        //new Discriminator<TParentEntity, TChildEntity>(propertySelector.GetMemberName());
        return this;
    }

    /// <summary>
    ///     A discriminator is used to tell which type of child type to load (in case of inheritance).
    /// </summary>
    /// <typeparam name="TDiscriminatorProperty">Property used to determine child type.</typeparam>
    /// <param name="selector">Expression used to selected discriminator property.</param>
    /// <param name="typeSelector">Factory used to construct types based on the discriminator property value.</param>
    /// <returns>Config.</returns>
    public HasOneConfigurator<TParentEntity, TChildEntity> Discriminator<TDiscriminatorProperty>(
        Expression<Func<TParentEntity, TDiscriminatorProperty>> selector,
        Func<TDiscriminatorProperty, Type?> typeSelector)
    {
        _discriminator = new Discriminator<TParentEntity, TChildEntity>(selector.GetMemberName(),
            x => typeSelector((TDiscriminatorProperty)x));
        return this;
    }

    /// <summary>
    ///     Foreign key in the child entity.
    /// </summary>
    /// <typeparam name="TForeignKeyProperty">Type of FK.</typeparam>
    /// <param name="propertySelector">Expression to select the fK property.</param>
    /// <returns>config.</returns>
    public ForeignKeyConfiguration<TParentEntity, TChildEntity> ForeignKey<TForeignKeyProperty>(
        Expression<Func<TChildEntity, TForeignKeyProperty>> propertySelector)
    {
        _fk = new ForeignKeyConfiguration<TParentEntity, TChildEntity>(propertySelector.GetPropertyInfo());
        return _fk;
    }

    /// <summary>
    ///     Foreign key in the child entity.
    /// </summary>
    /// <param name="columnName">Column that contains the FK value.</param>
    /// <returns>config.</returns>
    /// <remarks>
    ///     <para>
    ///         This option should only be used when the child entity do not contain a property for the foreign key.
    ///     </para>
    /// </remarks>
    public ForeignKeyConfiguration<TParentEntity, TChildEntity> ForeignKey(string columnName)
    {
        _fk = new ForeignKeyConfiguration<TParentEntity, TChildEntity>(columnName);
        return _fk;
    }

    /// <summary>
    ///     A constant column value.
    /// </summary>
    /// <param name="columnName">Column to set value for.</param>
    /// <param name="value">Value to set.</param>
    /// <remarks>
    ///     <para>
    ///         Typically used when the same table is used as a child for multiple types of entities (or multiple properties in
    ///         the same entity). This column identifies which parent the child entity is for.
    ///     </para>
    /// </remarks>
    public HasOneConfigurator<TParentEntity, TChildEntity> SubsetColumn(string columnName, string value)
    {
        if (columnName == null)
        {
            throw new ArgumentNullException(nameof(columnName));
        }

        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        _subsetColumn = new KeyValuePair<string, string>(columnName, value);
        return this;
    }
}

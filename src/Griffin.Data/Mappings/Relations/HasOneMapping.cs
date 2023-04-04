using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Griffin.Data.Configuration;
using Griffin.Data.Mapper;
using Griffin.Data.Mappings.Properties;

namespace Griffin.Data.Mappings.Relations;

/// <summary>
///     One to zero/one mapping.
/// </summary>
/// <typeparam name="TParent">Parent entity.</typeparam>
/// <typeparam name="TChild">Child entity.</typeparam>
public class HasOneMapping<TParent, TChild> : RelationShipBase<TParent, TChild>, IHasOneMapping
{
    private readonly Func<TParent, TChild> _getter;
    private readonly Action<TParent, TChild> _setter;

    /// <summary>
    /// </summary>
    /// <param name="fk">Foreign key property in the child entity.</param>
    /// <param name="getter">Getter to fetch the child object from a parent property.</param>
    /// <param name="setter">Setter to assign the child entity to a parent property.</param>
    public HasOneMapping(ForeignKeyMapping<TParent, TChild> fk, Func<TParent, TChild> getter,
        Action<TParent, TChild> setter) : base(fk, typeof(TChild))
    {
        _getter = getter;
        _setter = setter;
    }

    /// <summary>
    ///     Property used to determine which child sub class to create.
    /// </summary>
    internal IFieldMapping? DiscriminatorProperty { get; set; }

    /// <summary>
    ///     Callback used to decide sub class type.
    /// </summary>
    internal Func<object, Type?>? DiscriminatorTypeSelector { get; set; }

    KeyValuePair<string, string>? IHasOneMapping.SubsetColumn { get; set; }

    /// <inheritdoc />
    public void SetColumnValue([NotNull] object parentEntity, object value)
    {
        _setter((TParent)parentEntity, (TChild)value);
    }

    /// <inheritdoc />
    public object? GetColumnValue([NotNull] object parentEntity)
    {
        return _getter((TParent)parentEntity);
    }

    /// <summary>
    ///     Have a discriminator property.
    /// </summary>
    public bool HaveDiscriminator => DiscriminatorProperty != null;

    /// <summary>
    /// Should be added to all insert/update statements and used in all WHERE statements.
    /// </summary>
    public KeyValuePair<string, string>? SubsetColumn { get; set; }

    /// <summary>
    ///     Used to decide sub class type.
    /// </summary>
    /// <param name="parentEntity">Entity that contains the discriminator property.</param>
    /// <returns></returns>
    public Type? GetTypeUsingDiscriminator(object parentEntity)
    {
        if (parentEntity == null) throw new ArgumentNullException(nameof(parentEntity));
        if (DiscriminatorProperty == null || DiscriminatorTypeSelector == null)
            throw new MappingConfigurationException(typeof(TParent),
                $"A discriminator has not been configured correctly for {typeof(TChild).Name}.");
        var value = DiscriminatorProperty.GetColumnValue(parentEntity);
        if (value == null) throw new MappingException(parentEntity, "Failed to get a discriminator value.");

        return DiscriminatorTypeSelector(value);
    }

    protected override void ApplyConstraints(IDictionary<string, object> dbParameters)
    {
        if (SubsetColumn != null)
        {
            dbParameters.Add(SubsetColumn.Value.Key, SubsetColumn.Value.Value);
        }
    }
}
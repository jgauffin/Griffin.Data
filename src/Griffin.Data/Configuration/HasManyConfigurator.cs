using System;
using System.Linq.Expressions;
using Griffin.Data.Helpers;
using Griffin.Data.Mappings.Relations;

namespace Griffin.Data.Configuration;

/// <summary>
///     Configures a has many relationship between a parent (one) and the child (many).
/// </summary>
/// <typeparam name="TParentEntity">Type of parent entity (that contains a collection property).</typeparam>
/// <typeparam name="TChildEntity">Type of child (that is in the collection property).</typeparam>
public class HasManyConfigurator<TParentEntity, TChildEntity>
{
    private readonly HasManyMapping _hasManyMapping;


    /// <summary>
    /// </summary>
    /// <param name="hasManyMapping">Mapping to configure.</param>
    /// <exception cref="ArgumentNullException">hasManyMapping is null.</exception>
    public HasManyConfigurator(HasManyMapping hasManyMapping)
    {
        _hasManyMapping = hasManyMapping ?? throw new ArgumentNullException(nameof(hasManyMapping));
    }


    public IDenominator Denominator<TDenominatorProperty>(
        Expression<Func<TParentEntity, TDenominatorProperty>> denominatorSelector)
    {
        return new Denominator<TParentEntity, TChildEntity>(denominatorSelector.GetMemberName());
    }
    /// <summary>
    ///     FK in the child entity.
    /// </summary>
    /// <typeparam name="TForeignKeyProperty">Property selector</typeparam>
    /// <param name="referencedPropertySelector"></param>
    /// <returns>Configuration.</returns>
    public ForeignKeyConfiguration<TParentEntity> ForeignKey<TForeignKeyProperty>(
        Expression<Func<TChildEntity, TForeignKeyProperty>> referencedPropertySelector)
    {
        var prop = referencedPropertySelector.GetPropertyInfo();
        _hasManyMapping.ForeignKey = new ForeignKeyMapping(typeof(TParentEntity), prop.Name);
        return new ForeignKeyConfiguration<TParentEntity>(_hasManyMapping.ForeignKey);
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
    public ForeignKeyConfiguration<TParentEntity> ForeignKey(string columnName)
    {
        _hasManyMapping.ForeignKey = new ForeignKeyMapping(typeof(TParentEntity), columnName: columnName);
        return new ForeignKeyConfiguration<TParentEntity>(_hasManyMapping.ForeignKey);
    }
}

public class Denominator<T, T1> : IDenominator
{
    public Denominator(string getMemberName)
    {
        throw new NotImplementedException();
    }
}
using System;
using System.Linq.Expressions;
using Griffin.Data.Helpers;
using Griffin.Data.Mappings.Relations;

namespace Griffin.Data.Configuration;

public class HasOne<TParentEntity, TChildEntity>
{
    private readonly HasOneMapping _mapping;

    public HasOne(HasOneMapping mapping)
    {
        _mapping = mapping;
        _mapping.ParentEntityType = typeof(TParentEntity);
        _mapping.ChildEntityType = typeof(TChildEntity);
    }


    public HasOne<TParentEntity, TChildEntity> Denominator<TDenominatorProperty>(
        Expression<Func<TParentEntity, TDenominatorProperty>> denominatorSelector, Func<TDenominatorProperty, TChildEntity?> factory)
    {
        //new Denominator<TParentEntity, TChildEntity>(denominatorSelector.GetMemberName());
        return this;
    }

    public ForeignKeyConfiguration<TParentEntity> ForeignKey<TForeignKeyProperty>(
        Expression<Func<TChildEntity, TForeignKeyProperty>> propertySelector)
    {
        _mapping.ForeignKey = new ForeignKeyMapping(typeof(TParentEntity), propertySelector.GetMemberName());
        return new ForeignKeyConfiguration<TParentEntity>(_mapping.ForeignKey);
    }

    public ForeignKeyConfiguration<TParentEntity> ForeignKey(string columnName)
    {
        _mapping.ForeignKey = new ForeignKeyMapping(typeof(TParentEntity), columnName: columnName);
        return new ForeignKeyConfiguration<TParentEntity>(_mapping.ForeignKey);
    }
}


public class Denominator<T, T1> : IDenominator
{
    public Denominator(string getMemberName)
    {
        throw new NotImplementedException();
    }
}
public interface IDenominator
{

}
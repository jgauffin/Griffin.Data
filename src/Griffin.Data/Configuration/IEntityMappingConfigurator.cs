using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Griffin.Data.Configuration;

public interface IClassMappingConfigurator<TEntity>
{
    void TableName(string tableName);
    KeyConfigurator<TEntity, TProperty> Key<TProperty>(Expression<Func<TEntity, TProperty>> selector);

    PropertyConfigurator<TEntity, TProperty> Property<TProperty>(
        Expression<Func<TEntity, TProperty>> selector);

    HasManyConfigurator<TEntity, TProperty> HasMany<TProperty>(
        Expression<Func<TEntity, IReadOnlyList<TProperty>>> selector);

    HasManyConfigurator<TEntity, TProperty> HasMany<TProperty>(Expression<Func<TEntity, IList<TProperty>>> selector);

    HasManyConfigurator<TEntity, TProperty> HasMany<TProperty>(
        Expression<Func<TEntity, ICollection<TProperty>>> selector);

    HasOne<TEntity, TProperty> HasOne<TProperty>(Expression<Func<TEntity, TProperty>> selector);
}
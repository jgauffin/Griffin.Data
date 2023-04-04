using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Griffin.Data.Configuration;

/// <summary>
///     Allows developer to generate a mapping for a specific entity.
/// </summary>
/// <typeparam name="TEntity">Type of entity to create a mapping for.</typeparam>
public interface IClassMappingConfigurator<TEntity> where TEntity : notnull
{
    /// <summary>
    ///     Specify a table name (only required if the table name is different from the class name).
    /// </summary>
    /// <param name="tableName">Name of the corresponding table in the database.</param>
    /// <exception cref="ArgumentNullException">table name is not specified.</exception>
    void TableName(string tableName);


    /// <summary>
    ///     Define a primary key.
    /// </summary>
    /// <typeparam name="TProperty">Property type for the primary key.</typeparam>
    /// <param name="selector">Expression used to select the correct property.</param>
    /// <returns></returns>
    KeyConfigurator<TEntity, TProperty> Key<TProperty>(Expression<Func<TEntity, TProperty>> selector)
        where TProperty : notnull;

    /// <summary>
    ///     Map a property (which is not a key or child entity).
    /// </summary>
    /// <typeparam name="TProperty">Type of property.</typeparam>
    /// <param name="selector">Expression used to select a property.</param>
    /// <returns>Property configurator.</returns>
    PropertyConfigurator<TEntity, TProperty> Property<TProperty>(
        Expression<Func<TEntity, TProperty>> selector) where TProperty : notnull;

    /// <summary>
    ///     A property is for a collection of child entities.
    /// </summary>
    /// <typeparam name="TProperty">Type of child property.</typeparam>
    /// <param name="selector">Expression used to select the property.</param>
    /// <returns>Has many configuration.</returns>
    HasManyConfigurator<TEntity, TProperty> HasMany<TProperty>(
        Expression<Func<TEntity, IReadOnlyList<TProperty>>> selector) where TProperty : notnull;

    /// <summary>
    ///     A property is for a collection of child entities.
    /// </summary>
    /// <typeparam name="TProperty">Type of child property.</typeparam>
    /// <param name="selector">Expression used to select the property.</param>
    /// <returns>Has many configuration.</returns>
    HasManyConfigurator<TEntity, TProperty> HasMany<TProperty>(Expression<Func<TEntity, IList<TProperty>>> selector)
        where TProperty : notnull;

    /// <summary>
    ///     A property is for a collection of child entities.
    /// </summary>
    /// <typeparam name="TProperty">Type of child property.</typeparam>
    /// <param name="selector">Expression used to select the property.</param>
    /// <returns>Has many configuration.</returns>
    HasManyConfigurator<TEntity, TProperty> HasMany<TProperty>(
        Expression<Func<TEntity, ICollection<TProperty>>> selector) where TProperty : notnull;

    /// <summary>
    ///     A property is for a single child entity.
    /// </summary>
    /// <typeparam name="TProperty">Type of child property.</typeparam>
    /// <param name="selector">Expression used to select the property.</param>
    /// <returns>Has one configuration.</returns>
    HasOneConfigurator<TEntity, TProperty> HasOne<TProperty>(Expression<Func<TEntity, TProperty>> selector);

    /// <summary>
    ///     Map all properties which has not been mapped already.
    /// </summary>
    void MapRemainingProperties();
}
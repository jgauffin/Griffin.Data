using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Griffin.Data.Helpers;
using Griffin.Data.Mappings;
using Griffin.Data.Mappings.Properties;

namespace Griffin.Data.Configuration;

/// <summary>
///     Implementation for <see cref="IClassMappingConfigurator{TEntity}" />.
/// </summary>
/// <typeparam name="TEntity">Type of entity that this is a mapping for.</typeparam>
public class ClassMappingConfigurator<TEntity> : IMappingBuilder, IClassMappingConfigurator<TEntity>
    where TEntity : notnull
{
    private readonly List<IHasManyConfigurator> _hasManyMappings = new();
    private readonly List<IHasOneConfigurator> _hasOneMappings = new();
    private readonly List<IKeyMapping> _keys = new();
    private readonly List<IPropertyMapping> _properties = new();
    private ClassMapping? _mapping;
    private string _tableName;

    /// <summary>
    /// </summary>
    public ClassMappingConfigurator()
    {
        _tableName = typeof(TEntity).Name.Pluralize();
    }

    /// <summary>
    ///     Table name has been specified explicitly.
    /// </summary>
    public bool IsTableNameSpecified { get; set; }

    /// <inheritdoc />
    public void TableName(string tableName)
    {
        IsTableNameSpecified = true;
        _tableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
    }

    /// <inheritdoc />
    public KeyConfigurator<TEntity, TProperty> Key<TProperty>(Expression<Func<TEntity, TProperty>> selector)
        where TProperty : notnull
    {
        if (selector == null)
        {
            throw new ArgumentNullException(nameof(selector));
        }

        var compiled = selector.Compile();

        TProperty? Getter(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            return compiled(entity);
        }

        var setter = selector.GetPropertyInfo().GenerateSetterDelegate<TEntity, TProperty>();

        var mapping = new KeyMapping<TEntity, TProperty>(typeof(TEntity), Getter, setter)
        {
            PropertyName = selector.GetMemberName(), ColumnName = selector.GetMemberName()
        };
        _keys.Add(mapping);

        return new KeyConfigurator<TEntity, TProperty>(mapping);
    }

    /// <inheritdoc />
    public PropertyConfigurator<TEntity, TProperty> Property<TProperty>(
        Expression<Func<TEntity, TProperty>> selector) where TProperty : notnull
    {
        var compiled = selector.Compile();
        var setter = selector.GetPropertyInfo().GenerateSetterDelegate<TEntity, TProperty>();
        var prop = selector.GetPropertyInfo();

        var mapping = new PropertyMapping<TEntity, TProperty>(prop.Name, compiled, setter)
        {
            PropertyName = prop.Name, ColumnName = prop.Name
        };
        _properties.Add(mapping);

        return new PropertyConfigurator<TEntity, TProperty>(mapping);
    }

    /// <inheritdoc />
    public HasManyConfigurator<TEntity, TProperty> HasMany<TProperty>(
        Expression<Func<TEntity, IReadOnlyList<TProperty>>> selector) where TProperty : notnull
    {
        var getter = selector.Compile();
        var prop = selector.GetPropertyInfo();
        return CreateHasManyMapping<TProperty>(entity => getter(entity), prop, typeof(TProperty));
    }

    /// <inheritdoc />
    public HasManyConfigurator<TEntity, TProperty> HasMany<TProperty>(
        Expression<Func<TEntity, IList<TProperty>>> selector) where TProperty : notnull
    {
        var getter = selector.Compile();
        var prop = selector.GetPropertyInfo();
        return CreateHasManyMapping<TProperty>(entity => getter(entity), prop, typeof(TEntity));
    }

    /// <inheritdoc />
    public HasManyConfigurator<TEntity, TProperty> HasMany<TProperty>(
        Expression<Func<TEntity, ICollection<TProperty>>> selector) where TProperty : notnull
    {
        var getter = selector.Compile();
        var prop = selector.GetPropertyInfo();
        return CreateHasManyMapping<TProperty>(entity => getter(entity), prop, typeof(TEntity));
    }

    /// <inheritdoc />
    public HasOneConfigurator<TEntity, TProperty> HasOne<TProperty>(Expression<Func<TEntity, TProperty>> selector)
    {
        var config = new HasOneConfigurator<TEntity, TProperty>(selector);
        _hasOneMappings.Add(config);
        return config;
    }

    /// <inheritdoc />
    public void MapRemainingProperties()
    {
        const string methodName = nameof(GenerateProperty);
        var method = GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance) ??
                     throw new InvalidOperationException("Failed to get generate method from this.");
        
        var props = typeof(TEntity).GetProperties();
        foreach (var prop in props)
        {
            // Do not override manual configurations.
            if (_properties.Any(x => x.PropertyName == prop.Name) ||
                _keys.Any(x => x.PropertyName == prop.Name) ||
                _hasManyMappings.Any(x => x.PropertyName == prop.Name) ||
                _hasOneMappings.Any(x => x.PropertyName == prop.Name))
            {
                continue;
            }

            // Ignore all collection properties that has not been explicitly mapped.
            if (prop.PropertyType.IsCollection())
            {
                continue;
            }

            var propMapping =
                (IPropertyMapping?)method.MakeGenericMethod(prop.PropertyType).Invoke(this, new object[] { prop }) ??
                throw new InvalidOperationException(
                    $"Failed to retrieve property mapping for {prop.Name} in type {typeof(TEntity)}");
            

            _properties.Add(propMapping);
        }
    }

    /// <summary>
    ///     Create a mapping using this configuration.
    /// </summary>
    /// <returns>Generated mapping.</returns>
    ClassMapping IMappingBuilder.BuildMapping(bool pluralizeTableNames)
    {
        if (!IsTableNameSpecified && !pluralizeTableNames)
        {
            _tableName = typeof(TEntity).Name;
        }

        _mapping = new ClassMapping(typeof(TEntity), _tableName, _keys, _properties.Where(x => !x.IsIgnored).ToList());
        return _mapping;
    }

    void IMappingBuilder.BuildRelations(IMappingRegistry registry)
    {
        var hasMany = _hasManyMappings.Select(x => x.Build(registry)).ToList();
        var hasOne = _hasOneMappings.Select(x => x.Build(registry)).ToList();
        if (_mapping == null)
        {
            throw new MappingConfigurationException(typeof(TEntity),
                "You must build all mappings before assigning references.");
        }

        _mapping.AddRelations(hasMany, hasOne);
    }

    private HasManyConfigurator<TEntity, TProperty> CreateHasManyMapping<TProperty>(
        Func<TEntity, object> _,
        PropertyInfo prop,
        Type elementType) where TProperty : notnull
    {
        var config = new HasManyConfigurator<TEntity, TProperty>(prop, elementType);
        _hasManyMappings.Add(config);
        return config;
    }

    private IPropertyMapping GenerateProperty<TProperty>(PropertyInfo prop) where TProperty : notnull
    {
        if (prop == null)
        {
            throw new ArgumentNullException(nameof(prop));
        }

        var getter = prop.GenerateGetterDelegate<TEntity, TProperty>();
        var setter = prop.GenerateSetterDelegate<TEntity, TProperty>();
        return new PropertyMapping<TEntity, TProperty>(prop.Name, getter, setter);
    }
}

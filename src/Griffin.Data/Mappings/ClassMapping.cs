using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Griffin.Data.Configuration;
using Griffin.Data.Mapper;
using Griffin.Data.Mappings.Properties;
using Griffin.Data.Mappings.Relations;

namespace Griffin.Data.Mappings;

/// <summary>
///     Mapping for a specific type of entity.
/// </summary>
public class ClassMapping
{
    private readonly List<IHasOneMapping> _children = new();
    private readonly List<IHasManyMapping> _collections = new();
    private readonly List<IKeyMapping> _keys;
    private readonly List<IPropertyMapping> _properties;
    private bool _checkedConstructors = false;
    private Func<IDataRecord, object>? _itemFactory;
    /// <summary>
    /// </summary>
    /// <param name="entityType">Type of entity that the mapping is for.</param>
    /// <param name="tableName">Table that the entity is stored in.</param>
    /// <param name="keys">Keys used to be able to find a specific entity.</param>
    /// <param name="properties">All properties to read data into.</param>
    public ClassMapping(
        Type entityType,
        string tableName,
        List<IKeyMapping> keys,
        List<IPropertyMapping> properties)
    {
        EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
        TableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
        _properties = properties ?? throw new ArgumentNullException(nameof(properties));
        _keys = keys ?? throw new ArgumentNullException(nameof(keys));
    }

    /// <summary>
    ///     One to one relations.
    /// </summary>
    public IReadOnlyList<IHasOneMapping> Children => _children;

    /// <summary>
    ///     One to many relations.
    /// </summary>
    public IReadOnlyList<IHasManyMapping> Collections => _collections;

    /// <summary>
    ///     Class that this is a mapping for.
    /// </summary>
    public Type EntityType { get; }

    /// <summary>
    ///     Keys used to identify a specific entity.
    /// </summary>
    public IReadOnlyList<IKeyMapping> Keys => _keys;

    /// <summary>
    ///     Properties to fill with data.
    /// </summary>
    public IReadOnlyList<IPropertyMapping> Properties => _properties;

    /// <summary>
    ///     Table that the entity is stored in.
    /// </summary>
    public string TableName { get; set; }

    /// <summary>
    /// </summary>
    /// <param name="collections">One to many relationships.</param>
    /// <param name="children">One to one relationships.</param>
    public void AddRelations(IEnumerable<IHasManyMapping> collections, IEnumerable<IHasOneMapping> children)
    {
        _children.AddRange(children);
        _collections.AddRange(collections);
    }

    /// <summary>
    ///     Create an instance using the data record (which allows us to use non default constructors).
    /// </summary>
    /// <param name="record">Record set.</param>
    /// <returns>Created entity.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public object CreateInstance(IDataRecord record)
    {
        if (record == null)
        {
            throw new ArgumentNullException(nameof(record));
        }

        if (_itemFactory != null)
        {
            return _itemFactory(record);
        }

        if (_checkedConstructors)
        {
            return Activator.CreateInstance(EntityType, true);
        }

        _checkedConstructors = true;

        var result = GetConstructor();
        if (result == null)
        {
            return Activator.CreateInstance(EntityType, true);
        }

        var param = Expression.Parameter(typeof(IDataRecord), "t");
        var property = typeof(IDataRecord).GetProperties().FirstOrDefault(x =>
            x.GetIndexParameters().Length == 1 && x.GetIndexParameters()[0].ParameterType == typeof(string));
        if (property == null)
        {
            throw new InvalidOperationException("Failed to find indexer method in IDataRecord");
        }

        var constructor = result.Value.Item1;
        List<UnaryExpression> constructorArguments = new List<UnaryExpression>();
        foreach (var mapping in result.Value.Item2)
        {
            var columnNameConstant = Expression.Constant(mapping.ColumnName, typeof(string));
            var indexerAccessor = Expression.Property(param, property, columnNameConstant);

            UnaryExpression converted;
            var converter = mapping.GetType().GetProperty("ColumnToPropertyConverter", BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)?.GetValue(mapping);
            if (converter != null)
            {
                var method = ((Delegate)converter).Method;
                var target = ((Delegate)converter).Target;
                var instance = Expression.Constant(target);
                var converterMethod = Expression.Call(instance, method, indexerAccessor);
                converted=Expression.Convert(converterMethod, mapping.PropertyType);
            }
            else
            {
                converted = Expression.Convert(indexerAccessor, mapping.PropertyType);
            }
            
            constructorArguments.Add(converted);
        }

        var constructorExpression = Expression.New(constructor, constructorArguments);
        _itemFactory = Expression.Lambda<Func<IDataRecord, object>>(constructorExpression, param).Compile();
        try
        {
            return _itemFactory(record);
        }
        catch (Exception ex)
        {
            throw new MappingConfigurationException(EntityType,
                "Failed to create an instance of entity using non-default constructor.", ex);
        }
    }

    private (ConstructorInfo, List<IFieldMapping>)? GetConstructor()
    {
        foreach (var constructor in EntityType.GetConstructors(BindingFlags.NonPublic|BindingFlags.Public|BindingFlags.Instance))
        {
            List<IFieldMapping> properties = new List<IFieldMapping>();
            var parameters = constructor.GetParameters();
            foreach (var parameter in constructor.GetParameters())
            {
                var prop = FindPropertyByName(parameter.Name);
                if (prop != null)
                {
                    properties.Add(prop);
                }
                else
                {
                    break;
                }
            }

            if (parameters.Length == properties.Count)
            {
                return (constructor, properties);
            }
        }

        return null;
    }
    
    /// <summary>
    ///     Find a specific property (looks in keys and properties for the given name, using case insensitive search).
    /// </summary>
    /// <param name="propertyOrColumnName">Property or column name.</param>
    /// <returns>Property if found; otherwise <c>null</c>.</returns>
    public IFieldMapping? FindPropertyByName(string propertyOrColumnName)
    {
        if (propertyOrColumnName == null)
        {
            throw new ArgumentNullException(nameof(propertyOrColumnName));
        }

        return (IFieldMapping?)Properties.FirstOrDefault(x =>
                   x.PropertyName.Equals(propertyOrColumnName, StringComparison.OrdinalIgnoreCase) ||
                   x.ColumnName.Equals(propertyOrColumnName, StringComparison.OrdinalIgnoreCase))
               ?? Keys.FirstOrDefault(x =>
                   x.PropertyName.Equals(propertyOrColumnName, StringComparison.OrdinalIgnoreCase) ||
                   x.ColumnName.Equals(propertyOrColumnName, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>
    ///     Get a specific property (looks in keys and properties for the given name, using case insensitive search).
    /// </summary>
    /// <param name="propertyName">Property name.</param>
    /// <returns>Property</returns>
    /// <exception cref="InvalidOperationException">Thrown if the given name is not found.</exception>
    public IFieldMapping GetProperty(string propertyName)
    {
        return (IFieldMapping?)Keys.FirstOrDefault(x => x.PropertyName.Equals(propertyName)) ??
               Properties.FirstOrDefault(x => x.PropertyName.Equals(propertyName)) ??
               throw new MappingException(EntityType,
                   $"Failed to find property {propertyName}.");
    }

    /// <summary>
    /// Get the relationship configuration for a specific child entity.
    /// </summary>
    /// <param name="childType">Type of child entity.</param>
    /// <returns>Mapping if found; otherwise <c>null</c>.</returns>
    public IRelationShip? GetRelation(Type childType)
    {
        if (childType == null)
        {
            throw new ArgumentNullException(nameof(childType));
        }
        // We need this loop since mappings are registered using the base type
        // while child properties contains a concrete instances.

        var type = childType;
        while (type != null)
        {
            var relation = (IRelationShip?)_children.FirstOrDefault(x => x.ChildEntityType == type)
                           ?? _collections.FirstOrDefault(x => x.ChildEntityType == type);
            if (relation != null)
            {
                return relation;
            }

            type = type.BaseType;
        }

        // The base type can also be an interface.
        foreach (var @interface in childType.GetInterfaces())
        {
            var relation = (IRelationShip?)_children.FirstOrDefault(x => x.ChildEntityType == @interface)
                           ?? _collections.FirstOrDefault(x => x.ChildEntityType == @interface);
            if (relation != null)
            {
                return relation;
            }
        }

        return null;
    }

    /// <inheritdoc />
    public override string ToString()
    {
        return EntityType.Name;
    }
}

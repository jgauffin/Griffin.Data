using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Griffin.Data.Configuration;
using Griffin.Data.Mapper.Mappings.Properties;
using Griffin.Data.Mapper.Mappings.Relations;

namespace Griffin.Data.Mapper.Mappings;

/// <summary>
///     Mapping for a specific type of entity.
/// </summary>
public class ClassMapping
{
    private readonly List<IHasOneMapping> _children = new();
    private readonly List<IHasManyMapping> _collections = new();
    private readonly List<IKeyMapping> _keys;
    private readonly List<IPropertyMapping> _properties;
    private bool _checkedConstructors;
    private Func<object>? _defaultConstructorFactory;
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
            return _defaultConstructorFactory!();
        }

        _checkedConstructors = true;

        var builder = new ConstructorBuilder(this);
        var cons = builder.CreateConstructor();
        if (cons == null)
        {
            _defaultConstructorFactory = builder.CreateDefaultConstructor();
            return _defaultConstructorFactory!();
        }

        _itemFactory = cons;
        try
        {
            return _itemFactory(record);
        }
        catch (GriffinException)
        {
            throw;
        }
        catch (Exception ex)
        {
            throw new MappingConfigurationException(EntityType,
                "Failed to create an instance of entity using non-default constructor.", ex);
        }
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
    ///     Get the relationship configuration for a specific child entity.
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

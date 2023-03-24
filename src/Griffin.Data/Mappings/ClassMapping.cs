using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Griffin.Data.Mappings.Properties;
using Griffin.Data.Mappings.Relations;

namespace Griffin.Data.Mappings;

public class ClassMapping
{
    private readonly List<HasOneMapping> _children;
    private readonly List<HasManyMapping> _collections;
    private readonly List<KeyMapping> _keys;
    private readonly List<PropertyMapping> _properties;

    public ClassMapping(Type entityType, string tableName, List<PropertyMapping> properties, List<KeyMapping> keys,
        List<HasManyMapping> collections, List<HasOneMapping> children)
    {
        EntityType = entityType;
        TableName = tableName;
        _properties = properties;
        _keys = keys;
        _collections = collections;
        _children = children;
    }

    /// <summary>
    ///     Class that this is a mapping for.
    /// </summary>
    public Type EntityType { get; }

    public IReadOnlyList<PropertyMapping> Properties => _properties;
    public IReadOnlyList<KeyMapping> Keys => _keys;
    public IReadOnlyList<HasManyMapping> Collections => _collections;
    public IReadOnlyList<HasOneMapping> Children => _children;
    public string TableName { get; set; }

    public void Map(IDataRecord record, object entity)
    {
        foreach (var mapping in _properties)
        {
            var value = record[mapping.ColumnName];
            if (value is DBNull) continue;

            mapping.SetColumnValue(entity, value);
        }
    }

    public void AddCommandParameters(object entity, IDbCommand command)
    {
        foreach (var mapping in _properties)
        {
        }
    }

    public IPropertyAccessor GetProperty(string propertyName)
    {
        return (IPropertyAccessor?)Keys.FirstOrDefault(x => x.PropertyName.Equals(propertyName)) ??
               Properties.FirstOrDefault(x => x.PropertyName.Equals(propertyName)) ??
               throw new InvalidOperationException(
                   $"Failed to find property {propertyName} in entity {EntityType.Name}.");
    }

    public PropertyMapping? FindPropertyByName(string propertyOrColumnName)
    {
        return Properties.FirstOrDefault(x =>
            x.PropertyName.Equals(propertyOrColumnName, StringComparison.OrdinalIgnoreCase) ||
            x.ColumnName.Equals(propertyOrColumnName, StringComparison.OrdinalIgnoreCase));
    }
}
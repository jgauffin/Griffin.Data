using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Griffin.Data.Configuration;
using Griffin.Data.Mapper;
using Griffin.Data.Mapper.Mappings.Properties;
using Griffin.Data.Mapper.Mappings.Relations;
using static System.String;

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

        var result = GetConstructor();
        if (result == null)
        {
            CreateDefaultConstructor();
            return _defaultConstructorFactory!();
        }

        var param = Expression.Parameter(typeof(IDataRecord), "t");
        var property = typeof(IDataRecord).GetProperties().FirstOrDefault(x =>
            x.GetIndexParameters().Length == 1 && x.GetIndexParameters()[0].ParameterType == typeof(string));
        if (property == null)
        {
            throw new InvalidOperationException("Failed to find indexer method in IDataRecord");
        }

        var constructor = result.Value.Item1;
        var constructorArguments = new List<Expression>();
        foreach (var mapping in result.Value.Item2)
        {
            var columnNameConstant = Expression.Constant(mapping.ColumnName, typeof(string));
            var value = Expression.Variable(typeof(object));

            var indexerAccessor = Expression.Property(param, property, columnNameConstant);

            var dbNullRemoved = Expression.IfThenElse(
                Expression.Equal(indexerAccessor, Expression.Constant(DBNull.Value)),
                Expression.Assign(value, Expression.Constant(null)),
                Expression.Assign(value, indexerAccessor));

            Expression converted;
            var converter = mapping.GetType().GetProperty("ColumnToPropertyConverter",
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance)?.GetValue(mapping);
            if (converter != null)
            {
                var method = ((Delegate)converter).Method;
                var target = ((Delegate)converter).Target;
                var instance = Expression.Constant(target);
                var converterMethod = Expression.Call(instance, method, indexerAccessor);
                converted = Expression.Convert(converterMethod, mapping.PropertyType);
            }
            else
            {
                converted = Expression.Convert(indexerAccessor, mapping.PropertyType);
            }

            var exceptionMethod = GetType().GetMethod(nameof(ThrowConstructArgumentException),
                BindingFlags.NonPublic | BindingFlags.Instance)!.MakeGenericMethod(mapping.PropertyType);
            
            
            var exception = Expression.Variable(typeof(Exception), "exception");
            //var catchAll = Expression.Catch(
            //    exception,
            //    Expression.Block(
            //        Expression.Call(
            //            Expression.Constant(this), 
            //            exceptionMethod,
            //            exception,
            //            Expression.Constant(mapping),
            //            Expression.Constant(record))));
            var catchAll = Expression.Catch(
                exception,
                    Expression.Call(
                        Expression.Constant(this),
                        exceptionMethod,
                        exception,
                        Expression.Constant(mapping),
                        Expression.Constant(record)));

            var block = Expression.Block(dbNullRemoved, converted);
            Expression triedExpr = Expression.TryCatch(converted, catchAll);

            constructorArguments.Add(triedExpr);//converted, och ändrade från UnaryExpression to Expression för listan.
        }

        var constructorExpression = Expression.New(constructor, constructorArguments);
        _itemFactory = Expression.Lambda<Func<IDataRecord, object>>(constructorExpression, param).Compile();
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

    private TProperty ThrowConstructArgumentException<TProperty>(Exception exception, IFieldMapping mapping, IDataRecord record)
    {
        var values = "";
        for (int i = 0; i < record.FieldCount; i++)
        {
            var value = record.GetValue(i);
            if (value is null or DBNull)
            {
                values += $"{record.GetName(i)}: null, ";
            }
            else
            {
                values += $"{record.GetName(i)}: {value}, ";
            }
            
        }

        if (values.Length > 2)
        {
            values = values.Remove(values.Length - 2, 2);
        }
        
        var failingValue = record[mapping.ColumnName];
        var ex = new MappingException(EntityType,
            $"Property {mapping.PropertyName} cannot be cast from '{(failingValue?.GetType().Name ?? "null")}' to {mapping.PropertyType}.\r\nAll fields: {values}\r\nInner error: " + exception.Message);
        throw ex;
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

    private void CreateDefaultConstructor()
    {
        var defaultConstructor =
            EntityType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public, null, Type.EmptyTypes, null);
        if (defaultConstructor == null)
        {
            throw new MappingConfigurationException(EntityType,
                "There is no default constructor nor a constructor where arguments matches properties. Can therefore not instantiate the entity.");
        }

        var newExpression = Expression.New(defaultConstructor);
        _defaultConstructorFactory = Expression.Lambda<Func<object>>(newExpression).Compile();
    }

    private (ConstructorInfo, List<IFieldMapping>)? GetConstructor()
    {
        foreach (var constructor in EntityType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public |
                                                               BindingFlags.Instance))
        {
            var properties = new List<IFieldMapping>();
            var parameters = constructor.GetParameters();
            foreach (var parameter in constructor.GetParameters())
            {
                if (IsNullOrEmpty(parameter.Name))
                {
                    continue;
                }

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
}

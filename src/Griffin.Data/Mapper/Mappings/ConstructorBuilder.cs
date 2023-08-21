using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Griffin.Data.Configuration;
using Griffin.Data.Mapper.Mappings.Properties;

namespace Griffin.Data.Mapper.Mappings;

internal class ConstructorBuilder
{
    private readonly ClassMapping _classMapping;
    private readonly Type _entityType;

    public ConstructorBuilder(ClassMapping classMapping)
    {
        _classMapping = classMapping;
        _entityType = classMapping.EntityType;
    }

    /// <summary>
    ///     Create an instance using the data record (which allows us to use non default constructors).
    /// </summary>
    /// <returns>Created entity.</returns>
    /// <exception cref="ArgumentNullException"></exception>
    public Func<IDataRecord, object>? CreateConstructor()
    {
        var result = GetConstructor();
        if (result == null)
        {
            return null;
        }

        var recordParameter = Expression.Parameter(typeof(IDataRecord), "dataRecord");
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

            var indexerAccessor = Expression.Property(recordParameter, property, columnNameConstant);

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
            var catchAll = Expression.Catch(
                exception,
                Expression.Call(
                    Expression.Constant(this),
                    exceptionMethod,
                    exception,
                    Expression.Constant(mapping),
                    recordParameter));

            var block = Expression.Block(dbNullRemoved, converted);
            Expression triedExpr = Expression.TryCatch(converted, catchAll);

            constructorArguments.Add(triedExpr); //converted, och ändrade från UnaryExpression to Expression för listan.
        }

        var constructorExpression = Expression.New(constructor, constructorArguments);
        return Expression.Lambda<Func<IDataRecord, object>>(constructorExpression, recordParameter).Compile();
    }

    public Func<object> CreateDefaultConstructor()
    {
        var defaultConstructor =
            _entityType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public, null, Type.EmptyTypes, null);
        if (defaultConstructor == null)
        {
            throw new MappingConfigurationException(_entityType,
                "There is no default constructor nor a constructor where arguments matches properties. Can therefore not instantiate the entity.");
        }

        var newExpression = Expression.New(defaultConstructor);
        return Expression.Lambda<Func<object>>(newExpression).Compile();
    }

    private (ConstructorInfo, List<IFieldMapping>)? GetConstructor()
    {
        foreach (var constructor in _entityType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public |
                                                                BindingFlags.Instance))
        {
            var properties = new List<IFieldMapping>();
            var parameters = constructor.GetParameters();
            foreach (var parameter in parameters)
            {
                if (string.IsNullOrEmpty(parameter.Name))
                {
                    continue;
                }

                var prop = _classMapping.FindPropertyByName(parameter.Name);
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

    private TProperty ThrowConstructArgumentException<TProperty>(
        Exception exception,
        IFieldMapping mapping,
        IDataRecord record)
    {
        var values = "";
        for (var i = 0; i < record.FieldCount; i++)
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
        var ex = new MappingException(_entityType,
            $"Property {mapping.PropertyName} cannot be cast from '{failingValue?.GetType().Name ?? "null"}' to {mapping.PropertyType}.\r\nAll fields: {values}\r\nInner error: " +
            exception.Message);
        throw ex;
    }
}

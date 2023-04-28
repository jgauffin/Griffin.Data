using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Griffin.Data.Configuration;

namespace Griffin.Data.ChangeTracking.Services.Implementations;

internal class CopyConstructorFactory
{
    public Func<object, object> CreateCopyConstructor(Type entityType)
    {
        var result = GetConstructor(entityType);
        if (result == null)
        {
            var item = CreateDefaultConstructor(entityType);
            return item;
        }

        var objectInstance = Expression.Parameter(typeof(object), "instance");
        var castedInstance = Expression.Convert(objectInstance, entityType);
        
        //var instanceExpression = Expression.TypeAs(objectInstance, entityType);
        
        var constructor = result.Value.Item1;
        var constructorArguments = new List<UnaryExpression>();
        foreach (var mapping in result.Value.Item2)
        {
            UnaryExpression converted;
            if (mapping is PropertyInfo pi)
            {
                var memberAccessor = Expression.Property(castedInstance, pi);
                converted = Expression.Convert(memberAccessor, pi.PropertyType);

            }
            else
            {
                var memberAccessor = mapping.MemberType == MemberTypes.Property
                    ? Expression.Property(castedInstance, (PropertyInfo)mapping)
                    : Expression.Field(castedInstance, (FieldInfo)mapping);

                var type = mapping is PropertyInfo p ? p.PropertyType : ((FieldInfo)mapping).FieldType;

                converted = Expression.Convert(memberAccessor, type);
            }

            constructorArguments.Add(converted);
        }

        var constructorExpression = Expression.New(constructor, constructorArguments);
        var itemFactory = Expression.Lambda<Func<object, object>>(constructorExpression, objectInstance).Compile();
        try
        {
            return itemFactory;
        }
        catch (Exception ex)
        {
            throw new MappingConfigurationException(entityType,
                "Failed to create an instance of entity using non-default constructor.", ex);
        }
    }

    private Func<object, object> CreateDefaultConstructor(Type entityType)
    {
        var defaultConstructor =
            entityType.GetConstructor(BindingFlags.NonPublic | BindingFlags.Public, null, Type.EmptyTypes, null);
        if (defaultConstructor == null)
        {
            throw new MappingConfigurationException(entityType,
                $"Type[{entityType}]: There is no default constructor nor a constructor where arguments matches properties. Can therefore not instantiate the entity.");
        }

        var newExpression = Expression.New(defaultConstructor);
        var inner = Expression.Lambda<Func<object>>(newExpression).Compile();
        return _ => inner();
    }

    private MemberInfo? FindPropertyByName(Type entityType, string parameterName)
    {
        MemberInfo? field = entityType
            .GetProperties(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(x => x.CanRead && x.Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase));
        if (field != null)
        {
            return field;
        }

        field = entityType.GetFields(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Public)
            .FirstOrDefault(x =>
                x.Name.Equals("_" + parameterName, StringComparison.OrdinalIgnoreCase) ||
                x.Name.Equals(parameterName, StringComparison.OrdinalIgnoreCase));
        if (field != null)
        {
            return field;
        }

        return null;
    }

    private (ConstructorInfo, List<MemberInfo>)? GetConstructor(Type entityType)
    {
        foreach (var constructor in entityType.GetConstructors(BindingFlags.NonPublic | BindingFlags.Public |
                                                               BindingFlags.Instance))
        {
            var properties = new List<MemberInfo>();
            var parameters = constructor.GetParameters();
            foreach (var parameter in constructor.GetParameters())
            {
                if (string.IsNullOrEmpty(parameter.Name))
                {
                    continue;
                }

                var prop = FindPropertyByName(entityType, parameter.Name);
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

using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Griffin.Data.Helpers;

/// <summary>
///     Extensions used when getting information from expressions.
/// </summary>
public static class ExpressionExtensions
{
    /// <summary>
    ///     Get property/field name from an expression.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TProperty"></typeparam>
    /// <param name="property"></param>
    /// <returns></returns>
    /// <exception cref="ArgumentException"></exception>
    /// <exception cref="InvalidOperationException"></exception>
    public static string GetMemberName<TEntity, TProperty>(this Expression<Func<TEntity, TProperty>> property)
    {
        var type = typeof(TEntity);

        MemberExpression? exp = null;

        if (property.Body is UnaryExpression c)
            // Convert()
        {
            exp = (MemberExpression)c.Operand;
        }
        else if (property.Body is MemberExpression x)
        {
            exp = x;
        }

        var propInfo = exp?.Member as PropertyInfo;
        if (propInfo == null)
        {
            throw new ArgumentException($"Expression '{property}' do not refer to a property.");
        }

        if (propInfo.ReflectedType == null)
        {
            throw new InvalidOperationException($"Cannot find reflected type for property '{propInfo.Name}'.");
        }

        if (type != propInfo.ReflectedType &&
            !type.IsSubclassOf(propInfo.ReflectedType))
        {
            throw new ArgumentException($"Expression '{property}' refers to a property that is not from type {type}.");
        }

        return propInfo.Name;
    }

    public static PropertyInfo GetPropertyInfo<TEntity, TProperty>(this Expression<Func<TEntity, TProperty>> property)
    {
        var type = typeof(TEntity);

        if (property.Body is not MemberExpression member)
        {
            throw new ArgumentException($"Expression '{property}' refers to a method, not a property.");
        }

        var propInfo = member.Member as PropertyInfo;
        if (propInfo == null)
        {
            throw new ArgumentException($"Expression '{property}' refers to a field, not a property.");
        }

        if (propInfo.ReflectedType == null)
        {
            throw new InvalidOperationException($"Cannot find reflected type for property '{propInfo.Name}'.");
        }

        if (type != propInfo.ReflectedType &&
            !type.IsSubclassOf(propInfo.ReflectedType))
        {
            throw new ArgumentException($"Expression '{property}' refers to a property that is not from type {type}.");
        }

        return propInfo;
    }
}

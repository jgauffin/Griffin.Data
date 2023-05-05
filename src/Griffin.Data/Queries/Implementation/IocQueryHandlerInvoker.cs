using System;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace Griffin.Data.Queries.Implementation;

/// <summary>
///     An query runner invoker which uses the Microsoft DI abstraction library to find query runners.
/// </summary>
/// <remarks>
///     <para>
///         Register this class in your container and all query runners for it to work properly. Do note that the
///         <c>Session.Query</c> method is not available when using this runner. Instead, use
///         <see cref="IQueryHandlerInvoker" /> interface in your classes that need to invoke queries.
///     </para>
/// </remarks>
public class IocQueryHandlerInvoker : IQueryHandlerInvoker
{
    private readonly MethodInfo _innerMethod;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// </summary>
    /// <param name="serviceProvider">Scoped DI container.</param>
    public IocQueryHandlerInvoker(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        var innerMethod = GetType().GetMethod("ExecuteInner", BindingFlags.NonPublic|BindingFlags.Instance);
        if (innerMethod == null)
        {
            throw new InvalidOperationException("Internal error. Failed to find ExecuteInner.");
        }

        _innerMethod = innerMethod;
    }

    /// <inheritdoc />
    public async Task<TQueryResult> Execute<TQueryResult>(IQuery<TQueryResult> query)
    {
        if (query == null)
        {
            throw new ArgumentNullException(nameof(query));
        }

        var method = _innerMethod.MakeGenericMethod(query.GetType(), typeof(TQueryResult));
        return await (Task<TQueryResult>)method.Invoke(this, new object[] { query })!;
    }

    private async Task<TQueryResult> ExecuteInner<TQuery, TQueryResult>(TQuery query)
        where TQuery : IQuery<TQueryResult>
    {
        var handler = _serviceProvider.GetService<IQueryHandler<TQuery, TQueryResult>>();
        if (handler == null)
        {
            throw new InvalidOperationException($"Failed to find a query runner for {query.GetType()}.");
        }

        return await handler.Execute(query).ConfigureAwait(false);
    }

    //static Func<object, Task<object>> CreateLambda(Type queryType, Type queryResultType)
    //{
    //    var serviceProvider = Expression.Parameter(typeof(IServiceProvider), "serviceProvider");

    //    var queryHandlerType = typeof(IQueryHandler<,>).MakeGenericType(queryType, queryResultType);
    //    var resolveMethod = typeof(ServiceProviderServiceExtensions)
    //        .GetMethod("GetService", BindingFlags.Static | BindingFlags.Public)
    //        .MakeGenericMethod(queryHandlerType);
    //    var resolveMethodCall = Expression.Call(null, resolveMethod, serviceProvider);
    //    //var invokerInstance = Expression.Assign(serviceProvider, resolveMethodCall)

    //    var queryParameter = Expression.Parameter(queryType, "query");

    //    var executeMethod = queryHandlerType.GetMethod("Execute").MakeGenericMethod(queryResultType);

    //    var call = Expression.Call(resolveMethodCall, executeMethod, queryParameter);

    //    return Expression.Lambda(call, source)
    //}
}

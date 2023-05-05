using System;
using System.Data;
using System.Reflection;
using Griffin.Data.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Griffin.Data.Queries.Implementation;

/// <summary>
///     Extensions for Griffin.Data and DI.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    ///     Add the IoC implementation for <see cref="IQueryHandlerInvoker" />.
    /// </summary>
    /// <param name="services">Service collection</param>
    /// <param name="level">Isolation level to use.</param>
    /// <remarks>
    ///     <para>
    ///         Registers both <see cref="QuerySession" /> and <see cref="IocQueryHandlerInvoker" />.
    ///     </para>
    /// </remarks>
    public static void AddQueryHandlerInvoker(
        this ServiceCollection services,
        IsolationLevel level = IsolationLevel.ReadCommitted)
    {
        services.AddScoped(x =>
        {
            var connection = x.GetRequiredService<IDbConnection>();
            var transaction = connection.BeginTransaction(level);
            var config = x.GetRequiredService<DbConfiguration>();
            return new QuerySession(transaction, config.MappingRegistry, config.Dialect!,
                config.ChangeTrackerFactory?.Invoke());
        });
        services.AddScoped<IQueryHandlerInvoker, IocQueryHandlerInvoker>();
    }

    /// <summary>
    ///     Add all found runners in the given assembly.
    /// </summary>
    /// <param name="collection">DI service collection to register runners in.</param>
    /// <param name="assembly">Assembly to scan.</param>
    /// <example>
    ///     <code>
    /// services.AddQueryHandlers(typeof(MyQueryRunner).Assembly);
    ///  </code>
    /// </example>
    public static void AddQueryHandlers(this IServiceCollection collection, Assembly assembly)
    {
        if (collection == null)
        {
            throw new ArgumentNullException(nameof(collection));
        }

        if (assembly == null)
        {
            throw new ArgumentNullException(nameof(assembly));
        }

        var finder = new QueryHandlerFinder();
        finder.FindHandlers(assembly, result =>
        {
            collection.AddScoped(result.interfaceType, result.handlerType);
        });
    }
}

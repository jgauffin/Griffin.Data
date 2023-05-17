using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Griffin.Data.Helpers;

namespace Griffin.Data.Queries.Implementation;

/// <summary>
///     Query invokers that scans all loaded assemblies after implementations of
///     <see cref="IQueryHandler{TQuery,TQueryResult}" />.
/// </summary>
/// <remarks>
///     <para>
///         This implementation requires that the query runner implementations takes a single parameter of type
///         <see cref="Session" /> in the constructor. For other options use the <see cref="IocQueryHandlerInvoker" />
///         instead.
///     </para>
///     <para>
///         This class will attempt to load all query runners from all loaded assemblies. It requires that all query runner
///         assemblies have been loaded bore this class is used. If it fails to find runners, scan assemblies explicitly by
///         calling the static method <see cref="ScanAssembly" />.
///     </para>
/// </remarks>
public class StandAloneQueryHandlerInvoker : IQueryHandlerInvoker
{
    private static readonly Dictionary<Type, ConstructorInfo> RunnerTypes = new();
    private static readonly List<Assembly> ScannedAssemblies = new();
    private readonly MethodInfo _innerMethod;
    private readonly Session _session;

    /// <summary>
    /// </summary>
    /// <param name="session"></param>
    /// <exception cref="ArgumentNullException"></exception>
    public StandAloneQueryHandlerInvoker(Session session)
    {
        _session = session ?? throw new ArgumentNullException(nameof(session));
        var innerMethod = GetType().GetMethod("ExecuteInner", BindingFlags.NonPublic | BindingFlags.Instance);
        if (innerMethod == null)
        {
            throw new InvalidOperationException("Internal error. Failed to find ExecuteInner.");
        }

        _innerMethod = innerMethod;
    }

    /// <inheritdoc />
    public async Task<TQueryResult> Execute<TQueryResult>(IQuery<TQueryResult> query)
    {
        EnsureAssemblies();

        var queryType = query.GetType();

        if (!RunnerTypes.TryGetValue(queryType, out var constructorInfo))
        {
            var runnerNames = string.Join(", ", RunnerTypes.Select(x => x.Value.Name));
            throw new InvalidOperationException(
                $"Failed to get a query runner for {queryType}. Registered runners: {runnerNames}. Invoke ManualQueryInvoker.ScanAssembly() to manually register assemblies with query runners. Or ensure that all assemblies have been loaded before ManualQueryInvoker is used.");
        }

        var instance = constructorInfo.Invoke(new object[] { _session });

        try
        {
            var method = _innerMethod.MakeGenericMethod(query.GetType(), typeof(TQueryResult));
            return await (Task<TQueryResult>)method.Invoke(this, new[] { instance, query })!;
        }
        catch (GriffinException)
        {
            throw;
        }
        catch (Exception ex)
        {
            var parameters = query.ToDictionary();
            var ps = string.Join(", ", parameters.Select(x => $"{x.Key}: {x.Value}"));
            throw new DataException($"Failed to execute query {queryType.Name}({ps})", ex);
        }
    }

    /// <summary>
    ///     Add a handler.
    /// </summary>
    /// <param name="handlerType">Type of handler.</param>
    public static void AddHandlerType(Type handlerType)
    {
        if (handlerType == null)
        {
            throw new ArgumentNullException(nameof(handlerType));
        }

        var ifs = handlerType.GetInterfaces().Where(x => x.Name.StartsWith("IQueryHandler")).ToList();
        if (!ifs.Any())
        {
            throw new InvalidOperationException($"{handlerType} is not a IQueryHandler.");
        }

        foreach (var interfaceType in ifs)
        {
            var parameters = interfaceType.GetGenericArguments();
            var constructor = GetHandlerConstructor(handlerType);
            RunnerTypes[parameters[0]] = constructor;
        }
    }

    /// <summary>
    ///     Scan assemblies to find query runners.
    /// </summary>
    /// <param name="assembly">Assembly to scan.</param>
    /// <exception cref="InvalidOperationException">
    ///     Found a query runner which does not have a single constructor with an
    ///     argument of type Session.
    /// </exception>
    public static void ScanAssembly(Assembly assembly)
    {
        if (ScannedAssemblies.Contains(assembly))
        {
            return;
        }

        ScannedAssemblies.Add(assembly);

        var finder = new QueryHandlerFinder();
        finder.FindHandlers(assembly, result =>
        {
            var queryType = result.interfaceType.GetGenericArguments()[0];
            var constructor = GetHandlerConstructor(result.handlerType);
            RunnerTypes[queryType] = constructor;
        });
    }

    private static void EnsureAssemblies()
    {
        if (RunnerTypes.Any())
        {
            return;
        }

        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            ScanAssembly(assembly);
        }
    }

    private async Task<TQueryResult> ExecuteInner<TQuery, TQueryResult>(object instance, TQuery query)
        where TQuery : IQuery<TQueryResult>
    {
        return await ((IQueryHandler<TQuery, TQueryResult>)instance).Execute(query).ConfigureAwait(false);
    }

    private static ConstructorInfo GetHandlerConstructor(Type handlerType)
    {
        var constructor = handlerType.GetConstructor(new[] { typeof(Session) });
        if (constructor != null)
        {
            return constructor;
        }

        throw new InvalidOperationException(
            "Query runners may only have one constructor, and it must take a single argument of type Session. Incorrect QueryRunner: " +
            handlerType.FullName);
    }
}

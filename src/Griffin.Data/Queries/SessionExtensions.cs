using System.Threading.Tasks;
using Griffin.Data.Queries.Implementation;

namespace Griffin.Data.Queries;

/// <summary>
///     Extensions for queries.
/// </summary>
public static class SessionExtensions
{
    /// <summary>
    ///     Invoke a query.
    /// </summary>
    /// <typeparam name="TQueryResult">Type of result returned from a query.</typeparam>
    /// <param name="session">Session to run queries in.</param>
    /// <param name="query">Query to invoke.</param>
    /// <returns>Query result</returns>
    /// <remarks>
    ///     <para>
    ///         Uses <see cref="StandAloneQueryHandlerInvoker" /> which means that you must have registered all query runner
    ///         assemblies in it.
    ///     </para>
    /// </remarks>
    public static async Task<TQueryResult> Query<TQueryResult>(this Session session, IQuery<TQueryResult> query)
    {
        var runner = new StandAloneQueryHandlerInvoker(session);
        return await runner.Execute(query);
    }
}

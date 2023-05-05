using System.Threading.Tasks;

namespace Griffin.Data.Queries.Implementation;

/// <summary>
///     Defines class used to invoke query runners.
/// </summary>
public interface IQueryHandlerInvoker
{
    /// <summary>
    ///     Execute a query runner.
    /// </summary>
    /// <typeparam name="TQueryResult">Result returned from the query runner.</typeparam>
    /// <param name="query">Query to invoke.</param>
    /// <returns>Result.</returns>
    Task<TQueryResult> Execute<TQueryResult>(IQuery<TQueryResult> query);
}

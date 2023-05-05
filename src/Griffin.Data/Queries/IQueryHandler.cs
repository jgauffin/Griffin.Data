using System.Threading.Tasks;

namespace Griffin.Data.Queries;

/// <summary>
///     Defines a class that can invoke a query and generate a result.
/// </summary>
/// <typeparam name="TQuery">Type of query.</typeparam>
/// <typeparam name="TQueryResult">Type of result that the query produces.</typeparam>
/// <remarks>
///     <para>
///         Queries that generate a list result should always return something (an empty list). Single result queries
///         should only return <c>null</c> when the query parameters might result in zero results, otherwise it should
///         throw an exception when a result is not found.
///     </para>
/// </remarks>
public interface IQueryHandler<in TQuery, TQueryResult> where TQuery : IQuery<TQueryResult>
{
    /// <summary>
    ///     Execute query.
    /// </summary>
    /// <param name="query">Query to execute.</param>
    /// <returns>Generated result.</returns>
    Task<TQueryResult> Execute(TQuery query);
}

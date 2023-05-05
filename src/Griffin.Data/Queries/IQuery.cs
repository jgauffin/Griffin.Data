namespace Griffin.Data.Queries;

/// <summary>
///     Query interface.
/// </summary>
/// <typeparam name="TResult">Type of result</typeparam>
/// <remarks>
///     <para>
///         Used to define a query (the API and not the actual execution). See implementations as a request to execute a
///         query. The query itself will be invoked by a <see cref="IQueryHandler{TQuery,TQueryResult}" /> implementation
///         which in turn will return the result.
///     </para>
/// </remarks>
public interface IQuery<out TResult>
{
}

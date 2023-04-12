using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Griffin.Data.Mapper;
using Griffin.Data.Mappings;
using Griffin.Data.Queries;

namespace Griffin.Data.Dialects;

/// <summary>
///     Abstraction for different database engine.
/// </summary>
public interface ISqlDialect
{
    /// <summary>
    ///     Apply paging to a query.
    /// </summary>
    /// <param name="command">Command to adjust SQL query in.</param>
    /// <param name="keyColumnName">Column to sort by (when an ORDER BY is missing).</param>
    /// <param name="pageNumber">One based index.</param>
    /// <param name="pageSize">Number of items per page.</param>
    /// <remarks>
    ///     <para>
    ///         Paging should always be applied after Sorting (when both are used) since paging requires an ORDER BY claus in
    ///         certain DB engine (and this method will add one if missing).
    ///     </para>
    /// </remarks>
    void ApplyPaging(IDbCommand command, string keyColumnName, int pageNumber, int? pageSize);

    /// <summary>
    ///     Apply options to a business entity query (i.e there is a defined mapping for it).
    /// </summary>
    /// <param name="mapping">Mapping to use.</param>
    /// <param name="command">Command to modify.</param>
    /// <param name="options">Options to apply (paging and sorting should be applied when specified).</param>
    void ApplyQueryOptions(ClassMapping mapping, DbCommand command, QueryOptions options);

    /// <summary>
    ///     Apply sorting.
    /// </summary>
    /// <param name="command">Command to adjust.</param>
    /// <param name="entries">Sort entries in order.</param>
    void ApplySorting(IDbCommand command, IList<SortEntry> entries);

    /// <summary>
    ///     Execute an INSERT statement.
    /// </summary>
    /// <param name="mapping">Mapping for the entity.</param>
    /// <param name="entity">Entity being inserted.</param>
    /// <param name="command">Command that contains a generated INSERT statement</param>
    /// <returns>Task.</returns>
    /// <remarks>
    ///     <para>
    ///         At minimum, the implementation should execute the passed command.
    ///     </para>
    ///     <para>
    ///         Auto generated keys should be assigned to the correct property in the entity once the INSERT has completed. .
    ///     </para>
    ///     <para>
    ///         Sequences should be executed before and their value should be assigned to the command (which also means that
    ///         the sequence column must be added to the generated SQL string).
    ///     </para>
    /// </remarks>
    Task Insert(ClassMapping mapping, object entity, IDbCommand command);

    /// <summary>
    ///     Modify update statement.
    /// </summary>
    /// <param name="mapping">Mapping to use.</param>
    /// <param name="entity">Entity being updated.</param>
    /// <param name="command">Command that contains an UPDATE statement.</param>
    /// <returns></returns>
    Task Update(ClassMapping mapping, object entity, DbCommand command);
}

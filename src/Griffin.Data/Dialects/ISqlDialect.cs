using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Griffin.Data.Mapper;
using Griffin.Data.Mappings;

namespace Griffin.Data.Dialects;

/// <summary>
///     Abstraction for different database engine.
/// </summary>
public interface ISqlDialect
{
    void ApplyQueryOptions(ClassMapping mapping, DbCommand command, QueryOptions options);

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

    Task Update(ClassMapping mapping, object entity, DbCommand command);
}

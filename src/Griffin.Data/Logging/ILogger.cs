using System.Data;

namespace Griffin.Data.Logging;

/// <summary>
///     Implement this interface to receive all queries.
/// </summary>
public interface ILogger
{
    /// <summary>
    ///     A CRUD command is about to be executed.
    /// </summary>
    /// <param name="cmd"></param>
    void CrudCommand(IDbCommand cmd);

    /// <summary>
    ///     A SQL query is about to be executed.
    /// </summary>
    /// <param name="cmd"></param>
    void QueryCommand(IDbCommand cmd);
}

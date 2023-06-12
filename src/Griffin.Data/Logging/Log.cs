using System.Data;

namespace Griffin.Data.Logging;

/// <summary>
///     Log facade.
/// </summary>
/// <remarks>
///     <para>
///         Invoked each time a SQL statement is going to be executed. Assign <see cref="Logger" /> to receive all
///         statements.
///     </para>
/// </remarks>
public class Log
{
    /// <summary>
    ///     Assign this field to get logging.
    /// </summary>
    public static ILogger? Logger = null;

    /// <summary>
    ///     A CRUD statement is being executed.
    /// </summary>
    /// <param name="command">Command</param>
    public static void Crud(IDbCommand command)
    {
        Logger?.CrudCommand(command);
    }

    /// <summary>
    ///     A SQL query is being executed.
    /// </summary>
    /// <param name="command">Command</param>
    public static void Query(IDbCommand command)
    {
        Logger?.QueryCommand(command);
    }
}

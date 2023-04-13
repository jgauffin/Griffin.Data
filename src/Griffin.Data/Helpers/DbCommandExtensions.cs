using System;
using System.Data;
using System.Data.Common;

namespace Griffin.Data.Helpers;

/// <summary>
///     Extension methods for ADO.NET commands.
/// </summary>
public static class DbCommandExtensions
{
    /// <summary>
    ///     Add a parameter to a command.
    /// </summary>
    /// <param name="command">Command to add a parameter to.</param>
    /// <param name="name">Name of the parameter (without argument prefix).</param>
    /// <param name="value">Value. This method will replace <c>null</c> with <c>DbNull</c>.</param>
    /// <returns></returns>
    public static IDbDataParameter AddParameter(this IDbCommand command, string name, object? value)
    {
        if (command == null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        if (name == null)
        {
            throw new ArgumentNullException(nameof(name));
        }

        var p = command.CreateParameter();
        p.ParameterName = name;
        p.Value = value ?? DBNull.Value;
        command.Parameters.Add(p);
        return p;
    }

    /// <summary>
    ///     Create a command from a transaction.
    /// </summary>
    /// <param name="transaction"></param>
    /// <returns></returns>
    /// <remarks>
    ///     <para>
    ///         This method is required since the ADO.NET interfaces do not contain any async methods while the abstract base
    ///         classes do.
    ///     </para>
    /// </remarks>
    public static DbCommand CreateCommand(this IDbTransaction transaction)
    {
        if (transaction == null)
        {
            throw new ArgumentNullException(nameof(transaction));
        }

        var cmd = transaction.Connection.CreateCommand();
        cmd.Transaction = transaction;
        return (DbCommand)cmd;
    }
}

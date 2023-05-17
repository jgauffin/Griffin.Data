using System;
using System.Data;
using System.Data.Common;
using System.IO;
using System.Linq;

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
    ///     Create a new database command (and enlist it in the transaction).
    /// </summary>
    /// <param name="transaction">Transaction to create command on.</param>
    /// <returns>Created command.</returns>
    public static DbCommand CreateCommand(this IDbTransaction transaction)
    {
        if (transaction.Connection == null)
        {
            throw new InvalidOperationException(
                "The transaction has been committed. You may not use the session any more.");
        }

        var cmd = transaction.Connection!.CreateCommand();
        cmd.Transaction = transaction;
        return (DbCommand)cmd;
    }

    /// <summary>
    ///     Create a detailed exception with information from the command.
    /// </summary>
    /// <param name="command">Command that failed.</param>
    /// <param name="ex">Inner exception</param>
    /// <returns>More detailed exception.</returns>
    public static InvalidDataException CreateDetailedException2(this IDbCommand command, Exception ex)
    {
        if (ex is GriffinException ex2)
        {

        }
        var ps = command.Parameters.Cast<IDataParameter>().Select(x => $"{x.ParameterName}={x.Value}");
        var e = new InvalidDataException(
            $"{ex.Message}\r\n  SQL: '{command.CommandText}'\r\n  Parameters: {string.Join(", ", ps)}", ex);
        return e;
    }

    /// <summary>
    ///     Create a detailed exception with information from the command.
    /// </summary>
    /// <param name="command">Command that failed.</param>
    /// <param name="ex">Inner exception</param>
    /// <param name="entityType">Type of entity that the command was for.</param>
    /// <returns>More detailed exception.</returns>
    public static InvalidDataException CreateDetailedException2(this IDbCommand command, Exception ex, Type entityType)
    {
        var ps = command.Parameters.Cast<IDataParameter>().Select(x => $"{x.ParameterName}={x.Value}");
        var e = new InvalidDataException(
            $"{ex.Message}\r\n  EntityType: {entityType.FullName}\r\n  SQL: '{command.CommandText}'\r\n  Parameters: {string.Join(", ", ps)}",
            ex);
        return e;
    }

    /// <summary>
    ///     Create a detailed exception with information from the command (during an attempt to fetch a child.)
    /// </summary>
    /// <param name="command">Command that failed.</param>
    /// <param name="ex">Inner exception</param>
    /// <param name="parentType">Parent entity type.</param>
    /// <param name="entityType">Child entity type</param>
    /// <returns>More detailed exception.</returns>
    public static InvalidDataException CreateDetailedException2(
        this IDbCommand command,
        Exception ex,
        Type parentType,
        Type entityType)
    {
        var ps = command.Parameters.Cast<IDataParameter>().Select(x => $"{x.ParameterName}={x.Value}");
        var e = new InvalidDataException(
            $"{ex.Message}\r\n  ParentType: {parentType.FullName}\r\n  EntityType: {entityType.FullName}\r\n  SQL: '{command.CommandText}'\r\n  Parameters: {string.Join(", ", ps)}",
            ex);
        return e;
    }
}

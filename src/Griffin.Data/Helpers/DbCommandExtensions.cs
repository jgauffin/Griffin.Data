using System.Data;
using System.Data.Common;

namespace Griffin.Data.Helpers;

public static class DbCommandExtensions
{
    public static DbCommand CreateCommand(this IDbTransaction transaction)
    {
        var cmd = transaction.Connection.CreateCommand();
        cmd.Transaction = transaction;
        return (DbCommand)cmd;
    }

    public static IDbDataParameter AddParameter(this IDbCommand command, string name, object value)
    {
        var p = command.CreateParameter();
        p.ParameterName = name;
        p.Value = value;
        command.Parameters.Add(p);
        return p;
    }
}
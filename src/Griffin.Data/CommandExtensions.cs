using System;
using System.Data;

namespace Griffin.Data
{
    /// <summary>
    /// Extension methods for IDbCommand
    /// </summary>
    public static class CommandExtensions
    {
        /// <summary>
        /// Make it easier to find a command
        /// </summary>
        /// <param name="command">Command</param>
        /// <param name="name">Paramater name</param>
        /// <param name="value">Value. Null will be converted to <c>DBNull.Value</c></param>
        public static void AddParameter(this IDbCommand command, string name, object value)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (name == null) throw new ArgumentNullException("name");

            var p = command.CreateParameter();
            p.ParameterName = name;
            p.Value = value ?? DBNull.Value;
            command.Parameters.Add(p);
        }
    }
}
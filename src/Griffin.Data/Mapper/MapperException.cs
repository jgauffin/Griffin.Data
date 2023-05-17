using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;

namespace Griffin.Data.Mapper;

/// <summary>
///     Base exception for all errors thrown by the mapper.
/// </summary>
/// <remarks>
///     <para>
///         Also used to make sure that we don't rethrow our own exceptions.
///     </para>
/// </remarks>
public class MapperException : GriffinException
{
    private string _parametersStr;

    public MapperException(string message, IDbCommand command, object entity)
        : base(message)
    {
        AssignCommandInformation(command);
        Entity = entity;
        EntityType = entity.GetType();
    }

    public MapperException(string message, IDbCommand command, Exception inner)
        : base(message, inner)
    {
        AssignCommandInformation(command);
    }

    public MapperException(string message, IDbCommand command, object entity, Exception inner)
        : base(message, inner)
    {
        AssignCommandInformation(command);
        Entity = entity;
        EntityType = entity.GetType();
    }

    public MapperException(string message, IDbCommand command, Type entityType, Exception inner)
        : base(message, inner)
    {
        AssignCommandInformation(command);
        EntityType = entityType;
    }


    /// <summary>
    ///     Entity that failed.
    /// </summary>
    public object? Entity { get; set; }

    public Type? EntityType { get; set; }

    public override string Message
    {
        get
        {
            var sb = new StringBuilder();

            sb.AppendLine(base.Message);
            if (EntityType != null)
            {
                sb.AppendLine("EntityType: " + EntityType.FullName);
            }

            if (Entity != null)
            {
                sb.AppendLine("Entity: " + Entity);
            }

            if (SqlStatement != null)
            {
                sb.AppendLine("Sql: " + SqlStatement);
            }

            if (Parameters.Any())
            {
                sb.AppendLine("Parameters: " + _parametersStr);
            }

            return sb.ToString();
        }
    }

    /// <summary>
    ///     SQL Command parameters
    /// </summary>
    public IDictionary<string, object?> Parameters { get; set; }

    public string SqlStatement { get; set; }

    private void AssignCommandInformation(IDbCommand command)
    {
        var parameters = new Dictionary<string, object?>();
        foreach (DbParameter parameter in command.Parameters)
        {
            parameters[parameter.ParameterName] = parameter.Value;
        }

        Parameters = parameters;
        SqlStatement = command.CommandText;

        var ps = command.Parameters.Cast<IDataParameter>().Select(x => $"{x.ParameterName}={x.Value}");
        _parametersStr = string.Join(", ", ps);
    }
}

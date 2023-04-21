using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Griffin.Data.Helpers;
using Griffin.Data.Mapper.Implementation;

namespace Griffin.Data.Mapper;

/// <summary>
///     An entity was not found when one was expected.
/// </summary>
/// <remarks>
///     <para>
///         Thrown when queries are made against IDs and those entities are always expected to be found (unless there is a
///         bug somewhere in your code).
///     </para>
/// </remarks>
public class EntityNotFoundException : Exception
{
    private readonly string _constraintsStr;

    /// <summary>
    /// </summary>
    /// <param name="entityType">Type of entity that was not found.</param>
    /// <param name="constraints">Constraints used when querying.</param>
    /// <exception cref="ArgumentNullException">Any of the arguments are null.</exception>
    public EntityNotFoundException(Type entityType, object constraints)
    {
        if (constraints == null)
        {
            throw new ArgumentNullException(nameof(constraints));
        }

        var options = constraints switch
        {
            QueryOptions ops => ops,
            IHaveQueryOptions ops2 => ops2.Options,
            _ => null
        };

        if (options != null)
        {
            Constraints = options.DbParameters ?? options.Parameters ?? new Dictionary<string, object>();
            _constraintsStr = options.ToString();
        }
        else
        {
            Constraints = constraints.ToDictionary();
            _constraintsStr = string.Join(", ", Constraints.Select(x => $"{x.Key}: {x.Value}"));
        }

        EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
    }

    /// <summary>
    /// </summary>
    /// <param name="entityType">Type of entity that was not found.</param>
    /// <param name="command">Command used when trying to find the entity.</param>
    public EntityNotFoundException(Type entityType, IDbCommand command)
    {
        if (command == null)
        {
            throw new ArgumentNullException(nameof(command));
        }

        var ps = new Dictionary<string, object>();
        if (command.Parameters != null)
        {
            foreach (IDataParameter parameter in command.Parameters)
            {
                if (parameter.Value == null)
                {
                    continue;
                }

                ps[parameter.ParameterName] = parameter.Value;
            }
        }

        EntityType = entityType;
        Constraints = ps;
        _constraintsStr = string.Join(", ", ps.Select(x => $"{x.Key}: {x.Value}"));
    }

    /// <summary>
    ///     All specified constraints.
    /// </summary>
    public IDictionary<string, object> Constraints { get; }

    /// <summary>
    ///     Type of entity that was not found.
    /// </summary>
    public Type EntityType { get; }

    /// <inheritdoc />
    public override string Message =>
        $"{EntityType.Name}: Failed to find an entity using constraints: " + _constraintsStr;
}

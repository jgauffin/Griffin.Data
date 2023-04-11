using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Griffin.Data.Helpers;

namespace Griffin.Data.Mapper;

internal class EntityNotFoundException : Exception
{
    private readonly string _constraintsStr;

    public EntityNotFoundException(Type entityType, object constraints)
    {
        if (constraints == null)
        {
            throw new ArgumentNullException(nameof(constraints));
        }

        EntityType = entityType ?? throw new ArgumentNullException(nameof(entityType));
        Constraints = constraints.ToDictionary();
        _constraintsStr = string.Join(", ", Constraints.Select(x => $"{x.Key}: {x.Value}"));
    }

    public EntityNotFoundException(Type entityType, IDbCommand command)
    {
        var ps = new Dictionary<string, object>();
        foreach (IDataParameter parameter in command.Parameters)
        {
            ps[parameter.ParameterName] = parameter.Value;
        }

        EntityType = entityType;
        Constraints = ps;
        _constraintsStr = string.Join(", ", ps.Select(x => $"{x.Key}: {x.Value}"));
    }

    public IDictionary<string, object> Constraints { get; }

    public Type EntityType { get; }

    public override string Message =>
        $"{EntityType.Name}: Failed to find an entity using constraints '{_constraintsStr}'";
}

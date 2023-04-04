using System;
using System.Collections.Generic;
using System.Data;
using Griffin.Data.Mappings.Properties;

namespace Griffin.Data.Mappings.Relations;

/// <summary>
/// Mapping for a one to zero/one relationship.
/// </summary>
public interface IHasOneMapping : IRelationShip, IFieldAccessor
{
    /// <summary>
    /// Has a discriminator column (i.e. sub classes).
    /// </summary>
    bool HaveDiscriminator { get; }

    /// <summary>
    /// Used to limit returned rows.
    /// </summary>
    public KeyValuePair<string, string>? SubsetColumn { get; set; }

    /// <summary>
    /// Select type based on the parent property or the child record.
    /// </summary>
    /// <param name="parentEntity">Parent entity, to allow the mapping tof etch the denominator property value.</param>
    /// <returns></returns>
    Type? GetTypeUsingDiscriminator(object parentEntity);
}
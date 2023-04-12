using System;

namespace Griffin.Data.Scaffolding.Queries.Meta;

/// <summary>
/// Parameter 
/// </summary>
public class QueryMetaParameter
{
    public QueryMetaParameter(string name, Type propertyType)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        PropertyType = propertyType ?? throw new ArgumentNullException(nameof(propertyType));
    }

    /// <summary>
    /// Property type.
    /// </summary>
    public Type PropertyType { get; private set; }

    /// <summary>
    /// Name of parameter (pascal case).
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    /// Default value, used when generating test script (strings are encapsulated with '').
    /// </summary>
    public object? DefaultValue { get; set; }
}

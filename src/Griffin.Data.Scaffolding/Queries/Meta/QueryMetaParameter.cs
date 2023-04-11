using System;

namespace Griffin.Data.Scaffolding.Queries.Meta;

public class QueryMetaParameter
{
    public Type PropertyType { get; set; }
    public string Name { get; set; }
    public object DefaultValue { get; set; }
}
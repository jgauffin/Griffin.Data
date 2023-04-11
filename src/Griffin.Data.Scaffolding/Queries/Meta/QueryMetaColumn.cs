using System;

namespace Griffin.Data.Scaffolding.Queries.Meta;

public class QueryMetaColumn
{
    public Type PropertyType { get; set; }
    public string Name { get; set; }
    public int StringLength { get; set; }

    /// <summary>
    /// Data type (without size specification).
    /// </summary>
    public string SqlDataType { get; set; }
    
}

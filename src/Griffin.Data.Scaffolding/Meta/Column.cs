using System;

namespace Griffin.Data.Scaffolding.Meta;

public class Column
{
    public string ColumnName { get; set; }
    public string? DefaultValue { get; set; }
    public bool IsNullable { get; set; }
    public bool IsPrimaryKey { get; set; }
    public int? MaxStringLength { get; set; }
    public string PropertyName { get; set; }
    public Type PropertyType { get; set; }
    public string SqlDataType { get; set; }
}

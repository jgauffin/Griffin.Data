using System;

namespace Griffin.Data.Scaffolding.Meta;

public class Column
{
    public bool IsPrimaryKey { get; set; }
    public string ColumnName { get; set; }
    public string PropertyName { get; set; }
    public string SqlDataType { get; set; }
    public Type PropertyType { get; set; }
    public int? MaxStringLength { get; set; }
    public string? DefaultValue { get; set; }
    public bool IsNullable { get; set; }
}
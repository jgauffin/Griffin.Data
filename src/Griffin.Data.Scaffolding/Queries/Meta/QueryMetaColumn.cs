namespace Griffin.Data.Scaffolding.Queries.Meta;

/// <summary>
///     A column in a query result set.
/// </summary>
public class QueryMetaColumn
{
    public QueryMetaColumn(string name, Type propertyType)
    {
        Name = name;
        PropertyType = propertyType;
    }

    /// <summary>
    ///     Name of the column.
    /// </summary>
    public string Name { get; private set; }

    /// <summary>
    ///     Type of property.
    /// </summary>
    public Type PropertyType { get; private set; }

    /// <summary>
    ///     Data type (without size specification).
    /// </summary>
    public string SqlDataType { get; set; } = "";

    /// <summary>
    ///     Max length (from a varchar specification).
    /// </summary>
    public int? StringLength { get; set; }
}

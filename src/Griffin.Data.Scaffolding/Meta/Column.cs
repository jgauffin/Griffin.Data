namespace Griffin.Data.Scaffolding.Meta;

/// <summary>
///     Information about a column in a table.
/// </summary>
public class Column
{
    public Column(string columnName, string sqlDataType, Type propertyType)
    {
        ColumnName = columnName ?? throw new ArgumentNullException(nameof(columnName));
        SqlDataType = sqlDataType ?? throw new ArgumentNullException(nameof(sqlDataType));
        PropertyType = propertyType;
        PropertyName = columnName;
    }

    /// <summary>
    ///     Name of column.
    /// </summary>
    public string ColumnName { get; private set; }

    /// <summary>
    ///     Default value (as specified by a constraint).
    /// </summary>
    public string? DefaultValue { get; set; }

    /// <summary>
    ///     Column is nullable.
    /// </summary>
    public bool IsNullable { get; set; }

    /// <summary>
    ///     Column is a primary key.
    /// </summary>
    public bool IsPrimaryKey { get; set; }

    /// <summary>
    ///     Max string length (as specified by for instance 'varchar').
    /// </summary>
    public int? MaxStringLength { get; set; }

    /// <summary>
    ///     Property name (normalized column name).
    /// </summary>
    public string PropertyName { get; set; }

    /// <summary>
    ///     Type of property.
    /// </summary>
    public Type PropertyType { get; private set; }

    /// <summary>
    ///     SQL data type.
    /// </summary>
    public string SqlDataType { get; private set; }
}

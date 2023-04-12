namespace Griffin.Data.Meta;

/// <summary>
/// Column information
/// </summary>
public class Column
{
    /// <summary>
    /// Auto incremented key.
    /// </summary>
    public bool IsAutoIncrement;

    /// <summary>
    /// Nullable
    /// </summary>
    public bool IsNullable;

    /// <summary>
    /// Primary key column
    /// </summary>
    public bool IsPrimaryKey;

    /// <summary>
    /// Column name
    /// </summary>
    public string Name = "";

    /// <summary>
    /// Generated property name (based on the column name).
    /// </summary>
    public string PropertyName = "";

    /// <summary>
    /// Dotnet alias from the type.
    /// </summary>
    public string PropertyType = "";
}

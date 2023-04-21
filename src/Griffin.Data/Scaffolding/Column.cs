using System;

namespace Griffin.Data.Scaffolding;

/// <summary>
///     Information about a column in a table.
/// </summary>
public class Column
{
    /// <summary>
    /// </summary>
    /// <param name="columnName">Name of column.</param>
    /// <param name="sqlDataType">SQL data type (varchar etc)</param>
    /// <param name="propertyType">.NET type.</param>
    /// <exception cref="ArgumentNullException">Name or propertyType is null.</exception>
    public Column(string columnName, string? sqlDataType, string propertyType)
    {
        Name = columnName ?? throw new ArgumentNullException(nameof(columnName));
        SqlDataType = sqlDataType ?? throw new ArgumentNullException(nameof(sqlDataType));
        PropertyType = propertyType;
        PropertyName = columnName;
    }

    /// <summary>
    ///     When the standard type is overriden with a custom type (like a generated enum type).
    /// </summary>
    public string? CustomPropertyType { get; set; }

    /// <summary>
    ///     Default value (as specified by a constraint).
    /// </summary>
    public string? DefaultValue { get; set; }

    /// <summary>
    ///     Auto incremented key.
    /// </summary>
    public bool IsAutoIncrement { get; set; }

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
    ///     Name of column.
    /// </summary>
    public string Name { get; }

    /// <summary>
    ///     Property name (normalized column name).
    /// </summary>
    public string PropertyName { get; set; }

    /// <summary>
    ///     Type of property (used when <see cref="CustomPropertyType" /> is <c>null</c>).
    /// </summary>
    /// <remarks>
    ///     <para>
    ///         This must be a string since we can refer custom ADO.NET types which aren't loaded since we do not want to
    ///         reference every possible ADO.NET provider from this library.
    ///     </para>
    /// </remarks>
    public string PropertyType { get; }

    /// <summary>
    ///     SQL data type.
    /// </summary>
    public string? SqlDataType { get; }

    /// <inheritdoc />
    public override string ToString()
    {
        return IsPrimaryKey
            ? $"Primary key {Name}{(IsAutoIncrement ? " AutoIncremented" : "")}"
            : $"{Name} {SqlDataType}";
    }
}

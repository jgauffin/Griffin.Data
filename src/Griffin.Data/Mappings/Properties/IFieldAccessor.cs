namespace Griffin.Data.Mappings.Properties;

/// <summary>
///     Access value in a field (property or key).
/// </summary>
public interface IFieldAccessor
{
    /// <summary>
    ///     Get a column value.
    /// </summary>
    /// <param name="entity">Entity to read from.</param>
    /// <returns>Value to use in a SQL statement.</returns>
    /// <remarks>
    ///     <para>
    ///         The method should internally read the property, convert the value and then return the converted value.
    ///     </para>
    /// </remarks>
    object? GetColumnValue(object entity);

    /// <summary>
    ///     Set property value (i.e. write to the property).
    /// </summary>
    /// <param name="instance">Entity to write to.</param>
    /// <param name="value">Value (should be of same type as the property).</param>
    void SetPropertyValue(object instance, object value);
}

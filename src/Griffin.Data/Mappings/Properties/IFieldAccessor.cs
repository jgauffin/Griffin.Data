using System.Diagnostics.CodeAnalysis;

namespace Griffin.Data.Mappings.Properties;

/// <summary>
///     Access value in a field (property or key).
/// </summary>
public interface IFieldAccessor
{
    /// <summary>
    ///     Set column value (i.e. write to the property).
    /// </summary>
    /// <param name="instance">Entity to write to.</param>
    /// <param name="value">Value read from the database.</param>
    /// <remarks>
    ///     <para>
    ///         This method should invoke the converter internally before assigning the property.
    ///     </para>
    /// </remarks>
    void SetColumnValue([NotNull]object instance, object value);

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
    object? GetColumnValue([NotNull] object entity);
}
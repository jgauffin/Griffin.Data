using System.Diagnostics.CodeAnalysis;

namespace Griffin.Data.Converters;

/// <summary>
///     converts a single value between a column type and a property type.
/// </summary>
/// <typeparam name="TColumn">Column type.</typeparam>
/// <typeparam name="TProperty">Property type.</typeparam>
/// <remarks>
///     <para>
///         Converters are used when the types differs and there are no implicit cast between the data types.
///     </para>
/// </remarks>
public interface ISingleValueConverter<TColumn, TProperty> where TColumn: notnull where TProperty: notnull
{
    /// <summary>
    ///     Convert a column value to a property value.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <returns>Value that can be assigned to the entity property (without casts etc).</returns>
    [return: NotNull]
    TProperty ColumnToProperty([DisallowNull] TColumn value);

    /// <summary>
    ///     Convert a property value to a column value.
    /// </summary>
    /// <param name="value">Value to convert.</param>
    /// <returns>Value that can be used in the CRUD operation (without casts etc).</returns>
    [return: NotNull]
    TColumn PropertyToColumn([DisallowNull] TProperty value);
}
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Griffin.Data.Converters;

/// <summary>
///     A converter that requires multiple columns to be able to convert db values to a single property and vice versa.
/// </summary>
/// <typeparam name="TPropertyType">Type of property.</typeparam>
/// <remarks>
///     <para>
///         One use case is that a child entity is stored as JSON and the converter need to save the correct type in a
///         column.
///     </para>
/// </remarks>
public interface IRecordToValueConverter<TPropertyType>
{
    /// <summary>
    ///     Convert record to the correct property type.
    /// </summary>
    /// <param name="dataRecord">Record to convert (a row in a data reader).</param>
    /// <returns>Property value.</returns>
    [return: NotNull]
    TPropertyType Convert([NotNull] IDataRecord dataRecord);

    /// <summary>
    ///     Convert a property value to column values.
    /// </summary>
    /// <param name="entity">Entity to read property from.</param>
    /// <param name="columns">Column names and their respective value.</param>
    void ConvertToColumns([NotNull] TPropertyType entity, [NotNull] IDictionary<string, object> columns);
}

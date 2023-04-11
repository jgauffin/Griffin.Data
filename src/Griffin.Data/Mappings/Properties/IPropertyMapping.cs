using System.Data;

namespace Griffin.Data.Mappings.Properties;

/// <summary>
///     Mapping for a property which is not a key or child entity type.
/// </summary>
public interface IPropertyMapping : IFieldMapping
{
    /// <summary>
    ///     This field is ignored
    /// </summary>
    bool IsIgnored { get; set; }

    /// <summary>
    ///     Get property value without conversions.
    /// </summary>
    /// <param name="entity"></param>
    /// <returns></returns>
    object? GetValue(object entity);

    /**
     * Map a db row to this property.
     */
    void MapRecord(IDataRecord record, object entity);
}

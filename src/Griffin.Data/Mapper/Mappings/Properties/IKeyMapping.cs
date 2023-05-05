using System.Data;

namespace Griffin.Data.Mapper.Mappings.Properties;

/// <summary>
///     Mapping of a key (primary key).
/// </summary>
public interface IKeyMapping : IFieldMapping
{
    /// <summary>
    ///     This column is auto incremented (i.e. should not be specified in insert statements).
    /// </summary>
    bool IsAutoIncrement { get; }

    /// <summary>
    ///     Map a db row to this property.
    /// </summary>
    /// <param name="record">Database row.</param>
    /// <param name="entity">Entity to fill with data from the row.</param>
    void MapRecord(IDataRecord record, object entity);
}

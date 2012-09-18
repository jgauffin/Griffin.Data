using System.Data;

namespace Griffin.Data.Mappings
{
    /// <summary>
    /// Uses a <see cref="IDataRecord"/> (usually through <see cref="IDataReader"/> which inherits <c>IDataRecord</c>) to create your entity.
    /// </summary>
    /// <typeparam name="T">Data entity</typeparam>
    public interface IDataRecordMapper<out T> where T : class
    {
        /// <summary>
        /// Map a record to a new entity.
        /// </summary>
        /// <param name="record">Row from the query result</param>
        /// <returns>Created and populated entity.</returns>
        T Map(IDataRecord record);
    }
}

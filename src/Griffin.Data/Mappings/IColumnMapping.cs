using System.Data;

namespace Griffin.Data.Mappings
{
    /// <summary>
    /// Copuies the value from the data record column to the property in the entity.
    /// </summary>
    /// <remarks>It's up to the implementation to decide wether a conversion should take place or not.
    /// </remarks>
    public interface IColumnMapping
    {
        /// <summary>
        /// Assign the property
        /// </summary>
        /// <param name="record">A data record</param>
        /// <param name="entity">The entity that the property exists in (which the property should be assign a value for)</param>
        void SetValue(IDataRecord record, object entity);
    }
}
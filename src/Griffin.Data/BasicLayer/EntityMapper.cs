using System.Linq;
using Griffin.Data.Mappings;

namespace Griffin.Data.BasicLayer
{
    /// <summary>
    /// Provides table name and IdColumnName.
    /// </summary>
    /// <typeparam name="TEntity">POCO</typeparam>
    public class EntityMapper<TEntity> : SimpleMapper<TEntity>, ITableMapping where TEntity : class
    {
        #region ITableMapping Members

        /// <summary>
        /// Gets name of the table/view.
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// Gets id column name.
        /// </summary>
        public string IdColumnName { get; set; }

        /// <summary>
        /// Get column name from a property name
        /// </summary>
        /// <param name="propertyName">property name</param>
        /// <returns>Column name</returns>
        public string GetColumnName(string propertyName)
        {
            return Mappings.Where(x => x.PropertyName == propertyName).Select(x => x.ColumnName).FirstOrDefault();
        }

        #endregion
    }
}
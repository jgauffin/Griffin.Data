using Griffin.Data.Mappings;

namespace Griffin.Data.BasicLayer
{
    /// <summary>
    /// Provides table name and IdColumnName.
    /// </summary>
    /// <typeparam name="TEntity">POCO</typeparam>
    public class EntityMapper<TEntity> : SimpleMapper<TEntity>, ITableMapping where TEntity : class, new()
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

        #endregion
    }
}
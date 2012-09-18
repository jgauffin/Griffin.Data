using System;
using System.Data;

namespace Griffin.Data.Mappings
{
    /// <summary>
    /// Used a delegate to map the entity
    /// </summary>
    /// <typeparam name="TEntity">Entity type which is being mapped from a data record.</typeparam>
    public class DelegateMapper<TEntity> : IDataRecordMapper<TEntity> where TEntity : class
    {
        private readonly Func<IDataRecord, TEntity> _mapper;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelegateMapper{T}" /> class.
        /// </summary>
        /// <param name="mapper">Function which will convert the record to an entity.</param>
        public DelegateMapper(Func<IDataRecord, TEntity> mapper)
        {
            if (mapper == null) throw new ArgumentNullException("mapper");
            _mapper = mapper;
        }

        #region IDataRecordMapper<TEntity> Members

        /// <summary>
        /// Map the record
        /// </summary>
        /// <param name="record">DB record</param>
        /// <returns>Created entity</returns>
        public TEntity Map(IDataRecord record)
        {
            return _mapper(record);
        }

        #endregion
    }
}
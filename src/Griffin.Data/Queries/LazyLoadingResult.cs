using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Griffin.Data.Mappings;

namespace Griffin.Data.Queries
{
    /// <summary>
    /// Will not execute the result until the enumerator is requested.
    /// </summary>
    /// <typeparam name="TEntity">Model</typeparam>
    public class LazyLoadingResult<TEntity> : IDisposable, IEnumerable<TEntity> where TEntity : class
    {
        private IDbCommand _command;
        private readonly IDataRecordMapper<TEntity> _mapper;

        public LazyLoadingResult(IDbCommand command, IDataRecordMapper<TEntity> mapper)
        {
            if (command == null) throw new ArgumentNullException("command");
            if (mapper == null) throw new ArgumentNullException("mapper");
            _command = command;
            _mapper = mapper;
        }

        /// <summary>
        /// Returns an enumerator that iterates through the collection.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>1</filterpriority>
        public IEnumerator<TEntity> GetEnumerator()
        {
            var reader = _command.ExecuteReader();
            return new LazyLoadingEnumerator<TEntity>(reader, _mapper);
        }

        /// <summary>
        /// Returns an enumerator that iterates through a collection.
        /// </summary>
        /// <returns>
        /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (_command == null)
                return;

            _command.Dispose();
            _command = null;
        }
    }
}
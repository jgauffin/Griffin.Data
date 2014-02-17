using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using Griffin.Data.Mappings;

namespace Griffin.Data.Queries
{
    /// <summary>
    /// Enumerator which only loads one record at a time.
    /// </summary>
    /// <typeparam name="TEntity">POCO</typeparam>
    /// <remarks>This enumerator can only be iterated once. Use <c>ToList()</c> if you want to process it several times.</remarks>
    public class LazyLoadingEnumerator<TEntity> : IEnumerator<TEntity> where TEntity : class
    {
        private IDataReader _reader;
        private readonly IDataRecordMapper<TEntity> _mapper;

        public LazyLoadingEnumerator(IDataReader reader, IDataRecordMapper<TEntity> mapper)
        {
            if (reader == null) throw new ArgumentNullException("reader");
            _reader = reader;
            _mapper = mapper;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (_reader == null)
                return;
            
            _reader.Dispose();
            _reader = null;
        }

        /// <summary>
        /// Advances the enumerator to the next element of the collection.
        /// </summary>
        /// <returns>
        /// true if the enumerator was successfully advanced to the next element; false if the enumerator has passed the end of the collection.
        /// </returns>
        /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception><filterpriority>2</filterpriority>
        public bool MoveNext()
        {
            Current = null;
            var result = _reader.Read();
            if (result)
            {
                Current = _mapper.Map(_reader);
            }

            return result;
        }

        /// <summary>
        /// Sets the enumerator to its initial position, which is before the first element in the collection.
        /// </summary>
        /// <exception cref="T:System.InvalidOperationException">The collection was modified after the enumerator was created. </exception><filterpriority>2</filterpriority>
        public void Reset()
        {
            throw new NotSupportedException("Lazy query can only be iterated once.");
        }

        /// <summary>
        /// Gets the element in the collection at the current position of the enumerator.
        /// </summary>
        /// <returns>
        /// The element in the collection at the current position of the enumerator.
        /// </returns>
        public TEntity Current { get; private set; }

        /// <summary>
        /// Gets the current element in the collection.
        /// </summary>
        /// <returns>
        /// The current element in the collection.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        object IEnumerator.Current
        {
            get { return Current; }
        }
    }
}
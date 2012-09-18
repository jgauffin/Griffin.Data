using System.Collections.Generic;
using System.Data;
using System.Threading;

namespace Griffin.Data.BasicLayer
{
    /// <summary>
    /// Small IoC helper. Register this class as scoped to let all datalayer classes share the same connection
    /// </summary>
    public class AdoNetContext : IAdoNetContext
    {
        private readonly IDbConnection _connection;
        private readonly IConnectionFactory _connectionFactory;
        private readonly ReaderWriterLockSlim _rwLock = new ReaderWriterLockSlim();
        private readonly LinkedList<AdoNetUnitOfWork> _uows = new LinkedList<AdoNetUnitOfWork>();

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetContext" /> class.
        /// </summary>
        /// <param name="connectionFactory">Used to checkout a connection.</param>
        public AdoNetContext(IConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
            _connection = _connectionFactory.Create();
        }

        #region IAdoNetContext Members

        /// <summary>
        /// Create a new unit of work implementation
        /// </summary>
        /// <returns>
        /// A unit of work
        /// </returns>
        public IUnitOfWork CreateUnitOfWork()
        {
            var transaction = _connection.BeginTransaction();
            var uow = new AdoNetUnitOfWork(transaction, RemoveTransaction, RemoveTransaction);

            _rwLock.EnterWriteLock();
            _uows.AddLast(uow);
            _rwLock.ExitWriteLock();

            return uow;
        }


        /// <summary>
        /// Create a command (which will be enlisted in any active transaction)
        /// </summary>
        /// <returns>
        /// Command
        /// </returns>
        public IDbCommand CreateCommand()
        {
            var cmd = _connection.CreateCommand();

            _rwLock.EnterReadLock();
            if (_uows.Count > 0)
                cmd.Transaction = _uows.First.Value.Transaction;
            _rwLock.ExitReadLock();

            return cmd;
        }

        #endregion

        private void RemoveTransaction(AdoNetUnitOfWork obj)
        {
            _rwLock.EnterWriteLock();
            _uows.Remove(obj);
            _rwLock.ExitWriteLock();
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
using System;
using System.Data;

namespace Griffin.Data.BasicLayer
{
    /// <summary>
    /// Unit of work for vanilla ADO.NET.
    /// </summary>
    /// <remarks>Transaction is rolled back when UoW is disposed without have being commited.</remarks>
    public class AdoNetUnitOfWork : IUnitOfWork
    {
        private IDbTransaction _transaction;
        private readonly Action<AdoNetUnitOfWork> _rolledBack;
        private readonly Action<AdoNetUnitOfWork> _committed;

        /// <summary>
        /// Initializes a new instance of the <see cref="AdoNetUnitOfWork" /> class.
        /// </summary>
        /// <param name="transaction">The transaction which is used as UoW.</param>
        /// <param name="rolledBack">Callback invoked when UoW is rolled back.</param>
        /// <param name="committed">Callback invoked when being commited.</param>
        public AdoNetUnitOfWork(IDbTransaction transaction, Action<AdoNetUnitOfWork> rolledBack, Action<AdoNetUnitOfWork> committed)
        {
            Transaction = transaction;
            _transaction = transaction;
            _rolledBack = rolledBack;
            _committed = committed;
        }

        /// <summary>
        /// Gets transaction which is being wrapped by this UoW implementation.
        /// </summary>
        public IDbTransaction Transaction { get; private set; }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        /// <filterpriority>2</filterpriority>
        public void Dispose()
        {
            if (_transaction == null) 
                return;

            _transaction.Rollback();
            _transaction.Dispose();
            _rolledBack(this);
            _transaction = null;
        }

        /// <summary>
        /// Save changes into the data source.
        /// </summary>
        public void SaveChanges()
        {
            if (_transaction == null)
                throw new InvalidOperationException("May not call save changes twice.");

            _transaction.Commit();
            _committed(this);
            _transaction = null;
        }
    }
}
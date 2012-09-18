using System;

namespace Griffin.Data
{
    /// <summary>
    /// Unit of work definition
    /// </summary>
    /// <remarks>Disposed without <c>SaveChanges</c> being called = rollback</remarks>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Save changes into the data source.
        /// </summary>
        void SaveChanges();
    }
}
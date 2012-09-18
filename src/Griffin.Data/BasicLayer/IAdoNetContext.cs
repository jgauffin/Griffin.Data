using System.Data;

namespace Griffin.Data.BasicLayer
{
    /// <summary>
    /// DB context for vanilla ADO.NET implementations
    /// </summary>
    /// <remarks>This implementation makes sure that all commands have been registered in the last created transaction (which is wrapped in a unit of work).</remarks>
    public interface IAdoNetContext
    {
        /// <summary>
        /// Create a new unit of work implementation
        /// </summary>
        /// <returns>A unit of work</returns>
        IUnitOfWork CreateUnitOfWork();

        /// <summary>
        /// Create a command (which will be enlisted in any active transaction)
        /// </summary>
        /// <returns>Command</returns>
        IDbCommand CreateCommand();
    }
}
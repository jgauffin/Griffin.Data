using System.Data;

namespace Griffin.Data.BasicLayer
{
    /// <summary>
    /// Creates and opens DB connections.
    /// </summary>
    public interface IConnectionFactory
    {
        /// <summary>
        /// Create a new connection
        /// </summary>
        /// <returns>Open and valid connection</returns>
        IDbConnection Create();
    }
}
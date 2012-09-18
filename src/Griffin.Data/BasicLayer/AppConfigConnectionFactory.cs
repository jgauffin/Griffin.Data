using System;
using System.Configuration;
using System.Data;
using System.Data.Common;
using Griffin.Data.Mappings;

namespace Griffin.Data.BasicLayer
{
    /// <summary>
    /// Creates connections using connection strings configured in app/web.config.
    /// </summary>
    /// <example>
    /// <code>
    /// //setup
    /// yourContainer.RegisterInstance(new AppConfigConnectionFactory("myDb"));
    /// 
    /// // usage (through dependency injection)
    /// public class YourDbQueries
    /// {
    ///     public YourDbQueries(IConnectionFactory factory)
    ///     {
    ///         _connection = factory.Create();
    ///     }
    /// }
    /// </code>
    /// </example>
    public class AppConfigConnectionFactory : IConnectionFactory
    {
        private readonly DbProviderFactory _provider;
        private readonly string _connectionString;
        private readonly string _name;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppConfigConnectionFactory" /> class.
        /// </summary>
        /// <param name="connectionName">Name of the connection string in <c>connectionsString</c> element in <c>app/web.config</c>.</param>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">Failed to find the connection string, or it's configuration is invalid.</exception>
        public AppConfigConnectionFactory(string connectionName)
        {
            if (connectionName == null) throw new ArgumentNullException("connectionName");

            var conStr = ConfigurationManager.ConnectionStrings[connectionName];
            if (conStr == null)
                throw new ConfigurationErrorsException(string.Format("Failed to find connection string named '{0}' in app/web.config.", connectionName));

            _name = conStr.ProviderName;
            _provider = DbProviderFactories.GetFactory(conStr.ProviderName);
            _connectionString = conStr.ConnectionString;

        }

        /// <summary>
        /// Create a new connection
        /// </summary>
        /// <returns>
        /// Open and valid connection
        /// </returns>
        /// <exception cref="System.Configuration.ConfigurationErrorsException">Can't build a string using the provider specified in <c>app/web.config</c></exception>
        public IDbConnection Create()
        {
            var connection = _provider.CreateConnection();
            if (connection == null)
                throw new ConfigurationErrorsException(string.Format("Failed to create a connection using the connection string named '{0}' in app/web.config.", _name));

            connection.ConnectionString = _connectionString;
            connection.Open();
            return connection;
        }
    }
}
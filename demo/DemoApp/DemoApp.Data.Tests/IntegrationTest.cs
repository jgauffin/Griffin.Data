using System.Data.SqlClient;
using Griffin.Data;
using Griffin.Data.ChangeTracking;
using Microsoft.Extensions.Configuration;
using DemoApp.Data.Accounts.Mappings;
using Griffin.Data.Mapper.Mappings;
using Griffin.Data.SqlServer;

namespace DemoApp.Data.Tests
{
    /// <summary>
    ///     Install the nuget package "Microsoft.Extensions.Configuration.Json". Add a "appsettings.json", mark it as "copy
    ///     always" and then add a connection string named "TestDb" to it. Or change the contents below ;)
    /// </summary>
    public class IntegrationTest : IDisposable
    {
        protected IntegrationTest()
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", false)
                .Build();

            var connectionString = config.GetConnectionString("TestDb");
            if (connectionString == null)
                throw new InvalidOperationException("Failed to find a connection string named 'TestDb' in appsettings.json");

            // Change to the correct ADO.NET Provider.
            var connection = new SqlConnection(connectionString);
            var dialect = new SqlServerDialect();
            connection.Open();

            var registry = new MappingRegistry();
            registry.Scan(typeof(AccountMapping).Assembly);

            var changeTracking = new SnapshotChangeTracking(registry);

            Session = new Session(connection.BeginTransaction(), registry, dialect, changeTracking);
        }

        protected Session Session { get; }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            Session.Dispose();
        }
    }
}

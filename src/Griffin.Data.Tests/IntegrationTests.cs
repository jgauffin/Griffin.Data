using System.Data;
using System.Data.Common;
using System.Data.SqlClient;
using System.Reflection;
using Griffin.Data.ChangeTracking;
using Griffin.Data.Mappings;
using Griffin.Data.SqlServer;

namespace Griffin.Data.Tests;

public abstract class IntegrationTests : IDisposable
{
    private readonly SqlConnection _connection;
    private readonly MappingRegistry _mappingRegistry = new();
    private readonly IDbTransaction _transaction;

    protected IntegrationTests()
    {
        _connection = new SqlConnection("Data Source=.;Initial Catalog=GriffinData;Integrated Security=True");
        _connection.Open();
        _transaction = _connection.BeginTransaction();
        _mappingRegistry.Scan(Assembly.GetExecutingAssembly());
        ChangeTracking = new SnapshotChangeTracking(_mappingRegistry);
        Session = new Session(_transaction, _mappingRegistry, new SqlServerDialect(), ChangeTracking);
    }

    public SnapshotChangeTracking ChangeTracking { get; }

    public IMappingRegistry Registry => _mappingRegistry;

    public Session Session { get; }

    public DbTransaction Transaction => (DbTransaction)_transaction;

    public void Dispose()
    {
        GC.SuppressFinalize(this);
        _transaction.Dispose();
        _connection.Dispose();
    }

    public ClassMapping GetMapping<T>()
    {
        return _mappingRegistry.Get<T>();
    }
}

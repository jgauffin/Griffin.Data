using System.Collections.Generic;
using System.Data;
using Griffin.Data.ChangeTracking;
using Griffin.Data.Configuration;
using Griffin.Data.Dialects;
using Griffin.Data.Mapper.Mappings;

namespace Griffin.Data.Queries;

/// <summary>
///     A separate session implementation to be able to use another isolation level for the underlying database
///     transaction.
/// </summary>
public class QuerySession : Session
{
    /// <inheritdoc />
    public QuerySession(DbConfiguration configuration, IEnumerable<IChangeTracker> changeTrackers) : base(configuration, changeTrackers)
    {
    }

    /// <inheritdoc />
    public QuerySession(DbConfiguration configuration) : base(configuration)
    {
    }

    /// <inheritdoc />
    public QuerySession(IDbTransaction transaction, IMappingRegistry registry, ISqlDialect dialect, IChangeTracker? changeTracker) : base(transaction, registry, dialect, changeTracker)
    {
    }
}

using System;
using System.Data;
using System.Data.Common;
using Griffin.Data.Dialects;
using Griffin.Data.Mapper;
using Griffin.Data.Mappings;

namespace Griffin.Data;

/// <summary>
///     Unit of work for the DB.
/// </summary>
public class Session : IDisposable
{
    private readonly IEntityTracker? _entityTracker;
    private readonly IMappingRegistry _registry;
    private bool _done;

    /// <summary>
    /// </summary>
    /// <param name="configuration">Configuration to use.</param>
    public Session(DbConfiguration configuration)
    {
        Transaction = configuration.BeginTransaction();
        _registry = configuration.MappingRegistry;
        Dialect = configuration.Dialect;
    }

    /// <summary>
    /// </summary>
    /// <param name="transaction">Transaction to use.</param>
    /// <param name="registry">Registry to lookup mappings in.</param>
    internal Session(IDbTransaction transaction, IMappingRegistry registry)
    {
        Transaction = transaction;
        _registry = registry;
    }

    /// <summary>
    ///     Current DB transaction.
    /// </summary>
    public IDbTransaction Transaction { get; }

    /// <summary>
    ///     Dialect used to apply DB engine specific SQL statements.
    /// </summary>
    public ISqlDialect Dialect { get; set; } = new SqlServerDialect();

    /// <inheritdoc />
    public void Dispose()
    {
        var con = Transaction.Connection;
        if (!_done) Transaction.Rollback();

        Transaction.Dispose();
        con.Dispose();
    }

    internal DbCommand CreateCommand()
    {
        return Transaction.CreateCommand();
    }

    internal ClassMapping GetMapping<T>()
    {
        return _registry.Get<T>();
    }

    internal ClassMapping GetMapping(Type entityType)
    {
        if (entityType.IsAbstract || entityType.IsInterface)
        {
            throw new MappingException(entityType, "You should get hte mapping for the concrete type instead.");
        }
        return _registry.Get(entityType);
    }

    /// <summary>
    ///     Save all changes.
    /// </summary>
    /// <remarks>
    ///     <para>When change tracking is not used, it simply means that the transaction is committed.</para>
    ///     <para>
    ///         When using change tracking, all entities (and their child entities) will be compared to the snapshot and
    ///         persisted accordingly.
    ///     </para>
    /// </remarks>
    public void SaveChanges()
    {
        Transaction.Commit();
        _done = true;
    }

    /// <summary>
    ///     Track changes.
    /// </summary>
    /// <param name="item">Item to track.</param>
    /// <remarks>
    ///     <para>Should only be used for loaded entities and not when invoking CRUD methods.</para>
    /// </remarks>
    public void Track(object item)
    {
        _entityTracker?.Track(item);
    }
}
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Griffin.Data.ChangeTracking;
using Griffin.Data.Configuration;
using Griffin.Data.Dialects;
using Griffin.Data.Helpers;
using Griffin.Data.Mapper;
using Griffin.Data.Mappings;

namespace Griffin.Data;

/// <summary>
///     Unit of work for the DB.
/// </summary>
public class Session : IDisposable
{
    private readonly IChangeTracker? _changeTracker;
    private readonly IMappingRegistry _registry;
    private bool _commitOnDispose;
    private bool _done;

    /// <summary>
    /// </summary>
    /// <param name="configuration">Configuration to use.</param>
    /// <param name="changeTrackers">A list of zero or one change trackers.</param>
    /// <remarks>
    ///     <para>
    ///         Some IoC containers requires a IEnumerable instead of nullable for optional parameters.
    ///     </para>
    /// </remarks>
    public Session(DbConfiguration configuration, IEnumerable<IChangeTracker> changeTrackers)
    {
        Transaction = configuration.BeginTransaction();
        _registry = configuration.MappingRegistry;
        Dialect = configuration.Dialect;
        _changeTracker = changeTrackers.FirstOrDefault();
    }

    /// <summary>
    /// </summary>
    /// <param name="configuration">Configuration to use.</param>
    /// <param name="changeTracker">Optional change tracker.</param>
    public Session(DbConfiguration configuration, IChangeTracker? changeTracker = null)
    {
        Transaction = configuration.BeginTransaction();
        _registry = configuration.MappingRegistry;
        Dialect = configuration.Dialect;
        _changeTracker = changeTracker;
    }

    /// <summary>
    /// </summary>
    /// <param name="transaction">Transaction to use.</param>
    /// <param name="registry">Registry to lookup mappings in.</param>
    /// <param name="changeTracker"></param>
    public Session(
        IDbTransaction transaction,
        IMappingRegistry registry,
        IChangeTracker? changeTracker)
    {
        Transaction = transaction;
        _registry = registry;
        _changeTracker = changeTracker;
    }

    /// <summary>
    ///     Dialect used to apply DB engine specific SQL statements.
    /// </summary>
    public ISqlDialect Dialect { get; set; } = new SqlServerDialect();

    /// <summary>
    ///     Current DB transaction.
    /// </summary>
    public IDbTransaction Transaction { get; }

    /// <inheritdoc />
    public void Dispose()
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (Transaction == null)
        {
            return;
        }

        var con = Transaction.Connection;
        if (_commitOnDispose)
        {
            Transaction.Commit();
        }
        else if (!_done)
        {
            Transaction.Rollback();
        }

        Transaction.Dispose();
        con?.Dispose();
    }

    /// <summary>
    ///     Apply changes using a change tracker (if one is specified).
    /// </summary>
    /// <returns></returns>
    public async Task ApplyChangeTracking()
    {
        if (_changeTracker != null)
        {
            await _changeTracker.ApplyChanges(this);
        }
    }

    /// <summary>
    ///     Create a new command.
    /// </summary>
    /// <returns>Created command.</returns>
    public DbCommand CreateCommand()
    {
        return CommandExtensions.CreateCommand(Transaction);
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
    public async Task SaveChanges(bool commit = true)
    {
        if (_changeTracker != null)
        {
            await _changeTracker.ApplyChanges(this);
        }

        if (commit)
        {
            Transaction.Commit();
        }
        else
        {
            _commitOnDispose = true;
        }

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
        _changeTracker?.Track(item);
    }

    internal ClassMapping GetMapping<T>()
    {
        return _registry.Get<T>();
    }

    internal ClassMapping GetMapping(Type entityType)
    {
        if (entityType.IsAbstract || entityType.IsInterface)
        {
            throw new MappingException(entityType, "You should get the mapping for the concrete type instead.");
        }

        return _registry.Get(entityType);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Griffin.Data.ChangeTracking.Services.Implementations.v2;
using Griffin.Data.Mapper;
using Griffin.Data.Mappings;

namespace Griffin.Data.ChangeTracking.Services.Implementations;

/// <summary>
///     Used to applied changes between a snapshot entity and an un-tracked entity (which you have built yourself).
/// </summary>
/// <remarks>
///     <para>
///         Intended purpose is to be able to save entities which you have received from client side and want to persist
///         the changes to the database.
///     </para>
/// </remarks>
public class SingleEntityChangeService
{
    private readonly IMappingRegistry _registry;

    /// <summary>
    /// </summary>
    /// <param name="registry">Registry with all mappings.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public SingleEntityChangeService(IMappingRegistry registry)
    {
        _registry = registry ?? throw new ArgumentNullException(nameof(registry));
    }

    public async Task<CompareResultItem> PersistChanges(Session session, object snapshot, object current)
    {
        var comparer = new SingleEntityComparer(_registry);
        var result = comparer.Compare(snapshot, current);

        var persister = new ChangePersister(_registry);
        await persister.Persist(session, result);

        return result.First(x => x.Depth == 1);
    }
}

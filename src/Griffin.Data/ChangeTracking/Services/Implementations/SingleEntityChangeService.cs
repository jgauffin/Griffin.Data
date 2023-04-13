using System;
using System.Linq;
using System.Threading.Tasks;
using Griffin.Data.ChangeTracking.Services.Implementations.v2;
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
///     <para>
///         This service uses  <see cref="SingleEntityComparer" /> and <see cref="ChangePersister" /> internally. You can
///         use them directly for more control.
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

    /// <summary>
    ///     Detect and persist changes.
    /// </summary>
    /// <param name="session">Session to operate in.</param>
    /// <param name="snapshot">Version of the entity that has not been changed.</param>
    /// <param name="current">Version that has changes in it.</param>
    /// <returns></returns>
    public async Task<CompareResultItem> PersistChanges(Session session, object snapshot, object current)
    {
        if (session == null)
        {
            throw new ArgumentNullException(nameof(session));
        }

        if (snapshot == null)
        {
            throw new ArgumentNullException(nameof(snapshot));
        }

        if (current == null)
        {
            throw new ArgumentNullException(nameof(current));
        }

        var comparer = new SingleEntityComparer(_registry);
        var result = comparer.Compare(snapshot, current);

        var persister = new ChangePersister(_registry);
        await persister.Persist(session, result);

        return result.First(x => x.Depth == 1);
    }
}

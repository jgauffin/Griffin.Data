namespace Griffin.Data.ChangeTracking.Services.Implementations.v2;

/// <summary>
///     Context for <see cref="SingleEntityComparer.Filter" />.
/// </summary>
public class FilterContext
{
    /// <summary>
    /// </summary>
    /// <param name="entity">Copy that might have been modified.</param>
    /// <param name="snapshot">Clean copy loaded from the DB.</param>
    /// <param name="propertyName">Property that either should be approved or filtered out.</param>
    public FilterContext(object? entity, object? snapshot, string propertyName)
    {
        Entity = entity;
        Snapshot = snapshot;
        PropertyName = propertyName;
    }

    /// <summary>
    ///     Entity that might have been modified.
    /// </summary>
    public object? Entity { get; }

    /// <summary>
    ///     Property that will be checked.
    /// </summary>
    public string PropertyName { get; }

    /// <summary>
    ///     Stored unmodified version of the entity.
    /// </summary>
    public object? Snapshot { get; }

    internal bool CanCompare { get; private set; } = true;

    /// <summary>
    ///     Property can be used in comparison.
    /// </summary>
    public void CompareProperty()
    {
        CanCompare = true;
    }

    /// <summary>
    ///     Property should not be used in comparison.
    /// </summary>
    public void IgnoreProperty()
    {
        CanCompare = false;
    }
}

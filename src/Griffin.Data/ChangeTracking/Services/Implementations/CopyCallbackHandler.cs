namespace Griffin.Data.ChangeTracking.Services.Implementations;

/// <summary>
///     Callback invoked each time an entity have been copied.
/// </summary>
/// <param name="parent">Parent source entity.</param>
/// <param name="current">Source entity.</param>
/// <param name="snapshot">Created copy.</param>
/// <param name="depth">Depth in entity hierarchy.</param>
public delegate void CopyCallbackHandler(object? parent, object current, object snapshot, int depth);

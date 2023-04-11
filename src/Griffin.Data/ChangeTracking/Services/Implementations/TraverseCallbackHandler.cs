namespace Griffin.Data.ChangeTracking.Services.Implementations;

/// <summary>
///     Callback invoked each time an entity have been copied.
/// </summary>
/// <param name="parent">Parent source entity.</param>
/// <param name="entity">Source entity.</param>
/// <param name="depth">Depth in entity hierarchy.</param>
internal delegate void TraverseCallbackHandler(object? parent, object entity, int depth);

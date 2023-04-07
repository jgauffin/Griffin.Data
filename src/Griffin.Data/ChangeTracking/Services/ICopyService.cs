namespace Griffin.Data.ChangeTracking.Services;

/// <summary>
///     Used to copy entities fetched from the database.
/// </summary>
/// <remarks>
///     <para>
///         All entities have a primary key which can be used to match the copy to the source.
///     </para>
/// </remarks>
public interface ICopyService
{
    /// <summary>
    ///     Copy an object (deep copy).
    /// </summary>
    /// <param name="source">Item to copy.</param>
    /// <returns>A deep copy.</returns>
    object Copy(object source);
}

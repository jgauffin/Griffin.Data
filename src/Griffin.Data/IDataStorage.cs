namespace Griffin.Data
{
    /// <summary>
    /// Used to load/store items in the storage.
    /// </summary>
    /// <typeparam name="TModel">Entity type</typeparam>
    /// <typeparam name="TKey">Primary key </typeparam>
    /// <remarks>Should not be confused with queries which are used to search for information.</remarks>
    public interface IDataStorage<TModel, in TKey> where TModel : class
    {
        /// <summary>
        /// Load an item from the storage.
        /// </summary>
        /// <param name="id">Identity, may be any type as long as it can be converted from a string.</param>
        /// <returns>Item if found; otherwise <c>null</c>.</returns>
        TModel Load(TKey id);

        /// <summary>
        /// Store item.
        /// </summary>
        /// <param name="item">Might be a new object or a previously created one.</param>
        void Store(TModel item);

        /// <summary>
        /// Delete item
        /// </summary>
        /// <param name="id">Primary key</param>
        void Delete(TKey id);
    }
}
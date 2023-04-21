using System.Data;

namespace Griffin.Data.Mapper.Implementation;

/// <summary>
///     Factory delegate for <see cref="QueryOptions" />.
/// </summary>
/// <param name="record">Data record used to determine which type of entity to load.</param>
/// <returns>Created entity.</returns>
public delegate object CreateEntityDelegate(IDataRecord record);

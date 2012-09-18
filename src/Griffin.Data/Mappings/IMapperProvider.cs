using System.Reflection;

namespace Griffin.Data.Mappings
{
    /// <summary>
    /// Used when you want to use a custom source for the mappings.
    /// </summary>
    public interface IMapperProvider
    {
        /// <summary>
        /// Get a mapper for the specified entity.
        /// </summary>
        /// <typeparam name="T">Type of entity (class in .NET)</typeparam>
        /// <returns>Mapper</returns>
        /// <exception cref="System.ArgumentOutOfRangeException">A mapper was not found for the specified type.</exception>
        IDataRecordMapper<T> GetMapper<T>() where T : class;

        /// <summary>
        /// Register a specific mapper.
        /// </summary>
        /// <typeparam name="TEntityType">Type which should be mapped from a query.</typeparam>
        /// <param name="mapper">Mapper</param>
        void Register<TEntityType>(IDataRecordMapper<TEntityType> mapper) where TEntityType : class;

        /// <summary>
        /// Scans the assembly after all classes that implements the <c><![CDATA[IDataRecordMapper<TEntity>]]></c> interface.
        /// </summary>
        /// <param name="assembly">The assembly to scan after implementations.</param>
        void RegisterAssembly(Assembly assembly);
    }
}
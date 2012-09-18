using System;
using System.Collections.Generic;
using System.Reflection;

namespace Griffin.Data.Mappings
{
    /// <summary>
    /// Default implementation of the mapper provider.
    /// </summary>
    /// <remarks>
    /// Used to provide the mappings for the mapping functions.
    /// </remarks>
    /// <example>
    /// Start by registering a mapping. The typical approach is:
    /// <code>
    /// <![CDATA[
    /// // repeat per mapping
    /// public class MyCustomMapping : SimpleMapper<User>
    /// {
    ///     public MyCustomMapping()
    ///     {
    ///         Add(x => x.Id, "id");
    ///         Add(x => x.FirstName, "first_name", new DotNetConverter());
    ///     }
    /// }
    /// ]]>
    /// </code>
    /// 
    /// Then register all mappings:
    /// <code>
    /// MapperProvider.Instance.RegisterAssembly(Assembly.GetExecutingAssembly());
    /// </code>
    /// </example>
    public class MapperProvider : IMapperProvider
    {
        private static IMapperProvider _instance = new MapperProvider();
        private readonly Dictionary<Type, object> _mappers = new Dictionary<Type, object>();

        /// <summary>
        /// Gets current implementation
        /// </summary>
        public static IMapperProvider Instance
        {
            get { return _instance; }
        }

        #region IMapperProvider Members

        /// <summary>
        /// Get a mapper for the specified entity.
        /// </summary>
        /// <typeparam name="T">Type of entity</typeparam>
        /// <returns>
        /// Mapper
        /// </returns>
        /// <exception cref="System.ArgumentOutOfRangeException"></exception>
        public virtual IDataRecordMapper<T> GetMapper<T>() where T : class
        {
            object mapper;
            if (!_mappers.TryGetValue(typeof (T), out mapper))
                throw new ArgumentOutOfRangeException("Failed to find a mapper for " + typeof (T));

            return (IDataRecordMapper<T>) mapper;
        }

        #endregion

        /// <summary>
        /// Assign a new custom implementation
        /// </summary>
        /// <param name="provider">Custom provider implementation. For instance one that uses an IoC container to find the mappings.</param>
        public static void Assign(IMapperProvider provider)
        {
            if (provider == null) throw new ArgumentNullException("provider");
            _instance = provider;
        }

        /// <summary>
        /// Register a specific mapper.
        /// </summary>
        /// <typeparam name="TEntityType">Type which should be mapped from a query.</typeparam>
        /// <param name="mapper">Mapper</param>
        public void Register<TEntityType>(IDataRecordMapper<TEntityType> mapper) where TEntityType : class
        {
            if (mapper == null) throw new ArgumentNullException("mapper");

            var entityType = typeof (TEntityType);

            if (_mappers.ContainsKey(entityType))
                throw new MappingException(
                    string.Format("There is already an mapper ({0}) for entity type {1}. Cant register {2}.",
                                  _mappers[entityType].GetType().FullName, entityType.FullName, mapper.GetType().FullName));

            _mappers.Add(entityType, mapper);
        }

        /// <summary>
        /// Scans the assembly after all classes that implements the <c><![CDATA[IDataRecordMapper<TEntity>]]></c> interface.
        /// </summary>
        /// <param name="assembly">The assembly to scan after implementations.</param>
        public void RegisterAssembly(Assembly assembly)
        {
            if (assembly == null) throw new ArgumentNullException("assembly");

            foreach (var type in assembly.GetTypes())
            {
                if (type.IsInterface || type.IsAbstract)
                    continue;

                foreach (var interfaceType in type.GetInterfaces())
                {
                    if (!interfaceType.IsGenericType)
                        continue;

                    if (interfaceType.GetGenericTypeDefinition() == typeof (IDataRecordMapper<>))
                    {
                        var entityType = interfaceType.GetGenericArguments()[0];
                        if (_mappers.ContainsKey(entityType))
                            throw new MappingException(
                                string.Format("There is already an mapper ({0}) for entity type {1}. Can't register {2}.",
                                              _mappers[entityType].GetType().FullName, entityType.FullName, type.FullName));

                        _mappers.Add(entityType, Activator.CreateInstance(type));
                    }
                }
            }
        }
    }
}
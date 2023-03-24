using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Griffin.Data.Helpers;
using Griffin.Data.Mappings;
using Griffin.Data.Mappings.Properties;
using Griffin.Data.Mappings.Relations;

namespace Griffin.Data.Configuration
{
    /// <summary>
    ///     Allows developer to generate a mapping for a specific entity.
    /// </summary>
    /// <typeparam name="TEntity">Type of entity to create a mapping for.</typeparam>
    public class ClassMappingConfigurator<TEntity> : IMappingBuilder, IClassMappingConfigurator<TEntity>
    {
        private readonly List<HasManyMapping> _hasManyMappings = new();
        private readonly List<HasOneMapping> _hasOneMappings = new();
        private readonly List<KeyMapping> _keys = new();
        private readonly List<PropertyMapping> _properties = new();
        private string _tableName;

        /// <summary>
        /// </summary>
        public ClassMappingConfigurator()
        {
            _tableName = typeof(TEntity).Name;
        }

        /// <summary>
        /// </summary>
        /// <param name="tableName">Name of the corresponding table in the database.</param>
        /// <exception cref="ArgumentNullException">table name is not specified.</exception>
        public void TableName(string tableName)
        {
            _tableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
        }

        /// <summary>
        ///     Define a primary key.
        /// </summary>
        /// <typeparam name="TProperty">Property type for the primary key.</typeparam>
        /// <param name="selector">Expression used to select the correct property.</param>
        /// <returns></returns>
        public KeyConfigurator<TEntity, TProperty> Key<TProperty>(Expression<Func<TEntity, TProperty>> selector)
        {
            var compiled = selector.Compile();
            var getter = (object entity) => (object?)compiled((TEntity)entity);
            var setter = selector.GetPropertyInfo().GenerateSetterDelegate();

            var mapping = new KeyMapping(getter, setter)
            {
                PropertyName = selector.GetMemberName(),
                ColumnName = selector.GetMemberName()
            };
            _keys.Add(mapping);

            return new KeyConfigurator<TEntity, TProperty>(mapping);
        }

        /// <summary>
        ///     Map a property (which is not a key or child entity).
        /// </summary>
        /// <typeparam name="TProperty">Type of property.</typeparam>
        /// <param name="selector">Expression used to select a property.</param>
        /// <returns>Property configurator.</returns>
        public PropertyConfigurator<TEntity, TProperty> Property<TProperty>(
            Expression<Func<TEntity, TProperty>> selector)
        {
            var compiled = selector.Compile();
            var getter = (object entity) => (object?)compiled((TEntity)entity);
            var setter = selector.GetPropertyInfo().GenerateSetterDelegate();
            var prop = selector.GetPropertyInfo();

            var mapping = new PropertyMapping(getter, setter)
            {
                PropertyName = prop.Name,
                PropertyType = prop.PropertyType,
                ColumnName = prop.Name
            };
            _properties.Add(mapping);

            return new PropertyConfigurator<TEntity, TProperty>(mapping);
        }

        /// <summary>
        ///     A property is for a collection of child entities.
        /// </summary>
        /// <typeparam name="TProperty">Type of child property.</typeparam>
        /// <param name="selector">Expression used to select the property.</param>
        /// <returns>Has many configuration.</returns>
        public HasManyConfigurator<TEntity, TProperty> HasMany<TProperty>(
            Expression<Func<TEntity, IReadOnlyList<TProperty>>> selector)
        {
            var getter = selector.Compile();
            var prop = selector.GetPropertyInfo();
            return CreateHasManyMapping<TProperty>(entity => getter((TEntity)entity), prop, typeof(TProperty));
        }

        /// <summary>
        ///     A property is for a collection of child entities.
        /// </summary>
        /// <typeparam name="TProperty">Type of child property.</typeparam>
        /// <param name="selector">Expression used to select the property.</param>
        /// <returns>Has many configuration.</returns>
        public HasManyConfigurator<TEntity, TProperty> HasMany<TProperty>(
            Expression<Func<TEntity, IList<TProperty>>> selector)
        {
            var getter = selector.Compile();
            var prop = selector.GetPropertyInfo();
            return CreateHasManyMapping<TProperty>(entity => getter((TEntity)entity), prop, typeof(TEntity));
        }

        /// <summary>
        ///     A property is for a collection of child entities.
        /// </summary>
        /// <typeparam name="TProperty">Type of child property.</typeparam>
        /// <param name="selector">Expression used to select the property.</param>
        /// <returns>Has many configuration.</returns>
        public HasManyConfigurator<TEntity, TProperty> HasMany<TProperty>(
            Expression<Func<TEntity, ICollection<TProperty>>> selector)
        {
            var getter = selector.Compile();
            var prop = selector.GetPropertyInfo();
            return CreateHasManyMapping<TProperty>(entity => getter((TEntity)entity), prop, typeof(TEntity));
        }

        /// <summary>
        ///     A property is for a single child entity.
        /// </summary>
        /// <typeparam name="TProperty">Type of child property.</typeparam>
        /// <param name="selector">Expression used to select the property.</param>
        /// <returns>Has one configuration.</returns>
        public HasOne<TEntity, TProperty> HasOne<TProperty>(Expression<Func<TEntity, TProperty>> selector)
        {
            var inner = selector.Compile();
            Func<object, object?> getter = x => inner((TEntity)x);
            var setter = selector.GetPropertyInfo().GenerateSetterDelegate();

            var mapping = new HasOneMapping(selector.GetMemberName(), getter, setter);
            _hasOneMappings.Add(mapping);
            return new HasOne<TEntity, TProperty>(mapping);
        }

        /// <summary>
        ///     Create a mapping using this configuration.
        /// </summary>
        /// <returns>Generated mapping.</returns>
        public ClassMapping BuildMapping()
        {
            return new ClassMapping(typeof(TEntity), _tableName, _properties, _keys, _hasManyMappings, _hasOneMappings);
        }

        private HasManyConfigurator<TEntity, TProperty> CreateHasManyMapping<TProperty>(
            Func<object, object> getter,
            PropertyInfo prop,
            Type elementType)
        {
            var setter = prop.GenerateSetterDelegate();

            Func<object, Func<object, Task>, Task> visitorWrapper = async (entity, callback) =>
            {
                var list = (IEnumerable<TProperty>)entity;
                foreach (var item in list)
                {
                    if (item != null)
                    {
                        await callback(item);
                    }
                }
            };

            var hasManyMapping = new HasManyMapping(
                prop.Name,
                prop.PropertyType,
                elementType,
                entity => getter((TEntity)entity),
                setter,
                () => new List<TProperty>(),
                (object list, object item) => ((ICollection<TProperty>)list).Add((TProperty)item),
                visitorWrapper);
            _hasManyMappings.Add(hasManyMapping);
            return new HasManyConfigurator<TEntity, TProperty>(hasManyMapping);
        }
    }
}
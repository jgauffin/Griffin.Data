using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Griffin.Data.Mappings;

namespace Griffin.Data;

/// <summary>
///     Extensions used to fetch entities from the DB.
/// </summary>
public static class CommandQueryExtensions
{
    /// <summary>
    ///     Get an entity (and its children).
    /// </summary>
    /// <param name="transaction">Transaction used to fetch data.</param>
    /// <param name="type">Tpe of entity to fetch.</param>
    /// <param name="registry">Mapping registry used to fetch entity and child mappings.</param>
    /// <param name="constraints">Constraints (with property names and values).</param>
    /// <param name="factoryMethod">Custom factory used to create the entity.</param>
    /// <returns></returns>
    /// <exception cref="EntityNotFoundException"></exception>
    public static async Task<bool> Exists<T>(
        this IDbTransaction transaction,
        IMappingRegistry registry,
        object constraints)
    {
        if (transaction == null) throw new ArgumentNullException(nameof(transaction));

        if (registry == null) throw new ArgumentNullException(nameof(registry));

        if (constraints == null) throw new ArgumentNullException(nameof(constraints));

        var mapping = registry.Get(typeof(T));
        await using var cmd = transaction.CreateCommand();
        cmd.CommandText = $"SELECT TOP(1) {mapping.Keys[0].ColumnName} FROM {mapping.TableName}";
        cmd.ApplyWhere(mapping, constraints);

        await using var reader = await cmd.ExecuteReaderAsync();
        return await reader.ReadAsync();
    }

    /// <summary>
    ///     Get an entity (and its children).
    /// </summary>
    /// <param name="transaction">Transaction used to fetch data.</param>
    /// <param name="type">Tpe of entity to fetch.</param>
    /// <param name="registry">Mapping registry used to fetch entity and child mappings.</param>
    /// <param name="constraints">Constraints (with property names and values).</param>
    /// <param name="factoryMethod">Custom factory used to create the entity.</param>
    /// <returns></returns>
    /// <exception cref="EntityNotFoundException"></exception>
    public static async Task<object?> Get(
        this IDbTransaction transaction,
        Type type,
        IMappingRegistry registry,
        object constraints,
        Func<IDataRecord, object?>? factoryMethod = null)
    {
        if (transaction == null) throw new ArgumentNullException(nameof(transaction));

        if (type == null) throw new ArgumentNullException(nameof(type));

        if (registry == null) throw new ArgumentNullException(nameof(registry));

        if (constraints == null) throw new ArgumentNullException(nameof(constraints));

        var mapping = registry.Get(type);
        var constrainsDictionary = constraints.ToDictionary();

        await using var cmd = transaction.CreateCommand();
        cmd.CommandText = $"SELECT * FROM {mapping.TableName}";
        cmd.ApplyWhere(mapping, constraints);

        var fieldsStr = string.Join(", ", constrainsDictionary.Select(x => $"{x.Key}={x.Value}"));

        object entity;

        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            if (!await reader.ReadAsync()) return null;

            var entity2 = factoryMethod?.Invoke(reader) ?? Activator.CreateInstance(type, true);
            entity = entity2 ??
                     throw new InvalidOperationException("Failed to construct entity for " + cmd.CommandText);
            reader.Map(entity, mapping);
        }

        await FetchChildren(transaction, registry, mapping, entity);
        return entity;
    }

    /// <summary>
    ///     Get an entity (and its children).
    /// </summary>
    /// <typeparam name="T">Type of entity to fetch.</typeparam>
    /// <param name="transaction">Transaction used to fetch the entity.</param>
    /// <param name="registry">Used to lookup the entity and child mappings.</param>
    /// <param name="constraints">Constrains consisting of property names and values (in an anonymous object).</param>
    /// <returns>Loaded entity.</returns>
    /// <exception cref="EntityNotFoundException">Entity was not found.</exception>
    public static async Task<T> Get<T>(
        this IDbTransaction transaction,
        IMappingRegistry registry,
        object constraints)
    {
        var result = await Get(transaction, typeof(T), registry, constraints);
        if (result != null) return (T)result;
        var ps = string.Join(", ", constraints.ToDictionary().Select(x => $"{x.Key} = {x.Value}"));
        throw new EntityNotFoundException($"Failed to find {typeof(T).Name} with constrains {ps}.");
    }

    /// <summary>
    ///     Get an entity (and its children).
    /// </summary>
    /// <typeparam name="TEntity">Type of entity to fetch.</typeparam>
    /// <typeparam name="TKey">Key type (primitive type)</typeparam>
    /// <param name="transaction">Transaction used to fetch the entity.</param>
    /// <param name="registry">Used to lookup the entity and child mappings.</param>
    /// <param name="id">Primary key value.</param>
    /// <returns>Loaded entity.</returns>
    /// <exception cref="EntityNotFoundException">Entity was not found.</exception>
    public static Task<TEntity> GetById<TEntity>(
        this IDbTransaction transaction,
        IMappingRegistry registry,
        int id,
        bool fetchChildren = true)
    {
        return GetById<TEntity, int>(transaction, registry, id, fetchChildren);
    }

    /// <summary>
    ///     Get an entity (and its children).
    /// </summary>
    /// <typeparam name="TEntity">Type of entity to fetch.</typeparam>
    /// <typeparam name="TKey">Key type (primitive type)</typeparam>
    /// <param name="transaction">Transaction used to fetch the entity.</param>
    /// <param name="registry">Used to lookup the entity and child mappings.</param>
    /// <param name="id">Primary key value.</param>
    /// <returns>Loaded entity.</returns>
    /// <exception cref="EntityNotFoundException">Entity was not found.</exception>
    public static Task<TEntity> GetById<TEntity>(
        this IDbTransaction transaction,
        IMappingRegistry registry,
        string id,
        bool fetchChildren = true)
    {
        return GetById<TEntity, string>(transaction, registry, id, fetchChildren);
    }


    /// <summary>
    ///     Get an entity (and its children).
    /// </summary>
    /// <typeparam name="TEntity">Type of entity to fetch.</typeparam>
    /// <typeparam name="TKey">Key type (primitive type)</typeparam>
    /// <param name="transaction">Transaction used to fetch the entity.</param>
    /// <param name="registry">Used to lookup the entity and child mappings.</param>
    /// <param name="id">Primary key value.</param>
    /// <returns>Loaded entity.</returns>
    /// <exception cref="EntityNotFoundException">Entity was not found.</exception>
    public static async Task<TEntity> GetById<TEntity, TKey>(
        this IDbTransaction transaction,
        IMappingRegistry registry,
        TKey id,
        bool fetchChildren = true)
    {
        var mapping = registry.Get<TEntity>();
        if (mapping.Keys.Count != 1)
            throw new InvalidOperationException(
                $"This function requires only one key, which {typeof(TEntity).FullName} has not.");

        var key = mapping.Keys[0];
        await using var cmd = transaction.CreateCommand();
        cmd.CommandText = $"SELECT * FROM {mapping.TableName} WHERE {key.ColumnName}=@{key.PropertyName}";
        cmd.AddParameter(key.PropertyName, id);

        TEntity entity;
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            if (!await reader.ReadAsync())
                throw new EntityNotFoundException($"Failed to find {typeof(TEntity).Name} with id {id}.");

            entity = (TEntity)Activator.CreateInstance(typeof(TEntity), true)!;
            reader.Map(entity, mapping);
        }

        if (fetchChildren)
            // This must be outside the using
            // so that two data readers aren't open at the same time.
            await FetchChildren(transaction, registry, mapping, entity);

        return entity;
    }

    /// <summary>
    ///     Map a record to an entity.
    /// </summary>
    /// <typeparam name="T">Type of entity.</typeparam>
    /// <param name="record">Data record (db row)</param>
    /// <param name="entity">Entity to assign values to.</param>
    /// <param name="mapping">Mapping used to lookup columns/properties.</param>
    /// <exception cref="ArgumentNullException">Any of the fields are null.</exception>
    public static void Map<T>(this IDataRecord record, [DisallowNull] T entity, ClassMapping mapping)
    {
        if (record == null) throw new ArgumentNullException(nameof(record));

        if (entity == null) throw new ArgumentNullException(nameof(entity));

        if (mapping == null) throw new ArgumentNullException(nameof(mapping));

        var values = new object[record.FieldCount];
        record.GetValues(values);
        var names = new string[record.FieldCount];
        for (var i = 0; i < record.FieldCount; i++) names[i] = record.GetName(i);

        foreach (var property in mapping.Keys)
        {
            var value = record[property.ColumnName];
            property.SetColumnValue(entity, value);
        }

        foreach (var property in mapping.Properties)
        {
            if (!property.CanReadFromDatabase) continue;

            var value = record[property.ColumnName];
            if (value is DBNull) continue;

            //if (property.ToPropertyConverter != null)
            //{
            //    value = property.ToPropertyConverter(new ToPropertyValueConverterContext(value, record));
            //}

            property.SetColumnValue(entity, value);
        }
    }


    /// <summary>
    ///     Query to fetch a list of items (and their children).
    /// </summary>
    /// <typeparam name="T">Type of entities to fetch.</typeparam>
    /// <param name="transaction">Transaction to perform the fetch operation in.</param>
    /// <param name="mappingRegistry">Used to find mappings for the entity and its children.</param>
    /// <param name="propertyConstraints">Anonymous object with property names and values.</param>
    /// <param name="entityFactory">Custom factory to create the entities (if types differ between rows).</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">transaction or mappingRegistry are null</exception>
    /// <exception cref="InvalidOperationException">Failed to connect main and child entities.</exception>
    public static async Task<IReadOnlyList<T>> QuerySql<T>(this IDbTransaction transaction,
        IMappingRegistry mappingRegistry, string sql, object parameters, bool loadChildren = false)
    {
        var mapping = mappingRegistry.Get<T>();

        if (sql.Trim().StartsWith("WHERE", StringComparison.OrdinalIgnoreCase))
            sql = $"SELECT * FROM {mapping.TableName} {sql}";

        await using var command = transaction.CreateCommand();
        command.CommandText = sql;
        foreach (var kvp in parameters.ToDictionary())
        {
            var prop = mapping.FindPropertyByName(kvp.Key);
            if (prop?.PropertyToColumnConverter != null && kvp.Value != null)
            {
                var v = prop.PropertyToColumnConverter(kvp.Value) ?? kvp.Value;
                command.AddParameter(kvp.Key, v);
            }
            else
            {
                command.AddParameter(kvp.Key, kvp.Value);
            }
        }

        return await FillUsingReader<T>(mappingRegistry, command, loadChildren: loadChildren);
    }

    /// <summary>
    ///     Query to fetch a list of items (and their children).
    /// </summary>
    /// <typeparam name="T">Type of entities to fetch.</typeparam>
    /// <param name="transaction">Transaction to perform the fetch operation in.</param>
    /// <param name="mappingRegistry">Used to find mappings for the entity and its children.</param>
    /// <param name="propertyConstraints">Anonymous object with property names and values.</param>
    /// <param name="entityFactory">Custom factory to create the entities (if types differ between rows).</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">transaction or mappingRegistry are null</exception>
    /// <exception cref="InvalidOperationException">Failed to connect main and child entities.</exception>
    public static async Task<IReadOnlyList<T>> Query<T>(
        this IDbTransaction transaction,
        IMappingRegistry mappingRegistry,
        object? propertyConstraints = null,
        Func<IDataRecord, object>? entityFactory = null,
        bool loadChildren = false)
    {
        if (transaction == null) throw new ArgumentNullException(nameof(transaction));

        if (mappingRegistry == null) throw new ArgumentNullException(nameof(mappingRegistry));

        var mapping = mappingRegistry.Get<T>();
        await using var cmd = transaction.CreateCommand();
        cmd.CommandText = $"SELECT * FROM {mapping.TableName}";
        if (propertyConstraints != null) cmd.ApplyWhere(mapping, propertyConstraints);

        return await FillUsingReader<T>(mappingRegistry, cmd, entityFactory);
    }

    private static async Task<IReadOnlyList<T>> FillUsingReader<T>(IMappingRegistry mappingRegistry, DbCommand cmd,
        Func<IDataRecord, object>? entityFactory = null,
        bool loadChildren = false)
    {
        var transaction = cmd.Transaction;
        var mapping = mappingRegistry.Get<T>();
        var mainEntities = new List<T>();
        await using (var reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                var entity = entityFactory != null
                    ? (T)entityFactory(reader)
                    : (T)Activator.CreateInstance(typeof(T), true)!;

                // The custom factory reported null
                // = skip this entity.
                if (entity == null) continue;

                reader.Map(entity, mapping);
                mainEntities.Add(entity);
            }
        }

        if (!mainEntities.Any() || !loadChildren) return mainEntities;

        foreach (var hasOne in mapping.Children)
        {
            var parentIds = mainEntities.Select(x => hasOne.ForeignKey.GetColumnValue(x!)).ToList();
            var childMapping = mappingRegistry.Get(hasOne.ChildEntityType);
            var constraints = new Dictionary<string, object> { [hasOne.ForeignKey.ForeignKeyPropertyName] = parentIds };
            var children = await transaction.Query(hasOne.ChildEntityType, childMapping, constraints);
            foreach (var child in children)
            {
                var fkValue = hasOne.ForeignKey.GetColumnValue(child);
                var mainEntity = mainEntities.FirstOrDefault(x =>
                    hasOne.ForeignKey.ReferencedProperty.GetColumnValue(x!)!.Equals(fkValue));
                if (mainEntity == null)
                    throw new InvalidOperationException("Failed to find main entity for child " + child);

                hasOne.SetColumnValue(mainEntity, child);
            }
        }

        foreach (var hasManyMapping in mapping.Collections)
        {
            var parentIds = mainEntities.Select(x => hasManyMapping.ForeignKey.ReferencedProperty.GetColumnValue(x!))
                .ToList();
            var childMapping = mappingRegistry.Get(hasManyMapping.ElementType);
            var constraints = new Dictionary<string, object>
                { [hasManyMapping.ForeignKey.ForeignKeyPropertyName] = parentIds };
            //if (hasManyMapping.Denominator != null && hasManyMapping.DenominatorValue == null)
            //{
            //    constraints[hasManyMapping.Denominator.PropertyName] = hasManyMapping.DenominatorValue!;
            //}

            var children = await transaction.Query(hasManyMapping.ElementType, childMapping, constraints);


            var childLists = new Dictionary<object, IList>();
            foreach (var mainEntity in mainEntities)
            {
                if (mainEntity == null) throw new InvalidOperationException("Expected item to exist in list.");

                var fkValue = hasManyMapping.ForeignKey.ReferencedProperty.GetColumnValue(mainEntity);
                if (fkValue == null)
                    throw new InvalidOperationException($"Failed to get key from parent {typeof(T).Name}.");

                var collection = hasManyMapping.CreateCollection();
                hasManyMapping.SetColumnValue(mainEntity, collection);
                childLists[fkValue] = collection;
            }

            foreach (var child in children)
            {
                var list = hasManyMapping.ForeignKey.GetColumnValue(child);
                if (list == null) throw new InvalidOperationException("Expected item.");

                childLists[list].Add(child);
            }
        }

        return mainEntities;
    }

    /// <summary>
    ///     Query to fetch a list of items (and their children).
    /// </summary>
    /// <param name="transaction">Transaction to perform the fetch operation in.</param>
    /// <param name="type">Type of entities to fetch.</param>
    /// <param name="propertyConstraints">Anonymous object with property names and values.</param>
    /// <param name="mapping">Mapping to use.</param>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException">transaction or mappingRegistry are null</exception>
    /// <exception cref="InvalidOperationException">Failed to connect main and child entities.</exception>
    public static async Task<IList> Query(
        this IDbTransaction transaction,
        Type type,
        ClassMapping mapping,
        object propertyConstraints)
    {
        await using var cmd = transaction.CreateCommand();
        cmd.CommandText = $"SELECT * FROM {mapping.TableName}";
        cmd.ApplyWhere(mapping, propertyConstraints);

        var items = (IList)Activator.CreateInstance(typeof(List<>).MakeGenericType(type))!;

        await using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var entity = Activator.CreateInstance(type, true)!;
            reader.Map(entity, mapping);
            items.Add(entity);
        }

        return items;
    }

    private static async Task FetchChildren<T>(
        IDbTransaction transaction,
        IMappingRegistry registry,
        ClassMapping parentMapping,
        [DisallowNull] T parentEntity)
    {
        foreach (var hasOne in parentMapping.Children)
        {
            var foreignKeyValue = hasOne.ForeignKey.ReferencedProperty.GetColumnValue(parentEntity);
            var foreignKeyName = hasOne.ForeignKey.ForeignKeyPropertyName;

            var fetchConstraints = new Dictionary<string, object?> { { foreignKeyName, foreignKeyValue } };
            var childEntity = await transaction.Get(hasOne.ChildEntityType, registry, fetchConstraints);
            if (childEntity == null) continue;

            hasOne.SetColumnValue(parentEntity, childEntity);
        }

        foreach (var hasMany in parentMapping.Collections)
        {
            var childClassMapping = registry.Get(hasMany.ElementType);
            var foreignKeyValue = hasMany.ForeignKey.ReferencedProperty.GetColumnValue(parentEntity);
            var foreignKeyName = hasMany.ForeignKey.ForeignKeyPropertyName;
            var fetchConstraints = new Dictionary<string, object?> { { foreignKeyName, foreignKeyValue } };

            //if (hasMany.Denominator != null)
            //{
            //    fetchConstraints[hasMany.Denominator.PropertyName] = hasMany.DenominatorValue;
            //}

            //Func<IDataRecord, object?> factoryMethod;
            //if (hasMany.ChildFactoryProperty != null && hasMany.ChildFactory != null)
            //{
            //    factoryMethod = record =>
            //    {
            //        var denominatorValue = hasMany.ChildFactoryProperty.GetColumnValue(parentEntity);
            //        if (denominatorValue == null)
            //        {
            //            throw new InvalidOperationException("Expected a denominator value.");
            //        }

            //        var ctx = new ChildFactoryContext(parentEntity, record, denominatorValue);
            //        return hasMany.ChildFactory(ctx);
            //    };
            //}
            //else
            //{
            //    factoryMethod = _ => Activator.CreateInstance(hasMany.ChildProperty.PropertyType, true)!;
            //}

            var childEntity = await transaction.Query(hasMany.ElementType, childClassMapping, fetchConstraints);
            hasMany.SetColumnValue(parentEntity, childEntity);
        }
    }
}
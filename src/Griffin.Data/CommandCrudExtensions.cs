using System;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Griffin.Data.Mappings;
using Griffin.Data.Mappings.Properties;

namespace Griffin.Data;

/// <summary>
///     Extensions for CRUD operations directly on a command.
/// </summary>
public static class CommandCrudExtensions
{
    /// <summary>
    ///     Delete a entity (children will not be deleted when using a command).
    /// </summary>
    /// <param name="command">Command to add CommandText/Parameters to.</param>
    /// <param name="mapping">Mapping used to fetch keys from the entity.</param>
    /// <param name="entity">Entity to delete.</param>
    /// <returns>task.</returns>
    /// <exception cref="ArgumentNullException">Any of the arguments are null.</exception>
    public static async Task Delete(this IDbCommand command, ClassMapping mapping, object entity)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        if (mapping == null) throw new ArgumentNullException(nameof(mapping));

        if (entity == null) throw new ArgumentNullException(nameof(entity));

        var cmd = (DbCommand)command;
        cmd.CommandText = $"DELETE FROM {mapping.TableName}";
        cmd.ApplyKeyWhere(mapping, entity);
        await cmd.WrapExecution(() => cmd.ExecuteNonQueryAsync());
    }

    /// <summary>
    ///     Delete a entity (and all children).
    /// </summary>
    /// <param name="transaction">Command to add CommandText/Parameters to.</param>
    /// <param name="mappingRegistry">Used to lookup entity and child mappings.</param>
    /// <param name="entity">Entity to delete.</param>
    /// <returns>task.</returns>
    /// <exception cref="ArgumentNullException">Any of the arguments are null.</exception>
    public static async Task Delete(this IDbTransaction transaction, IMappingRegistry mappingRegistry, object entity)
    {
        if (transaction == null) throw new ArgumentNullException(nameof(transaction));

        if (mappingRegistry == null) throw new ArgumentNullException(nameof(mappingRegistry));

        if (entity == null) throw new ArgumentNullException(nameof(entity));

        var mapping = mappingRegistry.Get(entity.GetType());
        foreach (var child in mapping.Children)
        {
            var childMapping = mappingRegistry.Get(child.ChildEntityType);

            await using var cmd2 = transaction.CreateCommand();
            cmd2.CommandText =
                $"DELETE FROM {childMapping.TableName} WHERE {child.ForeignKey.ForeignKeyColumnName}=@fk";
            var fkValue = child.ForeignKey.ReferencedProperty.GetColumnValue(entity);
            cmd2.AddParameter("fk", fkValue);
            await cmd2.ExecuteNonQueryAsync();
        }

        foreach (var child in mapping.Collections)
        {
            var childMapping = mappingRegistry.Get(child.ElementType);

            await using var cmd2 = transaction.CreateCommand();
            cmd2.CommandText =
                $"DELETE FROM {childMapping.TableName} WHERE {child.ForeignKey.ForeignKeyColumnName}=@fk";
            cmd2.AddParameter("fk", child.ForeignKey.ReferencedProperty.GetColumnValue(entity));
            await cmd2.ExecuteNonQueryAsync();
        }

        await using var cmd = transaction.CreateCommand();
        await cmd.Delete(mapping, entity);
    }

    /// <summary>
    ///     Insert a new entity (and all it's children).
    /// </summary>
    /// <param name="transaction">Transaction to perform operation on.</param>
    /// <param name="mappingRegistry">Used to lookup entity and child mappings.</param>
    /// <param name="entity">Entity to insert.</param>
    /// <returns>task.</returns>
    /// <exception cref="ArgumentNullException">Any of the arguments are null.</exception>
    public static async Task Insert(this IDbTransaction transaction, IMappingRegistry mappingRegistry, object entity)
    {
        if (transaction == null) throw new ArgumentNullException(nameof(transaction));

        if (mappingRegistry == null) throw new ArgumentNullException(nameof(mappingRegistry));

        if (entity == null) throw new ArgumentNullException(nameof(entity));

        var mapping = mappingRegistry.Get(entity.GetType());

        await using var cmd = transaction.CreateCommand();
        await cmd.Insert(mapping, entity);

        await PersistChildren(transaction, mappingRegistry, entity, mapping);
    }

    /// <summary>
    ///     Insert a new entity (children are not inserted).
    /// </summary>
    /// <param name="command">Command to add CommandText/Parameters to.</param>
    /// <param name="mapping">Mapping to use.</param>
    /// <param name="entity">Entity to insert.</param>
    /// <returns>task.</returns>
    /// <exception cref="ArgumentNullException">Any of the arguments are null.</exception>
    public static async Task Insert(this IDbCommand command, ClassMapping mapping, object entity)
    {
        if (command == null) throw new ArgumentNullException(nameof(command));

        if (mapping == null) throw new ArgumentNullException(nameof(mapping));

        if (entity == null) throw new ArgumentNullException(nameof(entity));

        var allProps = mapping.Properties.Cast<IFieldMapping>().Union(mapping.Keys.Where(x => !x.IsAutoIncrement))
            .ToList();
        var cols = string.Join(",", allProps.Select(x => x.ColumnName));
        var values = string.Join(",", allProps.Select(x => '@' + x.PropertyName));
        command.CommandText = $"INSERT INTO {mapping.TableName} ({cols})\r\n  VALUES({values})";

        // Start by getting denominator
        //foreach (var property in mapping.Children.Where(x=>x.Denominator != null))
        //{
        //    property.Denominator!.SetValue(entity, property.DenominatorValue);
        //}

        foreach (var property in allProps)
        {
            var value = property.GetColumnValue(entity);
            command.AddParameter(property.PropertyName, value ?? DBNull.Value);
        }

        var cmd = (DbCommand)command;
        var autoIncrement = mapping.Keys.FirstOrDefault(x => x.IsAutoIncrement);
        if (autoIncrement == null)
        {
            await cmd.WrapExecution(() => cmd.ExecuteNonQueryAsync());
            return;
        }

        command.CommandText += ";SELECT CAST(SCOPE_IDENTITY() as int)";
        await cmd.WrapExecution(async () =>
        {
            var value = await cmd.ExecuteScalarAsync();
            var id = (int)value!;
            autoIncrement.SetColumnValue(entity, id);
        });
    }

    /// <summary>
    ///     Update entity (children are not being updated).
    /// </summary>
    /// <param name="transaction">Transaction to perform operation in.</param>
    /// <param name="mappingRegistry">Registry used to lookup mappings.</param>
    /// <param name="entity">Entity to update.</param>
    /// <returns></returns>
    /// <remarks>
    ///     <para>
    ///         Child updates are not performed as it's impossible to know if children have been added/updated/removed without
    ///         change tracking.
    ///     </para>
    /// </remarks>
    public static async Task Update(this IDbTransaction transaction, IMappingRegistry mappingRegistry, object entity)
    {
        await using var cmd = transaction.CreateCommand();
        var mapping = mappingRegistry.Get(entity.GetType());
        await cmd.Update(mapping, entity);
    }

    /// <summary>
    ///     Update entity (children are not being updated).
    /// </summary>
    /// <param name="command">Transaction to perform operation in.</param>
    /// <param name="mapping">Mapping to use.</param>
    /// <param name="entity">Entity to update.</param>
    /// <returns></returns>
    /// <remarks>
    ///     <para>
    ///         Child updates are not performed as it's impossible to know if children have been added/updated/removed without
    ///         change tracking.
    ///     </para>
    /// </remarks>
    public static async Task Update(this IDbCommand command, ClassMapping mapping, object entity)
    {
        var allProps = mapping.Properties.Where(x => x.CanWriteToDatabase).ToList();

        var cols = string.Join(", ", allProps.Select(x => $"{x.ColumnName} = @{x.PropertyName}"));
        command.CommandText = $"UPDATE {mapping.TableName} SET {cols}";
        command.ApplyKeyWhere(mapping, entity);

        foreach (var property in allProps) command.AddParameter(property.PropertyName, property.GetColumnValue(entity));

        var cmd = (DbCommand)command;
        await cmd.WrapExecution(() => cmd.ExecuteNonQueryAsync());
    }


    private static async Task PersistChildren(
        IDbTransaction transaction,
        IMappingRegistry mappingRegistry,
        object entity,
        ClassMapping mapping)
    {
        foreach (var hasOne in mapping.Children)
        {
            var foreignKeyValue = hasOne.ForeignKey.ReferencedProperty.GetColumnValue(entity);
            var childEntity = hasOne.GetColumnValue(entity);
            if (childEntity == null) continue;

            hasOne.ForeignKey.SetColumnValue(childEntity, foreignKeyValue);
            await transaction.Insert(mappingRegistry, childEntity);
        }

        foreach (var hasMany in mapping.Collections)
        {
            var col = hasMany.GetColumnValue(entity);
            if (col == null) continue;

            var foreignKeyValue = hasMany.ForeignKey.ReferencedProperty.GetColumnValue(entity);
            await hasMany.Visit(col, async childEntity =>
            {
                hasMany.ForeignKey.SetColumnValue(childEntity, foreignKeyValue);
                await transaction.Insert(mappingRegistry, childEntity);
            });
        }
    }

    private static async Task WrapExecution(this IDbCommand command, Func<Task> action)
    {
        try
        {
            await action();
        }
        catch (Exception exception)
        {
            throw command.CreateDetailedException(exception);
        }
    }
}
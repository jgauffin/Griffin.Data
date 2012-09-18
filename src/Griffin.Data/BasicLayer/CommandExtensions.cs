using System;
using System.Collections.Generic;
using System.Data;
using Griffin.Data.Mappings;
using ITableMapping = Griffin.Data.BasicLayer.ITableMapping;

namespace Griffin.Data.BasicLayer
{
    /// <summary>
    /// Extension methods for <see cref="IDbCommand"/>.
    /// </summary>
    /// <remarks>These extension methods requires that the mapping class implements <see cref="ITableMapping"/>.</remarks>
    public static class CommandExtensions
    {
        /// <summary>
        /// Load a single entity
        /// </summary>
        /// <typeparam name="TEntity">Entity to load</typeparam>
        /// <param name="command">A command object</param>
        /// <param name="id">Primary key</param>
        /// <returns>Entity if found; otherwise <c>null</c>.</returns>
        /// <remarks>Requires that the mapping class implements <see cref="ITableMapping"/>.</remarks>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// using (var cmd = _connection.CreateCommand())
        /// {
        ///     return cmd.ExecuteScalar<User>(1);
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static TEntity ExecuteScalar<TEntity>(this IDbCommand command, object id) where TEntity : class
        {
            if (command == null) throw new ArgumentNullException("command");
            if (id == null) throw new ArgumentNullException("id");

            var mapper = MapperProvider.Instance.GetMapper<TEntity>();
            var tableMapping = mapper as ITableMapping;
            if (tableMapping == null)
                throw new MappingException(
                    string.Format(
                        "The mapper for {0} do not implement ITableMapping which is required by this method.",
                        typeof (TEntity)));

            command.CommandText = string.Format("SELECT * FROM {0} WHERE {1} = @id", tableMapping.TableName,
                                                tableMapping.IdColumnName);
            command.AddParameter("@id", id);

            using (var reader = command.ExecuteReader())
            {
                return !reader.Read() ? null : mapper.Map(reader);
            }
        }

        /// <summary>
        /// Execute a query where the "WHERE" clause is generated for you
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="command"></param>
        /// <param name="parameters">columns which must be specified.</param>
        /// <returns>Lazy loading list</returns>
        /// <remarks>Requires that the mapping class implements <see cref="ITableMapping"/>.</remarks>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// using (var cmd = _connection.CreateCommand())
        /// {
        ///     return cmd.ExecuteQuery<User>(new { first_name = "Arne", last_name="Kalle" }).FirstOrDefault();
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static IEnumerable<TEntity> ExecuteQuery<TEntity>(this IDbCommand command, object parameters)
            where TEntity : class
        {
            if (command == null) throw new ArgumentNullException("command");

            var mapper = MapperProvider.Instance.GetMapper<TEntity>();
            var tableMapping = mapper as ITableMapping;
            if (tableMapping == null)
                throw new MappingException(
                    string.Format(
                        "The mapper for {0} do not implement ITableMapping which is required by this method.",
                        typeof (TEntity)));

            command.CommandText = string.Format("SELECT * FROM {0}", tableMapping.TableName);

            if (parameters != null)
            {
                command.CommandText += " WHERE ";
                foreach (var propertyInfo in parameters.GetType().GetProperties())
                {
                    command.CommandText += " " + propertyInfo.Name + " = @" + propertyInfo.Name + " AND ";
                    command.AddParameter(propertyInfo.Name, propertyInfo.GetValue(parameters, null));
                }
                command.CommandText = command.CommandText.Remove(command.CommandText.Length - 5, 5);
            }

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    yield return mapper.Map(reader);
                }
            }
        }
    }
}
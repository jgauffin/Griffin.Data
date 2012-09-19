using System;
using System.Collections.Generic;
using System.Data;
using Griffin.Data.Mappings;

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
        /// Transforms the SQL query into a <c>Count(*)</c>, runs a scalar execute and then restores the query.
        /// </summary>
        /// <param name="command">Command to use</param>
        /// <returns>Number of rows</returns>
        public static int Count(this IDbCommand command)
        {
            if (command == null) throw new ArgumentNullException("command");
            
            var pos = command.CommandText.IndexOf("FROM", StringComparison.OrdinalIgnoreCase);
            if (pos == -1)
                throw new NotSupportedException("Failed to find FROM in the SQL query");

            var org = command.CommandText;
            command.CommandText = "SELECT count(*) " + command.CommandText.Substring(pos);
            var result = (int)command.ExecuteScalar();

            command.CommandText = org;
            return result;
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
        ///     return cmd.ExecuteLazyQuery<User>(new { first_name = "Arne", last_name="Kalle" }).FirstOrDefault();
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static IEnumerable<TEntity> ExecuteQuery<TEntity>(this IDbCommand command, object parameters)
            where TEntity : class
        {
            if (command == null) throw new ArgumentNullException("command");

            PrepareCommand<TEntity>(command, parameters);

            return command.ExecuteQuery<TEntity>();
        }

        /// <summary>
        /// Execute a query where the "WHERE" clause is generated for you
        /// </summary>
        /// <typeparam name="TEntity">Entity</typeparam>
        /// <param name="command">Command to use</param>
        /// <param name="parameters">columns which must be specified.</param>
        /// <returns>Lazy loading list</returns>
        /// <remarks>
        /// <para>Everything is lazy loaded, which means that you may not dispose the command. It will be disposed for you when the returned list is disposed.
        /// </para>
        /// <para>Requires that the mapping class implements <see cref="ITableMapping"/>.</para>
        /// </remarks>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// using (var cmd = _connection.CreateCommand())
        /// {
        ///     return cmd.ExecuteLazyQuery<User>(new { FirstName = "Arne", LastName = "Kalle" }).FirstOrDefault();
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static IEnumerable<TEntity> ExecuteLazyQuery<TEntity>(this IDbCommand command, object parameters)
            where TEntity : class
        {
            if (command == null) throw new ArgumentNullException("command");

            PrepareCommand<TEntity>(command, parameters);

            return command.ExecuteLazyQuery<TEntity>();
        }


        private static void PrepareCommand<TEntity>(IDbCommand command, object parameters) where TEntity : class
        {
            var mapper = MapperProvider.Instance.GetMapper<TEntity>();
            var tableMapping = mapper as ITableMapping;
            if (tableMapping == null)
                throw new MappingException(
                    string.Format(
                        "The mapper for {0} do not implement ITableMapping which is required by this method.",
                        typeof (TEntity)));
            
            command.CommandText = string.Format("SELECT * FROM {0}", tableMapping.TableName);

            if (parameters == null) 
                return;

            command.CommandText += " WHERE ";
            foreach (var propertyInfo in parameters.GetType().GetProperties())
            {
                command.CommandText += " " + tableMapping.GetColumnName(propertyInfo.Name) + " = @" + propertyInfo.Name + " AND ";
                command.AddParameter(propertyInfo.Name, propertyInfo.GetValue(parameters, null));
            }
            command.CommandText = command.CommandText.Remove(command.CommandText.Length - 5, 5);
        }
    }
}
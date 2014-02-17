using System;
using System.Collections.Generic;
using System.Data;
using Griffin.Data.Queries;

namespace Griffin.Data.Mappings
{
    /// <summary>
    /// Extension methods for <see cref="IDbCommand"/>.
    /// </summary>
    public static class CommandExtensions
    {
        /// <summary>
        /// Execute a query and map the result.
        /// </summary>
        /// <typeparam name="TEntity">Entity type to return</typeparam>
        /// <param name="command">Command to execute</param>
        /// <returns>Lazy list. i.e. each row is mapped when requested.</returns>
        /// <remarks>
        /// <para>Does not execute the query until the list is traversed. And will not populate the
        /// list but map each row when it's really traversed. This means that you may not dispose the command,
        /// it will instead be disposed when the return collection is returned.</para>
        /// <para>Requires that a mapping has been registered in <see cref="MapperProvider"/>.</para>
        /// </remarks>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// using (var cmd = _connection.CreateCommand())
        /// {
        ///     cmd.CommandText = "SELECT * FROM User";
        ///     return cmd.ExecuteLazyQuery<User>();
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static IEnumerable<TEntity> ExecuteLazyQuery<TEntity>(this IDbCommand command) where TEntity : class
        {
            if (command == null) throw new ArgumentNullException("command");

            var mapper = MapperProvider.Instance.GetMapper<TEntity>();
            return new LazyLoadingResult<TEntity>(command, mapper);
            /*
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    yield return mapper.Map(reader);
                }
            }*/
        }

        /// <summary>
        /// Execute a query and map the result.
        /// </summary>
        /// <typeparam name="TEntity">Entity type to return</typeparam>
        /// <param name="command">Command to execute</param>
        /// <returns>Lazy list. i.e. each row is mapped when requested.</returns>
        /// <remarks>
        /// <para>Will populate the list and execute the query before anything is returned. You can safely dispose the command ater invoking this command.</para>
        /// <para>Requires that a mapping has been registered in <see cref="MapperProvider"/>.</para>
        /// </remarks>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// using (var cmd = _connection.CreateCommand())
        /// {
        ///     cmd.CommandText = "SELECT * FROM User";
        ///     return cmd.ExecuteLazyQuery<User>();
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static IEnumerable<TEntity> ExecuteQuery<TEntity>(this IDbCommand command) where TEntity : class
        {
            if (command == null) throw new ArgumentNullException("command");

            var mapper = MapperProvider.Instance.GetMapper<TEntity>();

            var entries = new List<TEntity>();
            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    entries.Add(mapper.Map(reader));
                }
            }

            return entries;
        }
   }
}
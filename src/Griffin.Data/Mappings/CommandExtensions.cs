using System;
using System.Collections.Generic;
using System.Data;

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
        /// <remarks>Requires that a mapping has been registered in <see cref="MapperProvider"/>.</remarks>
        /// <example>
        /// <code>
        /// <![CDATA[
        /// using (var cmd = _connection.CreateCommand())
        /// {
        ///     cmd.CommandText = "SELECT * FROM User";
        ///     return cmd.ExequteQuery<User>();
        /// }
        /// ]]>
        /// </code>
        /// </example>
        public static IEnumerable<TEntity> ExecuteQuery<TEntity>(this IDbCommand command) where TEntity : class
        {
            if (command == null) throw new ArgumentNullException("command");

            var mapper = MapperProvider.Instance.GetMapper<TEntity>();
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
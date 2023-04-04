using System;
using System.Threading.Tasks;
using Griffin.Data.Mapper.Helpers;

namespace Griffin.Data.Mapper
{
    internal static class GetOneExtensions
    {
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="session"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static async Task<TEntity?> FirstOrDefault<TEntity>(this Session session, QueryOptions options)
        {
            options.PageSize = 1;

            var mapping = session.GetMapping<TEntity>();
            TEntity? entity;
            await using (var cmd = session.CreateQueryCommand(typeof(TEntity), options))
            {
                entity = await cmd.GetSingleOrDefault<TEntity>(mapping);
            }

            if (entity == null)
            {
                return default;
            }

            if (options.LoadChildren)
            {
                await session.GetChildren(entity);
            }

            session.Track((object)entity);
            return entity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="TEntity"></typeparam>
        /// <param name="session"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static async Task<TEntity> First<TEntity>(this Session session, QueryOptions options) where TEntity : notnull
        {
            options.PageSize = 1;

            var mapping = session.GetMapping<TEntity>();
            TEntity entity;
            await using (var cmd = session.CreateQueryCommand(typeof(TEntity), options))
            {
                entity = await cmd.GetSingle<TEntity>(mapping);
            }

            if (options.LoadChildren)
            {
                await session.GetChildren(entity);
            }

            session.Track(entity);
            return entity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="session"></param>
        /// <param name="entityType"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static async Task<object?> FirstOrDefault(this Session session, Type entityType, QueryOptions options)
        {
            options.PageSize = 1;

            var mapping = session.GetMapping(entityType);
            object? entity;
            await using (var cmd = session.CreateQueryCommand(entityType, options))
            {
                entity = await cmd.GetSingleOrDefault(mapping, options);
            }

            if (entity == null)
            {
                return default;
            }

            if (options.LoadChildren)
            {
                await session.GetChildren(entity);
            }

            session.Track(entity);
            return entity;
        }
    }
}

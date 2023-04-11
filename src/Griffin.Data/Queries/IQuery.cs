using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Griffin.Data.Queries
{
    public interface IQuery<TResult>
    {
    }

    public interface IQueryRunner<in TQuery, TQueryResult> where TQuery : IQuery<TQueryResult>
    {
        Task<TQueryResult> Execute(TQuery query);
    }
}

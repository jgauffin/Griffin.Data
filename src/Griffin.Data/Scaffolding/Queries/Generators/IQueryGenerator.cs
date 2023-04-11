using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Griffin.Data.Scaffolding.Queries.Meta;

namespace Griffin.Data.Scaffolding.Queries.Generators
{
    internal interface IQueryGenerator
    {
        Task<GeneratedFile> Generate(QueryMeta meta);
    }
}

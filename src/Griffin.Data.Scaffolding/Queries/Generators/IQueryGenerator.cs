using Griffin.Data.Scaffolding.Queries.Meta;

namespace Griffin.Data.Scaffolding.Queries.Generators;

internal interface IQueryGenerator
{
    Task<GeneratedFile> Generate(QueryMeta meta);
}

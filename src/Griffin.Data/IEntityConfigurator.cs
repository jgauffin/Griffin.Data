using Griffin.Data.Configuration;

namespace Griffin.Data;

public interface IEntityConfigurator<TEntity>
{
    void Configure(IClassMappingConfigurator<TEntity> config);
}
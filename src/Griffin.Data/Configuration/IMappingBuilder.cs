using Griffin.Data.Mappings;

namespace Griffin.Data.Configuration;

public interface IMappingBuilder
{
    ClassMapping BuildMapping();
    void BuildRelations(IMappingRegistry registry);
}
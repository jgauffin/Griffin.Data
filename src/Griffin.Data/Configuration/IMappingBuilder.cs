using Griffin.Data.Mappings;

namespace Griffin.Data.Configuration;

public interface IMappingBuilder
{
    ClassMapping BuildMapping(bool pluralizeTableNames);
    void BuildRelations(IMappingRegistry registry);
}

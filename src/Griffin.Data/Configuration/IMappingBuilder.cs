using Griffin.Data.Mappings;

namespace Griffin.Data.Configuration;

public interface IMappingBuilder
{
    public ClassMapping BuildMapping();
}
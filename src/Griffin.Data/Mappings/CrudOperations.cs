using Griffin.Data.Dialects;

namespace Griffin.Data.Mappings;

public class CrudOperations
{
    private readonly IMappingRegistry _mappingRegistry;
    private ISqlDialect _dialect;

    public CrudOperations(IMappingRegistry mappingRegistry, ISqlDialect dialect)
    {
        _mappingRegistry = mappingRegistry;
        _dialect = dialect;
    }
}
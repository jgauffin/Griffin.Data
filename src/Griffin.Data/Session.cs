using System.Data;
using Griffin.Data.Mappings;

namespace Griffin.Data;

public class DbConfiguration
{
    public IMappingRegistry MappingRegistry { get; set; }
}

internal class Session
{
    private readonly IDbTransaction _transaction;
    private MappingRegistry _registry;

    public Session(IDbTransaction transaction)
    {
        _transaction = transaction;
    }
}
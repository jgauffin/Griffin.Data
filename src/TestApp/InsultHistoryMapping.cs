using Griffin.Data;
using Griffin.Data.Configuration;

class InsultHistoryMapping : IEntityConfigurator<InsultHistory>
{
    public void Configure(IClassMappingConfigurator<InsultHistory> config)
    {
        config.TableName("InsultHistory");
        config.Key(x => x.Id).AutoIncrement();
        config.Property(x => x.AccountId);
        config.Property(x => x.CreatedAtUtc);
        config.Property(x => x.TargetAccountId);
    }
}

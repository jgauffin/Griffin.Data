using Griffin.Data;
using Griffin.Data.Configuration;

class AnalyzeRoosterMapping : IEntityConfigurator<AnalyzeRooster>
{
    public void Configure(IClassMappingConfigurator<AnalyzeRooster> config)
    {
        config.TableName("AnalyzeRoosters");
        config.Key(x => x.Id).AutoIncrement();
        config.Property(x => x.AccountId);
        config.Property(x => x.Scanned);
    }
}

using Griffin.Data;
using Griffin.Data.Configuration;

class MatchesToScanMapping : IEntityConfigurator<MatchesToScan>
{
    public void Configure(IClassMappingConfigurator<MatchesToScan> config)
    {
        config.TableName("MatchesToScan");
        config.Key(x => x.Id).AutoIncrement();
        config.Property(x => x.MatchId);
        config.Property(x => x.RequestedById);
        config.Property(x => x.Scanned);
    }
}

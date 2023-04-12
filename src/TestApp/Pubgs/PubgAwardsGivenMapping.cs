using Griffin.Data;
using Griffin.Data.Configuration;

namespace Pubgs.Data.Mappings
{

    class PubgAwardsGivenMapping : IEntityConfigurator<PubgAwardsGiven>
    {
        public void Configure(IClassMappingConfigurator<PubgAwardsGiven> config)
        {
            config.TableName("PubgAwardsGiven");
            config.Key(x => x.Id).AutoIncrement();
            config.Property(x => x.AccountId);
            config.Property(x => x.MatchId);
            config.Property(x => x.AwardName);
            config.Property(x => x.Title);
            config.Property(x => x.Motivation);
            config.Property(x => x.CreatedAtUtc);
        }
    }

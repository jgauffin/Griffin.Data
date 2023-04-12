using Griffin.Data;
using Griffin.Data.Configuration;

namespace Pubgs.Data.Mappings
{

    class PubgPlayerMapping : IEntityConfigurator<PubgPlayer>
    {
        public void Configure(IClassMappingConfigurator<PubgPlayer> config)
        {
            config.TableName("PubgPlayers");
            config.Key(x => x.AccountId).AutoIncrement();
            config.Property(x => x.PubgId);
            config.Property(x => x.Nickname);
            config.Property(x => x.LatestMatchDate);
        }
    }

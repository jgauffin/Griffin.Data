using Griffin.Data;
using Griffin.Data.Configuration;

namespace PubgMatches.Data.Mappings
{

    class PubgMatchRosterMapping : IEntityConfigurator<PubgMatchRoster>
    {
        public void Configure(IClassMappingConfigurator<PubgMatchRoster> config)
        {
            config.TableName("PubgMatchRoster");
            config.Key(x => x.Id).AutoIncrement();
            config.Property(x => x.Rank);
            config.Property(x => x.MatchId);
            config.HasMany(x => x.Members)
                  .ForeignKey(x => x.RosterId)
                  .References(x => x.Id);
            //config.HasOneConfigurator(x => x.PubgMatchRosterMember)
            //      .ForeignKey(x => x.RosterId)
            //      .References(x => x.Id);

        }
    }

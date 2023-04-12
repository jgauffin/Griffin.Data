using Griffin.Data;
using Griffin.Data.Configuration;

namespace PubgMatches.Data.Mappings
{

    class PubgMatchMapping : IEntityConfigurator<PubgMatch>
    {
        public void Configure(IClassMappingConfigurator<PubgMatch> config)
        {
            config.TableName("PubgMatches");
            config.Key(x => x.Id).AutoIncrement();
            config.Property(x => x.MatchId);
            config.Property(x => x.MapName);
            config.Property(x => x.CreatedAtUtc);
            config.Property(x => x.MatchType);
            config.Property(x => x.Duration);
            config.Property(x => x.GameMode);
            config.Property(x => x.IsCustomMatch);
            config.HasMany(x => x.Rosters)
                  .ForeignKey(x => x.MatchId)
                  .References(x => x.Id);
            //config.HasOneConfigurator(x => x.PubgMatchRoster)
            //      .ForeignKey(x => x.MatchId)
            //      .References(x => x.Id);

            config.HasMany(x => x.RosterMembers)
                  .ForeignKey(x => x.MatchId)
                  .References(x => x.Id);
            //config.HasOneConfigurator(x => x.PubgMatchRosterMember)
            //      .ForeignKey(x => x.MatchId)
            //      .References(x => x.Id);

            config.HasMany(x => x.PubgAwardsGivens)
                  .ForeignKey(x => x.MatchId)
                  .References(x => x.Id);
            //config.HasOneConfigurator(x => x.PubgAwardsGiven)
            //      .ForeignKey(x => x.MatchId)
            //      .References(x => x.Id);

        }
    }

using Griffin.Data;
using Griffin.Data.Configuration;

namespace PubgMatches.Data.Mappings
{

    class PubgMatchRosterMemberMapping : IEntityConfigurator<PubgMatchRosterMember>
    {
        public void Configure(IClassMappingConfigurator<PubgMatchRosterMember> config)
        {
            config.TableName("PubgMatchRosterMember");
            config.Key(x => x.Id).AutoIncrement();
            config.Property(x => x.AccountId);
            config.Property(x => x.MatchId);
            config.Property(x => x.RosterId);
            config.Property(x => x.PlayerId);
            config.Property(x => x.NickName);
            config.Property(x => x.Assists);
            config.Property(x => x.Boosts);
            config.Property(x => x.DamageDealt);
            config.Property(x => x.DeathType);
            config.Property(x => x.HeadshotKills);
            config.Property(x => x.Heals);
            config.Property(x => x.Kills);
            config.Property(x => x.KillPlace);
            config.Property(x => x.KillStreak);
            config.Property(x => x.Knocks);
            config.Property(x => x.LongestKill);
            config.Property(x => x.Revives);
            config.Property(x => x.RideDistance);
            config.Property(x => x.RoadKills);
            config.Property(x => x.SwimDistance);
            config.Property(x => x.TimeSurvived);
            config.Property(x => x.TeamKills);
            config.Property(x => x.VehicleDestroys);
            config.Property(x => x.WalkDistance);
            config.Property(x => x.WeaponsAcquired);
            config.Property(x => x.WinPlace);
        }
    }

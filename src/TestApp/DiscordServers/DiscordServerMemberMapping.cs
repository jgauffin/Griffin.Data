using Griffin.Data;
using Griffin.Data.Configuration;

namespace DiscordServers.Data.Mappings
{

    class DiscordServerMemberMapping : IEntityConfigurator<DiscordServerMember>
    {
        public void Configure(IClassMappingConfigurator<DiscordServerMember> config)
        {
            config.TableName("DiscordServerMembers");
            config.Property(x => x.AccountId);
            config.Property(x => x.ServerId);
            config.Property(x => x.IsAdmin);
        }
    }

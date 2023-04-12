using Griffin.Data;
using Griffin.Data.Configuration;

namespace DiscordServers.Data.Mappings
{

    class DiscordServerMapping : IEntityConfigurator<DiscordServer>
    {
        public void Configure(IClassMappingConfigurator<DiscordServer> config)
        {
            config.TableName("DiscordServers");
            config.Key(x => x.Id).AutoIncrement();
            config.Property(x => x.GuildId);
            config.Property(x => x.Name);
            config.HasMany(x => x.Members)
                  .ForeignKey(x => x.ServerId)
                  .References(x => x.Id);
            //config.HasOneConfigurator(x => x.DiscordServerMember)
            //      .ForeignKey(x => x.ServerId)
            //      .References(x => x.Id);

        }
    }

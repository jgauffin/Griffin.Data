using Griffin.Data;
using Griffin.Data.Configuration;

class AccountMapping : IEntityConfigurator<Account>
{
    public void Configure(IClassMappingConfigurator<Account> config)
    {
        config.TableName("Accounts");
        config.Key(x => x.Id).AutoIncrement();
        config.Property(x => x.DiscordUserId);
        config.Property(x => x.Nickname);
        config.Property(x => x.HelpAccess);
        config.HasMany(x => x.InsultHistories)
              .ForeignKey(x => x.AccountId)
              .References(x => x.Id);
        //config.HasOneConfigurator(x => x.InsultHistory)
        //      .ForeignKey(x => x.AccountId)
        //      .References(x => x.Id);

        config.HasMany(x => x.InsultHistories)
              .ForeignKey(x => x.TargetAccountId)
              .References(x => x.Id);
        //config.HasOneConfigurator(x => x.InsultHistory)
        //      .ForeignKey(x => x.TargetAccountId)
        //      .References(x => x.Id);

        config.HasMany(x => x.PubgPlayers)
              .ForeignKey(x => x.AccountId)
              .References(x => x.Id);
        //config.HasOneConfigurator(x => x.PubgPlayer)
        //      .ForeignKey(x => x.AccountId)
        //      .References(x => x.Id);

        config.HasMany(x => x.PubgMatchRosterMembers)
              .ForeignKey(x => x.AccountId)
              .References(x => x.Id);
        //config.HasOneConfigurator(x => x.PubgMatchRosterMember)
        //      .ForeignKey(x => x.AccountId)
        //      .References(x => x.Id);

        config.HasMany(x => x.PubgAwardsGivens)
              .ForeignKey(x => x.AccountId)
              .References(x => x.Id);
        //config.HasOneConfigurator(x => x.PubgAwardsGiven)
        //      .ForeignKey(x => x.AccountId)
        //      .References(x => x.Id);

        config.HasMany(x => x.DiscordServerMembers)
              .ForeignKey(x => x.AccountId)
              .References(x => x.Id);
        //config.HasOneConfigurator(x => x.DiscordServerMember)
        //      .ForeignKey(x => x.AccountId)
        //      .References(x => x.Id);

    }
}

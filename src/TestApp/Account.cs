public class Account
{
    public int Id { get; set; }
    public long DiscordUserId { get; set; }
    public string Nickname { get; set; }
    public bool HelpAccess { get; set; }
    public IReadOnlyList<InsultHistory> InsultHistories { get; private set; } = new List<InsultHistory>();
    // public InsultHistory InsultHistory { get; set; }
    public IReadOnlyList<InsultHistory> InsultHistories { get; private set; } = new List<InsultHistory>();
    // public InsultHistory InsultHistory { get; set; }
    public IReadOnlyList<PubgPlayer> PubgPlayers { get; private set; } = new List<PubgPlayer>();
    // public PubgPlayer PubgPlayer { get; set; }
    public IReadOnlyList<PubgMatchRosterMember> PubgMatchRosterMembers { get; private set; } = new List<PubgMatchRosterMember>();
    // public PubgMatchRosterMember PubgMatchRosterMember { get; set; }
    public IReadOnlyList<PubgAwardsGiven> PubgAwardsGivens { get; private set; } = new List<PubgAwardsGiven>();
    // public PubgAwardsGiven PubgAwardsGiven { get; set; }
    public IReadOnlyList<DiscordServerMember> DiscordServerMembers { get; private set; } = new List<DiscordServerMember>();
    // public DiscordServerMember DiscordServerMember { get; set; }
}

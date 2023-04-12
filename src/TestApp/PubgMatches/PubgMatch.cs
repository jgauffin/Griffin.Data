namespace PubgMatches.Domain
{
    public class PubgMatch
    {
        public int Id { get; set; }
        public string MatchId { get; set; }
        public string MapName { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public int MatchType { get; set; }
        public int Duration { get; set; }
        public int GameMode { get; set; }
        public bool IsCustomMatch { get; set; }
        public IReadOnlyList<PubgMatchRoster> Rosters { get; private set; } = new List<PubgMatchRoster>();
        // public PubgMatchRoster PubgMatchRoster { get; set; }
        public IReadOnlyList<PubgMatchRosterMember> RosterMembers { get; private set; } = new List<PubgMatchRosterMember>();
        // public PubgMatchRosterMember PubgMatchRosterMember { get; set; }
        public IReadOnlyList<PubgAwardsGiven> PubgAwardsGivens { get; private set; } = new List<PubgAwardsGiven>();
        // public PubgAwardsGiven PubgAwardsGiven { get; set; }
    }
}

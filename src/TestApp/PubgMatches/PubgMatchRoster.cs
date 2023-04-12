namespace PubgMatches.Domain
{
    public class PubgMatchRoster
    {
        public int Id { get; set; }
        public int Rank { get; set; }
        public int MatchId { get; set; }
        public IReadOnlyList<PubgMatchRosterMember> Members { get; private set; } = new List<PubgMatchRosterMember>();
        // public PubgMatchRosterMember PubgMatchRosterMember { get; set; }
    }
}

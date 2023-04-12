namespace Pubgs.Domain
{
    public class PubgAwardsGiven
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int MatchId { get; set; }
        public string AwardName { get; set; }
        public string Title { get; set; }
        public string Motivation { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}

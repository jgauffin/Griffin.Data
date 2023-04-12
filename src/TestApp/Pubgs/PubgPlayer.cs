namespace Pubgs.Domain
{
    public class PubgPlayer
    {
        public int AccountId { get; set; }
        public string PubgId { get; set; }
        public string Nickname { get; set; }
        public DateTime LatestMatchDate { get; set; }
    }
}

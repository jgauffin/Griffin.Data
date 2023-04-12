namespace PubgMatches.Domain
{
    public class PubgMatchRosterMember
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        public int MatchId { get; set; }
        public int RosterId { get; set; }
        public string PlayerId { get; set; }
        public string NickName { get; set; }
        public int Assists { get; set; }
        public int Boosts { get; set; }
        public int DamageDealt { get; set; }
        public string DeathType { get; set; }
        public int HeadshotKills { get; set; }
        public int Heals { get; set; }
        public int Kills { get; set; }
        public int KillPlace { get; set; }
        public int KillStreak { get; set; }
        public int Knocks { get; set; }
        public int LongestKill { get; set; }
        public int Revives { get; set; }
        public int RideDistance { get; set; }
        public int RoadKills { get; set; }
        public int SwimDistance { get; set; }
        public int TimeSurvived { get; set; }
        public int TeamKills { get; set; }
        public int VehicleDestroys { get; set; }
        public int WalkDistance { get; set; }
        public int WeaponsAcquired { get; set; }
        public int WinPlace { get; set; }
    }
}

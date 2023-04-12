namespace DiscordServers.Domain
{
    public class DiscordServerMember
    {
        public int AccountId { get; set; }
        public int ServerId { get; set; }
        public bool IsAdmin { get; set; }
    }
}

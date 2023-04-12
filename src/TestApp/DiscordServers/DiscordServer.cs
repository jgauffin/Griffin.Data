namespace DiscordServers.Domain
{
    public class DiscordServer
    {
        public int Id { get; set; }
        public long GuildId { get; set; }
        public string Name { get; set; }
        public IReadOnlyList<DiscordServerMember> Members { get; private set; } = new List<DiscordServerMember>();
        // public DiscordServerMember DiscordServerMember { get; set; }
    }
}

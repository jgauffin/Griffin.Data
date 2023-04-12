public class InsultHistory
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public DateTime CreatedAtUtc { get; set; }
    public int TargetAccountId { get; set; }
}

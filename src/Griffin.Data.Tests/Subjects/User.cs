namespace Griffin.Data.Tests.Subjects;

public class User
{
    public IReadOnlyList<Address> Addresses { get; } = new List<Address>();
    public DateTime CreatedAt { get; set; }
    public Data? Data { get; set; }
    public string FirstName { get; set; } = "";
    public int Id { get; set; }
    public AccountState State { get; set; } = AccountState.Disabled;
    public AccountState? NullableState { get; set; } = AccountState.Disabled;

    public ExplicitState? NullableExplicit { get; set; } = ExplicitState.Admin;

    public IReadOnlyList<string> Titles { get; } = new List<string>();
}

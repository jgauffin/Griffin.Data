namespace Griffin.Data.Tests.Subjects;

public class User
{
    public IReadOnlyList<Address> Addresses { get; } = new List<Address>();

    public Data? Data { get; set; }
    public string FirstName { get; set; } = "";
    public int Id { get; set; }
    public AccountState State { get; set; } = AccountState.Disabled;
    public IReadOnlyList<string> Titles { get; } = new List<string>();
}

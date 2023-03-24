namespace Griffin.Data.Tests.Configuration.Subjects;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public AccountState State { get; set; }
    public IReadOnlyList<string> Titles => new List<string>();
    public IReadOnlyList<Address> Addresses => new List<Address>();

    public Data Data { get; set; }
}
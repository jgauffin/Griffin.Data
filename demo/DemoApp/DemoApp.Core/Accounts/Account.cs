namespace DemoApp.Core.Accounts;

public class Account
{
    public Account(string userName, string password, string salt)
    {
        UserName = userName;
        Password = password;
        Salt = salt;
    }

    public int Id { get; private set; }
    public string Password { get; }
    public string Salt { get; }
    public string UserName { get; }
}

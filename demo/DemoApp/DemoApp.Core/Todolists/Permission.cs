namespace DemoApp.Core.Todolists;

public class Permission
{
    public Permission(
        int todolistId,
        int accountId,
        bool canRead,
        bool canWrite,
        bool isAdmin)
    {
        TodolistId = todolistId;
        AccountId = accountId;
        CanRead = canRead;
        CanWrite = canWrite;
        IsAdmin = isAdmin;
    }

    public int AccountId { get; }
    public bool CanRead { get; }
    public bool CanWrite { get; }

    public int Id { get; private set; }
    public bool IsAdmin { get; }
    public int TodolistId { get; }
}

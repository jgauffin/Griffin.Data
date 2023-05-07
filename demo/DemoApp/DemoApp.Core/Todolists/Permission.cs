
namespace DemoApp.Core.Todolists
{
    public class Permission
    {
        public Permission(int todolistId, int accountId, bool canRead, bool canWrite, bool isAdmin)
        {
            TodolistId = todolistId;
            AccountId = accountId;
            CanRead = canRead;
            CanWrite = canWrite;
            IsAdmin = isAdmin;
        }

        public int Id { get; private set; }
        public int TodolistId { get; private set; }
        public int AccountId { get; private set; }
        public bool CanRead { get; private set; }
        public bool CanWrite { get; private set; }
        public bool IsAdmin { get; private set; }

    }
}

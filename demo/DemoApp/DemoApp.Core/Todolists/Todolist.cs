
using DemoApp.Core.TodoTasks;

namespace DemoApp.Core.Todolists
{
    public class Todolist
    {
        private readonly List<Permission> _permissions = new();
        private readonly List<TodoTask> _todoTasks = new();

        public Todolist(int createdById, DateTime createdAtUtc)
        {
            CreatedById = createdById;
            CreatedAtUtc = createdAtUtc;
        }

        public int Id { get; private set; }
        public int Name { get; private set; }
        public int CreatedById { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public int? UpdatedById { get; set; }
        public DateTime? UpdatedAtUtc { get; set; }

        public IReadOnlyList<Permission> Permissions => _permissions;
        // public Permission Permission { get; set; }

        public IReadOnlyList<TodoTask> TodoTasks => _todoTasks;
        // public TodoTask TodoTask { get; set; }

    }
}

using DemoApp.Core.TodoTasks;

namespace DemoApp.Core.Todolists;

public class Todolist
{
    private readonly List<Permission> _permissions = new();
    private readonly List<TodoTask> _todoTasks = new();

    public Todolist(string name, int createdById)
    {
        Name = name;
        CreatedById = createdById;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public DateTime CreatedAtUtc { get; }
    public int CreatedById { get; }

    public int Id { get; private set; }
    public string Name { get; }

    public IReadOnlyList<Permission> Permissions => _permissions;
    // public Permission Permission { get; set; }

    public IReadOnlyList<TodoTask> TodoTasks => _todoTasks;
    public DateTime UpdatedAtUtc { get; set; }

    public int UpdatedById { get; set; }
    // public TodoTask TodoTask { get; set; }
}

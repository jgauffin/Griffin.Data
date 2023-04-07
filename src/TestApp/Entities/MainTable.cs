namespace TestApp.Entities;

public class MainTable
{
    private static int LogId = 1;
    private readonly List<Log> _logs = new();

    public short Age { get; set; }

    public IReadOnlyList<ChildTable> Children { get; set; } = new List<ChildTable>();

    public int Id { get; set; }

    public IReadOnlyList<Log> Logs => _logs;

    public long Money { get; set; }

    public string Name { get; set; } = "";

    public bool Rocks { get; set; }

    public void AddLog(string msg)
    {
        _logs.Add(new Log(msg) { Id = LogId++ });
    }
}

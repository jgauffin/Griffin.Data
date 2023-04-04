namespace Griffin.Data.Tests.Entities;

public class MainTable
{
    private readonly List<Log> _logs = new();

    public int Id { get; set; }

    public string Name { get; set; } = "";

    public short Age { get; set; }

    public long Money { get; set; }

    public bool Rocks { get; set; }

    public IReadOnlyList<ChildTable> Children { get; set; } = new List<ChildTable>();
   

    public IReadOnlyList<Log> Logs => _logs;

    public void AddLog(string msg)
    {
        _logs.Add(new Log(msg));
    }
}
namespace Griffin.Data.Tests.Entitites;

public class MainTable
{
    private readonly List<Log> _logs = new();

    public int Id { get; set; }

    public string Name { get; set; }

    public short Age { get; set; }

    public long Money { get; set; }

    public bool Rocks { get; set; }

    public MainActionType ActionType { get; set; }

    public ExtraAction ExtraAction { get; set; }

    public IReadOnlyList<Log> Logs => _logs;

    public void AddLog(string msg)
    {
        _logs.Add(new Log(msg));
    }
}
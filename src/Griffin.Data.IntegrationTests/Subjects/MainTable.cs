namespace Griffin.Data.Tests.Entities;

public class MainTable
{
    private static int _idSequence = 1;
    private readonly List<Log> _logs = new();
    private List<ChildTable> _children = new();
    private int _id1;

    public short Age { get; set; }

    public IReadOnlyList<ChildTable> Children
    {
        get => _children;
        set => _children = new List<ChildTable>(value);
    }

    public int Id
    {
        get
        {
            if (_id1 == 0)
            {
                _id1 = _idSequence++;
            }

            return _id1;
        }
        set => _id1 = value;
    }

    public IReadOnlyList<Log> Logs => _logs;

    public long Money { get; set; }

    public string Name { get; set; } = "";

    public bool Rocks { get; set; }

    public void AddChild(ChildTable childTable)
    {
        _children.Add(childTable);
    }

    public void AddLog(string msg)
    {
        _logs.Add(new Log(msg));
    }

    public void ClearChildren()
    {
        _children.Clear();
    }

    public void RemoveChild(ChildTable child)
    {
        _children.Remove(child);
    }
}

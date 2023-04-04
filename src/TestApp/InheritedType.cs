namespace TestApp;

public class InheritedType : SomeType
{
    private readonly IChangeTracker _tracker;
    private int _field;
    private string _someString = "";

    public InheritedType(int field, IChangeTracker tracker) : base(field)
    {
        _tracker = tracker;
    }

    public override int Field
    {
        get => _field;
        set
        {
            _field = value;
            _tracker.Changed(this);
        }
    }

    public override string SomeString
    {
        get => _someString;
        set
        {
            _someString = value;
            _tracker.Changed(this);
        }
    }
}
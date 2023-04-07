namespace TestApp;

public class SomeType
{
    public SomeType(int field)
    {
        // ReSharper disable once VirtualMemberCallInConstructor
        Field = field;
    }

    public virtual int Field { get; set; }
    public virtual string SomeString { get; set; } = "";
}

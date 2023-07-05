namespace Griffin.Data.Tests.Subjects;

public class ClassWithConstructor
{
    public ClassWithConstructor(int id, string name)
    {
        Id = id;
        Name = name ?? throw new ArgumentNullException(nameof(name));
    }

    public int Id { get; }
    public string Name { get; }
}

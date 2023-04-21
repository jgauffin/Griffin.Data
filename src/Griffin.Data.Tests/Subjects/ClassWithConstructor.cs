namespace Griffin.Data.Tests.Subjects;

public class ClassWithConstructor
{
    public ClassWithConstructor(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; }
    public string Name { get; }
}

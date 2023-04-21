namespace Griffin.Data.Tests.Subjects;

public class ClassWithConstructorAndConverter
{
    public ClassWithConstructorAndConverter(int id, string name)
    {
        Id = id;
        Name = name;
    }

    public int Id { get; }
    public string Name { get; }
}

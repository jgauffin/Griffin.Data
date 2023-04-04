using Griffin.Data.ChangeTracking;

public class Diff : IDiff
{
    public void Added(object entity, int depth)
    {
        Console.WriteLine("Added " + entity);
    }

    public void Modified(object entity, int depth)
    {
        Console.WriteLine("Modified " + entity);
    }

    public void Removed(object entity, int depth)
    {
        Console.WriteLine("Removed " + entity);
    }
}
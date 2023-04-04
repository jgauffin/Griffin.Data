namespace Griffin.Data.ChangeTracking;

public interface IDiff
{
    void Added(object entity, int depth);
    void Modified(object entity, int depth);
    void Removed(object entity, int depth);

}
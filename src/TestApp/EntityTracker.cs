using Griffin.Data;
using Griffin.Data.ChangeTracking;
using Griffin.Data.Mappings;

public class EntityTracker : IEntityTracker
{
    private List<object> _loadedItems = new List<object>();
    private List<object> _snapshot = new List<object>();
    private IMappingRegistry _registry;
    private ICopyService _copyService = new CopyService();

    public void ApplyChanges()
    {
        var diff = new Diff();
        var compareService = new CompareService(_registry, diff);
        foreach (var VARIABLE in _loadedItems)
        {
            
        }
    }

    public void Loaded(object entity)
    {

    }

    public void Track(object item)
    {
        _loadedItems.Add(item);
        var copy = _copyService.Copy(item);
        _snapshot.Add(item);
    }
}
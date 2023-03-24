namespace Griffin.Data.Mappings.Properties;

public interface IPropertyAccessor
{
    void SetColumnValue(object instance, object value);
    object? GetColumnValue(object entity);
}
namespace Griffin.Data.Mappings.Properties;

public interface IFieldMapping : IPropertyAccessor
{
    string ColumnName { get; }
    string PropertyName { get; }
}
Conversions
===========

Conversions are made when the column and property types are incompatible.

Conversions are done on a property level.

Minimal example:

```csharp
internal class UserMapping : IEntityConfigurator<User>
{
    public void Configure(IClassMappingConfigurator<User> config)
    {
        config.Key(x => x.Id).AutoIncrement();

        // Will convert local time to UTC 
        // when persisting and vice versa.
        config.Property(x => x.CreatedAt)
            .Converter(new UtcToLocal());

        config.MapRemainingProperties();
    }
}
```

There are also built-in conversions that can be added through extension methods. For instance, the library automatically handles enum to int but supports other formats. Below, enums are converted to strings.

```csharp
internal class UserMapping : IEntityConfigurator<User>
{
    public void Configure(IClassMappingConfigurator<User> config)
    {
        config.Key(x => x.Id).AutoIncrement();

        // Convert enum to a string (varchar).
        config.Property(x => x.State).StringEnum();

        config.MapRemainingProperties();
    }
}
```


All built-in conversions are found in the [code](https://github.com/jgauffin/Griffin.Data/tree/master/src/Griffin.Data/Converters). Feel free to contribute through a pull request.

## Custom converters

You can create your own converters by implementing `ISingleValueConverter<TColumn, TProperty>`.

Let's say that dates should be stored as strings in the database.

```csharp
public class DatesToStrings : ISingleValueConverter<string, DateTime>
{
    public DateTime ColumnToProperty(string value)
    {
        return DateTime.Parse(value, CultureInfo.InvariantCulture);
    }

    public string PropertyToColumn(DateTime value)
    {
        return value.ToString(CultureInfo.InvariantCulture);
    }
}
```

Then add it to your mapping:

```csharp
internal class UserMapping : IEntityConfigurator<User>
{
    public void Configure(IClassMappingConfigurator<User> config)
    {
        config.Key(x => x.Id).AutoIncrement();

        config.Property(x => x.CreatedAt).Converter(new DatesToStrings());

        config.MapRemainingProperties();
    }
}
```

An extension method can be created for it:

```csharp
public static class ConverterExtensions
{
    public static PropertyConfigurator<TEntity, DateTime> DateToString<TEntity, DateTime>(
        this PropertyConfigurator<TEntity, DateTime> prop)
    {
        prop.Converter(new DatesToStrings());
        return prop;
    }
}


internal class UserMapping : IEntityConfigurator<User>
{
    public void Configure(IClassMappingConfigurator<User> config)
    {
        config.Key(x => x.Id).AutoIncrement();

        config.Property(x => x.CreatedAt).DateToString();

        config.MapRemainingProperties();
    }
}
```


## Multi column converters

You can store more complex structures in multiple columns and convert them into custom classes.

You might have children that vary and do not want to create a table for every possible child type.

In that case, you can store them in a column as JSON and then use a converter to serialize/deserialize them.

Multi-column converters must implement the interface `IRecordToValueConverter<TPropertyType>`. It uses ADO.NETs `IDataRecord`, which gives you direct access to a row in the table.

Here is the JSON example:

```csharp
public class JsonConverter<T> : IRecordToValueConverter<T>
{
    private readonly string _dataTypeColumn;
    private readonly string _jsonColumn;

    public JsonConverter(string dataTypeColumn, string jsonColumn)
    {
        _dataTypeColumn = dataTypeColumn;
        _jsonColumn = jsonColumn;
    }

    public T? Convert(IDataRecord dataRecord)
    {
        var typeStr = dataRecord[_dataTypeColumn];
        if (typeStr is DBNull)
        {
            return default;
        }

        var type = Type.GetType((string)typeStr);
        if (type == null)
        {
            return default;
        }

        var json = dataRecord[_jsonColumn];
        if (json is DBNull)
        {
            return default;
        }

        return JsonConvert.DeserializeObject((string)json, type);
    }


    public void ConvertToColumns(T? entity, IDictionary<string, object> columns)
    {
        if (entity == null)
        {
            return;
        }

        columns[_dataTypeColumn] = entity.GetType().AssemblyQualifiedName;
        columns[_jsonColumn] = JsonConverter.SerializeObject(entity);
    }
}
```

It can be registered for a property as:

```csharp
{
    public void Configure(IClassMappingConfigurator<User> config)
    {
        config.Key(x => x.Id).AutoIncrement();

        config.Property(x => x.SomeChild).Converter(new JsonConverter<ChildBase>("JsonType", "Json"));

        config.MapRemainingProperties();
    }
}
```

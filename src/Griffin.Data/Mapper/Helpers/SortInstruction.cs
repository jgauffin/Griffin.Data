namespace Griffin.Data.Mapper.Helpers;

internal class SortInstruction
{
    public SortInstruction(string name, bool isAscending, bool isPropertyName)
    {
        Name = name;
        IsAscending = isAscending;
        IsPropertyName = isPropertyName;
    }

    public string Name { get; set; }
    public bool IsAscending { get; set; }
    public bool IsPropertyName { get; }
}
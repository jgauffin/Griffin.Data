namespace Griffin.Data.Scaffolding.Queries.Parser;

public enum ParameterState
{
    Declare,
    Name,
    DataType,
    Equal,
    TestValue,
    Complete
}

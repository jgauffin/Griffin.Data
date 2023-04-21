namespace Griffin.Data.Scaffolding.Queries.Parser;

public class QueryParameter
{
    public QueryParameter(string name, string sqlType, string testValue)
    {
        Name = name;
        SqlType = sqlType;
        TestValue = testValue;
    }

    public string Name { get; set; }
    public string SqlType { get; set; }
    public string TestValue { get; set; }
}

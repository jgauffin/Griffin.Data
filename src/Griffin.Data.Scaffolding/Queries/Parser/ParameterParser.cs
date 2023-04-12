namespace Griffin.Data.Scaffolding.Queries.Parser;

/// <summary>
///     Parses parameters in a SQL query script.
/// </summary>
internal class ParameterParser
{
    private readonly Dictionary<ParameterState, ParserMethod> _methods = new();
    private string _dataType = "";
    private string _name = "";
    private ParameterState _state = ParameterState.Declare;
    private string _testValue = "";

    public ParameterParser()
    {
        _methods[ParameterState.Declare] = ParseDeclare;
        _methods[ParameterState.Name] = ParseName;
        _methods[ParameterState.DataType] = ParseSqlType;
        _methods[ParameterState.Equal] = ParseEqual;
        _methods[ParameterState.TestValue] = ParseTestValue;
    }

    public QueryParameter ParseParameter(string line)
    {
        var pos = 0;
        while (_state != ParameterState.Complete)
        {
            _methods[_state](line, ref pos);
            if (pos < line.Length)
            {
                SkipWhiteSpace(line, ref pos);
            }
        }

        return new QueryParameter(_name, _dataType, _testValue);
    }

    private static void EnsureNotTheEnd(string line, int pos)
    {
        if (pos == line.Length)
        {
            throw new InvalidOperationException("Unexpected end of line while parsing parameter name: " + line);
        }
    }

    private void ParseDeclare(string line, ref int pos)
    {
        while (pos < line.Length && !char.IsWhiteSpace(line[pos]))
        {
            pos++;
        }

        EnsureNotTheEnd(line, pos);

        _state = ParameterState.Name;
    }

    private void ParseEqual(string line, ref int pos)
    {
        while (pos < line.Length && line[pos] != '=')
        {
            pos++;
        }

        pos++;

        EnsureNotTheEnd(line, pos);
        _state = ParameterState.TestValue;
    }

    private void ParseName(string line, ref int pos)
    {
        var start = pos;
        while (pos < line.Length && !char.IsWhiteSpace(line[pos]))
        {
            pos++;
        }

        EnsureNotTheEnd(line, pos);

        _name = line.Substring(start, pos - start);

        // remove '@' or similar.
        if (!char.IsLetterOrDigit(_name[0]))
        {
            _name = _name[1..];
        }

        _state = ParameterState.DataType;
    }

    private void ParseSqlType(string line, ref int pos)
    {
        var start = pos;
        for (; pos < line.Length; pos++)
        {
            var ch = line[pos];
            if (char.IsWhiteSpace(ch) || ch is ';' or '=')
            {
                break;
            }
        }

        var end = pos;

        SkipWhiteSpace(line, ref pos);
        if (line[pos] == '=')
        {
            _dataType = line.Substring(start, end - start);
            _state = ParameterState.Equal;
            return;
        }

        _state = ParameterState.Complete;
    }

    private void ParseTestValue(string line, ref int pos)
    {
        var start = pos;
        while (pos < line.Length && !char.IsWhiteSpace(line[pos]))
        {
            pos++;
        }

        _testValue = line[start..].TrimEnd().TrimEnd(';');
        _state = ParameterState.Complete;
    }

    private void SkipWhiteSpace(string line, ref int pos)
    {
        while (char.IsWhiteSpace(line[pos]) && pos < line.Length)
        {
            pos++;
        }
    }

    private delegate void ParserMethod(string line, ref int pos);
}

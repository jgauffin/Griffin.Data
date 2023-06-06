using System.Data;

namespace Griffin.Data.Tests.Helpers;

public class FakeRecord : IDataRecord
{
    private readonly IDictionary<string, object> _dict;
    private readonly string[] _names;
    private readonly object[] _values;
    public FakeRecord(IDictionary<string, object> dict)
    {
        _dict = dict;
        var index = 0;
        _names = new string[dict.Count];
        _values = new object[dict.Count];
        foreach (var kvp in dict)
        {
            _names[index] = kvp.Key;
            _values[index++] = kvp.Value;
        }
    }

    public bool GetBoolean(int i)
    {
        throw new NotImplementedException();
    }

    public byte GetByte(int i)
    {
        throw new NotImplementedException();
    }

    public long GetBytes(
        int i,
        long fieldOffset,
        byte[]? buffer,
        int bufferoffset,
        int length)
    {
        throw new NotImplementedException();
    }

    public char GetChar(int i)
    {
        throw new NotImplementedException();
    }

    public long GetChars(
        int i,
        long fieldoffset,
        char[]? buffer,
        int bufferoffset,
        int length)
    {
        throw new NotImplementedException();
    }

    public IDataReader GetData(int i)
    {
        throw new NotImplementedException();
    }

    public string GetDataTypeName(int i)
    {
        throw new NotImplementedException();
    }

    public DateTime GetDateTime(int i)
    {
        throw new NotImplementedException();
    }

    public decimal GetDecimal(int i)
    {
        throw new NotImplementedException();
    }

    public double GetDouble(int i)
    {
        throw new NotImplementedException();
    }

    public Type GetFieldType(int i)
    {
        throw new NotImplementedException();
    }

    public float GetFloat(int i)
    {
        throw new NotImplementedException();
    }

    public Guid GetGuid(int i)
    {
        throw new NotImplementedException();
    }

    public short GetInt16(int i)
    {
        throw new NotImplementedException();
    }

    public int GetInt32(int i)
    {
        throw new NotImplementedException();
    }

    public long GetInt64(int i)
    {
        throw new NotImplementedException();
    }

    public string GetName(int i)
    {
        return _names[i];
    }

    public int GetOrdinal(string name)
    {
        throw new NotImplementedException();
    }

    public string GetString(int i)
    {
        throw new NotImplementedException();
    }

    public object GetValue(int i)
    {
        return _values[i];
    }

    public int GetValues(object[] values)
    {
        throw new NotImplementedException();
    }

    public bool IsDBNull(int i)
    {
        throw new NotImplementedException();
    }

    public int FieldCount => _dict.Count;

    public object this[int i] => throw new NotImplementedException();

    public object this[string name] => _dict[name];
}

using System.Data.Common;
using System.Text.RegularExpressions;

namespace Griffin.Data.Meta;

internal abstract class SchemaReader
{
    private static readonly Regex rxCleanUp = new(@"[^\w\d_]", RegexOptions.Compiled);

    public abstract TableCollection ReadSchema(DbConnection connection, DbProviderFactory factory);

    public void WriteLine(string o)
    {
    }

    protected virtual string CleanUp(string name)
    {
        var str = rxCleanUp.Replace(name, "_");
        if (char.IsDigit(str[0]))
        {
            str = "_" + str;
        }

        return str;
    }

    private string CheckNullable(Column col)
    {
        var result = "";
        if (col.IsNullable &&
            col.PropertyType != "byte[]" &&
            col.PropertyType != "string" &&
            col.PropertyType != "Microsoft.SqlServer.Types.SqlGeography" &&
            col.PropertyType != "Microsoft.SqlServer.Types.SqlGeometry"
           )
        {
            result = "?";
        }

        return result;
    }
}

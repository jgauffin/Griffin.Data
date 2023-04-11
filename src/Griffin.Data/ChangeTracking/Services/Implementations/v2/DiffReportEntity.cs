using System.Collections.Generic;
using Griffin.Data.Scaffolding.Helpers;

namespace Griffin.Data.ChangeTracking.Services.Implementations.v2;

public class DiffReportEntity
{
    public ChangeState State { get; set; }
    public object Entity { get; set; }
    public List<DiffReportEntity> Children { get; set; } = new List<DiffReportEntity>();
    internal string? Key { get; set; }
    public string ToText()
    {
        var sb = new TabbedStringBuilder();
        ToText(sb);
        return sb.ToString();
    }

    private void ToText(TabbedStringBuilder sb)
    {
        sb.AppendLine(State.ToString().PadRight(11) + Entity);
        sb.Indent();
        foreach (var child in Children)
        {
            child.ToText(sb);
        }
        sb.Dedent();
    }
}

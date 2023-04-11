using System;
using System.Collections.Generic;
using System.Diagnostics;
using Griffin.Data.Scaffolding.Helpers;

namespace Griffin.Data.ChangeTracking.Services.Implementations.v2;

public class CompareResultItem
{
    private readonly List<CompareResultItem> _children = new();

    public CompareResultItem(TrackedEntity2 trackedItem, ChangeState state)
    {
        TrackedItem = trackedItem;
        State = state;
    }

    public IReadOnlyList<CompareResultItem> Children => _children;
    public int Depth => TrackedItem.Depth;
    public ChangeState State { get; set; }

    public TrackedEntity2 TrackedItem { get; set; }

    public void AppendChild(CompareResultItem child)
    {
        _children.Add(child);
    }

    public string GenerateReport(Func<object, string>? objectFormatter = null)
    {
        var sb = new TabbedStringBuilder();
        GenerateReport(sb, new List<CompareResultItem>(), objectFormatter);
        return sb.ToString();
    }

    public override string ToString()
    {
        var title = TrackedItem.Key != null
            ? TrackedItem.Key.PadRight(20)
            : $"NEW {TrackedItem.Entity,-20}";
        return $"{State,-10} {title} Depth: {TrackedItem.Depth}";
    }

    protected void GenerateReport(TabbedStringBuilder sb, List<CompareResultItem> visited, Func<object, string>? objectFormatter = null)
    {
        if (visited.Contains(this))
        {
            return;
        }

        visited.Add(this);

        var objStr = objectFormatter?.Invoke(TrackedItem.Entity) ?? TrackedItem.Entity;
        sb.AppendLine($"{State,-10} {objStr}");
        sb.Indent();
        foreach (var child in Children)
        {
            child.GenerateReport(sb, visited, objectFormatter);
        }

        sb.Dedent();
    }
}

using System;
using System.Collections.Generic;
using Griffin.Data.Helpers;

namespace Griffin.Data.ChangeTracking.Services.Implementations.v2;

/// <summary>
///     Result for <see cref="SingleEntityComparer" />.
/// </summary>
public class CompareResultItem
{
    private readonly List<CompareResultItem> _children = new();

    /// <summary>
    /// </summary>
    /// <param name="trackedItem">Item that was analyzed.</param>
    /// <param name="state">State of the item.</param>
    /// <exception cref="ArgumentNullException">Item is null.</exception>
    public CompareResultItem(CompareResultItem? parent, TrackedEntity2 trackedItem, ChangeState state)
    {
        Parent = parent;
        TrackedItem = trackedItem ?? throw new ArgumentNullException(nameof(trackedItem));
        State = state;
    }

    /// <summary>
    ///     All children for this item.
    /// </summary>
    public IReadOnlyList<CompareResultItem> Children => _children;

    /// <summary>
    ///     Current depth (1 = root entity).
    /// </summary>
    public int Depth => TrackedItem.Depth;

    /// <summary>
    ///     State of the entity.
    /// </summary>
    public ChangeState State { get; set; }

    /// <summary>
    /// Parent result item (if any).
    /// </summary>
    public CompareResultItem? Parent { get; }

    /// <summary>
    ///     Item being tracked (i.e. the entity).
    /// </summary>
    public TrackedEntity2 TrackedItem { get; set; }

    /// <summary>
    ///     Append a new child
    /// </summary>
    /// <param name="child">child entity (should have a FK that points on this entity).</param>
    public void AppendChild(CompareResultItem child)
    {
        if (child == null)
        {
            throw new ArgumentNullException(nameof(child));
        }

        _children.Add(child);
    }

    /// <summary>
    ///     Generate a report with all changes.
    /// </summary>
    /// <param name="objectFormatter">
    ///     Custom formatter which will be invoked for every entity (for instance if you want to
    ///     print each entity as JSON or extract relevant properties).
    /// </param>
    /// <returns>Generated report with a hierarchical state of every entity.</returns>
    public string GenerateReport(Func<object, string>? objectFormatter = null)
    {
        var sb = new TabbedStringBuilder();
        GenerateReport(sb, new List<CompareResultItem>(), objectFormatter);
        return sb.ToString();
    }

    /// <inheritdoc />
    public override string ToString()
    {
        var title = TrackedItem.Key != null
            ? TrackedItem.Key.PadRight(20)
            : $"NEW {TrackedItem.Entity,-20}";
        return $"{State,-10} {title} Depth: {TrackedItem.Depth}";
    }

    /// <summary>
    ///     Recursive function to generate the report for every entity in the hierarchy.
    /// </summary>
    /// <param name="sb">String builder to append to.</param>
    /// <param name="visited">Keep track of all entities that have been called already (to prevent endless loops).</param>
    /// <param name="objectFormatter">
    ///     Custom formatter which will be invoked for every entity (for instance if you want to
    ///     print each entity as JSON or extract relevant properties).
    /// </param>
    protected void GenerateReport(
        TabbedStringBuilder sb,
        List<CompareResultItem> visited,
        Func<object, string>? objectFormatter = null)
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

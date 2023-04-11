using System;
using System.Net.WebSockets;
using System.Text;
using Griffin.Data.Scaffolding.Helpers;

namespace Griffin.Data.ChangeTracking.Services.Implementations.v2;

public class CompareResultItem
{
    public CompareResultItem(TrackedEntity2 trackedItem, ChangeState state)
    {
        TrackedItem = trackedItem;
        State = state;
    }

    public TrackedEntity2 TrackedItem { get; set; }
    public int Depth => TrackedItem.Depth;
    public ChangeState State { get; set; }

    public override string ToString()
    {
        return $"{State,-10} {TrackedItem.Key,-20} Depth: {TrackedItem.Depth}";
    }
}

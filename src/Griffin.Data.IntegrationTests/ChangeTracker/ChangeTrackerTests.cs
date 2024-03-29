﻿using Griffin.Data.ChangeTracking;
using Griffin.Data.Mapper;
using Griffin.Data.Tests.Entities;

namespace Griffin.Data.IntegrationTests.ChangeTracker;

public class ChangeTrackerTests : IntegrationTests
{
    [Fact]
    public async Task Should_be_able_to_swap_child()
    {
        var main = await Session.GetById<MainTable>(1);
        main.Children[0].ActionType = ActionType.Extra;
        main.Children[0].Action = new ExtraAction { Extra = "Yo" };

        await Session.ApplyChangeTracking();

        var result = await Session.GetById<MainTable>(main.Id);
        result.Children[0].Action.As<ExtraAction>().Extra.Should().Be("Yo");
    }

    [Fact]
    public void Should_detect_added_child()
    {
        var entity = new MainTable { Age = 10 };
        var sut = new SnapshotChangeTracking(Registry);
        sut.Track(entity);

        entity.AddLog("Hello");

        sut.Refresh(entity);
        var actual = sut.GetState(entity.Logs[0]);
        actual.Should().Be(ChangeState.Added);
    }

    [Fact]
    public void Should_detect_main_entity_change()
    {
        var entity = new MainTable { Age = 10 };
        var sut = new SnapshotChangeTracking(Registry);
        sut.Track(entity);

        entity.Age = 13;

        var actual = sut.GetState(entity);
        actual.Should().Be(ChangeState.Modified);
    }

    [Fact]
    public void Should_detect_modified_child_without_refresh()
    {
        var entity = new MainTable { Age = 10 };
        var sut = new SnapshotChangeTracking(Registry);
        var child = new ChildTable { Id = 3, ActionType = ActionType.Extra };
        entity.AddChild(child);
        sut.Track(entity);

        child.ActionType = ActionType.Disabled;

        var actual = sut.GetState(child);
        actual.Should().Be(ChangeState.Modified);
    }

    [Fact]
    public void Should_detect_removed_child()
    {
        var entity = new MainTable { Age = 10 };
        var sut = new SnapshotChangeTracking(Registry);
        var child = new ChildTable { Id = 3 };
        entity.AddChild(child);
        sut.Track(entity);

        entity.ClearChildren();

        sut.Refresh(entity);
        var actual = sut.GetState(child);
        actual.Should().Be(ChangeState.Removed);
    }

    [Fact]
    public async Task Should_persist_changes()
    {
        var main = await Session.GetById<MainTable>(1);
        main.RemoveChild(main.Children[0]);
        main.AddLog("Hejsan");
        main.Children[0].ActionType = ActionType.Disabled;
        main.Children[0].Action = null!;

        await Session.ApplyChangeTracking();

        var result = await Session.GetById<MainTable>(main.Id);
        result.Logs[1].Message.Should().Be("Hejsan");
        result.Children[0].Action.Should().BeNull();
    }

    //TODO: Should be able to switch child (=> delete/insert).
}

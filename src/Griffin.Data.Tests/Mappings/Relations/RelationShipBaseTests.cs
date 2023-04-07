using FluentAssertions;
using Griffin.Data.Mapper;
using Griffin.Data.Mappings.Properties;
using Griffin.Data.Mappings.Relations;
using Griffin.Data.Tests.Subjects;

namespace Griffin.Data.Tests.Mappings.Relations;

public class RelationShipBaseTests
{
    private readonly MyTests _mapping;

    public RelationShipBaseTests()
    {
        var referencedProperty = new PropertyMapping(typeof(User), typeof(int), GetId, SetId);
        var fkProperty = new PropertyMapping(typeof(Address), typeof(int), GetUserId, SetUserId);
        var fk = new ForeignKeyMapping<User, Address>("user_id", fkProperty, referencedProperty);
        _mapping = new MyTests(fk, typeof(Address));
    }

    [Fact]
    public void Should_assign_value_to_FK()
    {
        var entity = new Address();

        _mapping.SetForeignKey(entity, 4);

        entity.UserId.Should().Be(4);
    }

    [Fact]
    public void Should_extract_FK_reference_from_parents()
    {
        var parents = new List<User> { new() { Id = 3 }, new() { Id = 4 } };

        var result = _mapping.CreateDbConstraints(parents);

        var actual = result["user_id"].As<IReadOnlyList<object>>();
        actual[0].Should().Be(3);
        actual[1].Should().Be(4);
    }

    [Fact]
    public void Should_extract_single_FK_reference_from_parents_as_a_single_value()
    {
        var parents = new List<User> { new() { Id = 3 } };

        var result = _mapping.CreateDbConstraints(parents);

        var actual = result["user_id"];
        actual.Should().Be(3);
    }

    [Fact]
    public void Should_get_value_from_FK()
    {
        var entity = new Address { UserId = 5 };

        var actual = _mapping.GetForeignKeyValue(entity);

        actual.Should().Be(5);
    }

    [Fact]
    public void Should_say_that_it_has_a_FK_property_when_configured()
    {
        _mapping.HasForeignKeyProperty.Should().BeTrue();
    }

    [Fact]
    public void Should_use_column_when_FK_property_is_not_specified()
    {
        var referencedProperty =
            new PropertyMapping(typeof(User), typeof(IReadOnlyList<Address>), GetAddress, SetAddress);
        var fk = new ForeignKeyMapping<User, Address>("user_id", null, referencedProperty);
        var mapping = new MyTests(fk, typeof(Address));

        var sut = mapping;

        sut.HasForeignKeyProperty.Should().BeFalse();
        sut.ForeignKeyColumnName.Should().Be("user_id");
        var actual1 = () => sut.GetForeignKeyValue("null");
        var actual2 = () => sut.SetForeignKey("", "");
        actual1.Should().Throw<MappingException>();
        actual2.Should().Throw<MappingException>();
    }

    private object GetAddress(object arg)
    {
        var user = (User)arg;
        return user.GetType().GetProperty("Addresses")!.GetValue(arg)!;
    }

    private object GetId(object arg)
    {
        return ((User)arg).Id;
    }

    private object GetUserId(object arg)
    {
        return ((Address)arg).UserId;
    }

    private void SetAddress(object arg1, object arg2)
    {
        var user = (User)arg1;
        user.GetType().GetProperty("Addresses")!.SetValue(arg1, arg2);
    }

    private void SetId(object arg1, object arg2)
    {
        ((User)arg1).Id = (int)arg2;
    }

    private void SetUserId(object arg1, object arg2)
    {
        ((Address)arg1).UserId = (int)arg2;
    }
}

public class MyTests : RelationShipBase<User, Address>
{
    public MyTests(ForeignKeyMapping<User, Address> fk, Type? childEntityType) : base(fk, childEntityType)
    {
    }
}

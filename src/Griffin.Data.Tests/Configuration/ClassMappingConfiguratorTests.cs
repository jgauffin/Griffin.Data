using FluentAssertions;
using Griffin.Data.Configuration;
using Griffin.Data.Tests.Configuration.Subjects;

namespace Griffin.Data.Tests.Configuration;

public class ClassMappingConfiguratorTests
{
    [Fact]
    public void Should_create_mapping()
    {
        var sut = new ClassMappingConfigurator<User>();
        sut.Property(x => x.FirstName);
        var entity = new User();

        var mapping = sut.BuildMapping();

        mapping.Properties[0].PropertyName.Should().Be("FirstName");
        mapping.Properties[0].ColumnName.Should().Be("FirstName");
        mapping.Properties[0].PropertyType.Should().Be(typeof(string));
        mapping.Properties[0].SetColumnValue(entity, "G");
        mapping.Properties[0].GetColumnValue(entity).Should().Be("G");
    }

    [Fact]
    public void Should_create_key()
    {
        var sut = new ClassMappingConfigurator<User>();
        sut.Key(x => x.Id).AutoIncrement();
        var entity = new User();

        var mapping = sut.BuildMapping();

        mapping.Keys[0].PropertyName.Should().Be("Id");
        mapping.Keys[0].ColumnName.Should().Be("Id");
        mapping.Keys[0].IsAutoIncrement.Should().BeTrue();
        mapping.Keys[0].SetColumnValue(entity, 4);
        mapping.Keys[0].GetColumnValue(entity).Should().Be(4);
    }

    [Fact]
    public void Should_create_correct_hasManyBinding_when_child_has_foreignKeyProperty()
    {
        var sut = new ClassMappingConfigurator<User>();
        sut.HasMany(x => x.Addresses)
            .ForeignKey(x => x.UserId)
            .References(x => x.Id);

        var mapping = sut.BuildMapping();

        mapping.Collections[0].ForeignKey.ForeignKeyPropertyName.Should().Be("UserId");
        mapping.Collections[0].ForeignKey.ForeignKeyColumnName.Should().BeNull();
        mapping.Collections[0].ForeignKey.ReferencedPropertyName.Should().Be("Id");
        mapping.Collections[0].ElementType.Should().Be(typeof(Address));
        mapping.Collections[0].PropertyName.Should().Be("Addresses");
        mapping.Collections[0].PropertyType.Should().Be(typeof(IReadOnlyList<Address>));
    }

    [Fact]
    public void Should_create_correct_hasManyBinding_when_child_has_not_foreignKeyProperty()
    {
        var sut = new ClassMappingConfigurator<User>();
        sut.HasMany(x => x.Addresses)
            .ForeignKey("UserId")
            .References(x => x.Id);

        var mapping = sut.BuildMapping();

        mapping.Collections[0].ForeignKey.ForeignKeyPropertyName.Should().BeNull();
        mapping.Collections[0].ForeignKey.ForeignKeyColumnName.Should().Be("UserId");
        mapping.Collections[0].ForeignKey.ReferencedPropertyName.Should().Be("Id");
        mapping.Collections[0].ElementType.Should().Be(typeof(Address));
        mapping.Collections[0].PropertyName.Should().Be("Addresses");
        mapping.Collections[0].PropertyType.Should().Be(typeof(IReadOnlyList<Address>));
    }

    [Fact]
    public void Should_create_correct_hasOneBinding()
    {
        var sut = new ClassMappingConfigurator<User>();
        sut.Key(x => x.Id).AutoIncrement();
        sut.Property(x => x.FirstName);
        sut.HasOne(x => x.Data)
            .ForeignKey(x => x.UserId)
            .References(x => x.Id);
    }
}
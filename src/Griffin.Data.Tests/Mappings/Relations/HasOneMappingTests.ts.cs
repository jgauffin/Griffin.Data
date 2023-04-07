using FluentAssertions;
using Griffin.Data.Mappings.Properties;
using Griffin.Data.Mappings.Relations;
using Griffin.Data.Tests.Subjects;

namespace Griffin.Data.Tests.Mappings.Relations;

public class HasOneMappingTests
{
    private readonly IHasOneMapping _hasOneMapping;

    public HasOneMappingTests()
    {
        var reg = RegistryBuilder.Build();
        _hasOneMapping = reg.Get<User>().Children[0];
    }

    [Fact]
    public void Should_not_has_a_discriminator_as_default()
    {
        var fkAccessor = new PropertyMapping(typeof(Subjects.Data), typeof(int), x => ((Subjects.Data)x).UserId,
            (x, y) => ((Subjects.Data)x).UserId = (int)y);
        var referenceAccessor =
            new PropertyMapping(typeof(User), typeof(int), x => ((User)x).Id, (x, y) => ((User)x).Id = (int)y);
        var fk = new ForeignKeyMapping<User, Subjects.Data>("data", fkAccessor, referenceAccessor);

        var sut = new HasOneMapping<User, Subjects.Data>(fk, GetData, SetData);

        sut.HaveDiscriminator.Should().BeFalse();
    }

    [Fact]
    public void Should_use_discriminator()
    {
        var actual = _hasOneMapping.GetTypeUsingDiscriminator(new User());

        actual.Should().Be<AdminData>();
    }

    private Subjects.Data GetData(User arg)
    {
        return arg.Data!;
    }

    private void SetData(User arg1, Subjects.Data arg2)
    {
        arg1.Data = arg2;
    }
}

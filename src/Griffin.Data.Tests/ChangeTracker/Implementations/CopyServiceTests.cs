using FluentAssertions;
using Griffin.Data.ChangeTracking.Services.Implementations;
using Griffin.Data.Configuration;
using Griffin.Data.IntegrationTests.ChangeTracker.Implementations.Subjects;

namespace Griffin.Data.IntegrationTests.ChangeTracker.Implementations
{
    public class CopyServiceTests
    {
        [Fact]
        public void Should_be_able_to_create_class_with_parameter_constructor()
        {
            var sut = new CopyService();
            var source = new SomeClass(3);

            var actual = (SomeClass)sut.Copy(source);

            actual.Id.Should().Be(3);
        }

        [Fact]
        public void Should_be_able_to_create_class_with_default_constructor()
        {
            var sut = new CopyService();
            var source = new DefClass();

            var actual = sut.Copy(source);

            actual.Should().NotBeNull();
        }

        [Fact]
        public void Should_report_if_constructor_arguments_do_not_match_properties()
        {
            var sut = new CopyService();
            var source = new NotMatchingConstructor(4);

            Action actual = () => sut.Copy(source);

            actual.Should().Throw<MappingConfigurationException>().And.Message.Should().Contain("NotMatchingConstructor");
        }
    }
}

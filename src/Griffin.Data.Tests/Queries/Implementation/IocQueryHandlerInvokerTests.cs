using FluentAssertions;
using Griffin.Data.Queries;
using Griffin.Data.Queries.Implementation;
using Griffin.Data.Tests.Queries.Implementation.Subjects;

namespace Griffin.Data.Tests.Queries.Implementation;

public class IocQueryHandlerInvokerTests
{
    [Fact]
    public async Task Should_be_able_to_return_when_configured_correctly()
    {
        var provider = new ServiceProviderStub();
        var handler = new MyQueryHandler();
        provider.Register<IQueryHandler<MyQuery, MyQueryResult>>(handler);

        var sut = new IocQueryHandlerInvoker(provider);
        var result = await sut.Execute(new MyQuery());

        result.Should().NotBeNull();
    }

    [Fact]
    public async Task Should_tell_which_query_failed_when_handler_is_not_found()
    {
        var provider = new ServiceProviderStub();

        var sut = new IocQueryHandlerInvoker(provider);
        var actual = async () => await sut.Execute(new MyQuery());

        await actual.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage("*MyQuery*");
    }
}

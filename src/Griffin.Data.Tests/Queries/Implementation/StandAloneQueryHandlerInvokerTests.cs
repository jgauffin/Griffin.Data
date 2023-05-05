using System.Reflection;
using FluentAssertions;
using Griffin.Data.Configuration;
using Griffin.Data.Queries.Implementation;
using Griffin.Data.SqlServer;
using Griffin.Data.Tests.Queries.Implementation.Subjects;

namespace Griffin.Data.Tests.Queries.Implementation
{
    public class StandAloneQueryHandlerInvokerTests
    {
        [Fact]
        public async Task Should_be_able_to_return_when_configured_correctly()
        {
            StandAloneQueryHandlerInvoker.AddHandlerType(typeof(Query2Handler));
            var session = new Session(new DbConfiguration("Data Source=.;Initial Catalog=GriffinData;Integrated Security=True") { Dialect = new SqlServerDialect() });

            var sut = new StandAloneQueryHandlerInvoker(session);
            var result = await sut.Execute(new Query2());

            result.Should().NotBeNull();
        }

        [Fact]
        public async Task Should_tell_which_query_failed_when_handler_is_not_found()
        {
            StandAloneQueryHandlerInvoker.AddHandlerType(typeof(Query2Handler));
            var session = new Session(new DbConfiguration("Data Source=.;Initial Catalog=GriffinData;Integrated Security=True") { Dialect = new SqlServerDialect() });

            var sut = new StandAloneQueryHandlerInvoker(session);
            var actual = async () => await sut.Execute(new QueryWithoutHandler());

            await actual.Should().ThrowAsync<InvalidOperationException>()
                .WithMessage("*ManualQueryInvoker*");
        }
    }
}

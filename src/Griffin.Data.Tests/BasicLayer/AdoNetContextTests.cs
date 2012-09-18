using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Griffin.Data.BasicLayer;
using NSubstitute;
using Xunit;

namespace Griffin.Data.Tests.BasicLayer
{
    public class AdoNetContextTests
    {
        [Fact]
        public void CreateCommand()
        {
            var factory = Substitute.For<IConnectionFactory>();
            var connectionMock = Substitute.For<IDbConnection>();
            var expected = Substitute.For<IDbCommand>();
            connectionMock.CreateCommand().Returns(expected);
            factory.Create().Returns(connectionMock);

            var context = new AdoNetContext(factory);
            var command = context.CreateCommand();

            Assert.NotNull(command);
            Assert.Same(expected, command);
        }

        [Fact]
        public void CreateCommandWithTransaction()
        {
            var factory = Substitute.For<IConnectionFactory>();
            var connectionMock = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            connectionMock.CreateCommand().Returns(command);
            var transaction = Substitute.For<IDbTransaction>();
            connectionMock.BeginTransaction().Returns(transaction);
            factory.Create().Returns(connectionMock);

            var context = new AdoNetContext(factory);
            context.CreateUnitOfWork();
            var cmd = context.CreateCommand();

            Assert.Same(transaction, cmd.Transaction);
        }

        [Fact]
        public void CommandWithoutTransaction()
        {
            var factory = Substitute.For<IConnectionFactory>();
            var connectionMock = Substitute.For<IDbConnection>();
            var command = Substitute.For<IDbCommand>();
            connectionMock.CreateCommand().Returns(command);
            var transaction = Substitute.For<IDbTransaction>();
            connectionMock.BeginTransaction().Returns(transaction);
            factory.Create().Returns(connectionMock);

            var context = new AdoNetContext(factory);
            context.CreateUnitOfWork().Dispose();
            var cmd = context.CreateCommand();

            Assert.NotSame(transaction, cmd.Transaction);
        }

    }
}

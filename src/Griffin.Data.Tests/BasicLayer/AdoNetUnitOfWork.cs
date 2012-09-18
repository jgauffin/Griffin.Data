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
    public class AdoNetUnitOfWorkTests
    {
        [Fact]
        public void SaveChanges()
        {
            bool isDisposed = false, isCommited = false;
            var trans = Substitute.For<IDbTransaction>();
            var uow = new AdoNetUnitOfWork(trans, x => isDisposed = true, x => isCommited = true);

            uow.SaveChanges();

            Assert.True(isCommited);
            Assert.False(isDisposed);
            trans.Received().Commit();
        }

        [Fact]
        public void SaveChangesTwice()
        {
            bool isDisposed = false, isCommited = false;
            var trans = Substitute.For<IDbTransaction>();
            var uow = new AdoNetUnitOfWork(trans, x => isDisposed = true, x => isCommited = true);

            uow.SaveChanges();
            Assert.Throws<InvalidOperationException>(() => uow.SaveChanges());
        }

        [Fact]
        public void RolledBack()
        {
            bool isDisposed = false, isCommited = false;
            var trans = Substitute.For<IDbTransaction>();
            var uow = new AdoNetUnitOfWork(trans, x => isDisposed = true, x => isCommited = true);

            uow.Dispose();

            Assert.False(isCommited);
            Assert.True(isDisposed);
            trans.Received().Rollback();
        }

    }
}

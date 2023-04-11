using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentAssertions;
using Griffin.Data.ChangeTracking;
using Griffin.Data.ChangeTracking.Services.Implementations;
using Griffin.Data.ChangeTracking.Services.Implementations.v2;
using Griffin.Data.Mapper;
using Griffin.Data.Tests.Entities;
using Microsoft.Win32;

namespace Griffin.Data.Tests.ChangeTracker
{
    public class SingleEntityComparerTests : IntegrationTests
    {
        [Fact]
        public async Task Should_detect_changes()
        {
            var main2 = await Session.GetById<MainTable>(1);
            var main = CreateManualMain();

            var sut = new SingleEntityComparer(Registry);
            var result = sut.Compare(main2, main);

            result[0].State.Should().Be(ChangeState.Modified);
        }

        [Fact]
        public async Task Should_be_able_to_generate_report()
        {
            var main2 = await Session.GetById<MainTable>(1);
            var main = CreateManualMain();
            var sut = new SingleEntityChangeService(Registry);
            await sut.PersistChanges(Session, main2, main);

            var report = sut.CreateReport();
            var str = report.ToText();

        }

        private static MainTable CreateManualMain()
        {
            var main = new MainTable
            {
                Id = 1,
                Name = "Mine",
                Money = 3,
                Children = new[]
                {
                    new()
                    {
                        MainId = 1,
                        Id = 2,
                        ActionType = ActionType.Extra,
                        Action = new ExtraAction
                        {
                            Extra = "Something new",
                            Id = 1,
                            ChildId = 2
                        }
                    },
                    new ChildTable { ActionType = ActionType.Simple, Action = new SimpleAction { Simple = 4 } }
                }
            };
            main.AddLog("Hejsan");
            return main;
        }
    }
}

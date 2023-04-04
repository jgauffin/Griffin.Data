﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Griffin.Data.Configuration;

namespace Griffin.Data.Tests.Subjects.Mappings
{
    internal class SharedMainConfigurator : IEntityConfigurator<SharedMain>
    {
        public void Configure(IClassMappingConfigurator<SharedMain> config)
        {
            config.TableName("SharedMain");
            config.Key(x => x.Id).AutoIncrement();
            config.HasOne(x => x.Left)
                .SubsetColumn("ParentProperty", "Left")
                .ForeignKey(x=>x.MainId)
                .References(x=>x.Id);
            config.HasOne(x => x.Right)
                .SubsetColumn("ParentProperty", "Right")
                .ForeignKey(x => x.MainId)
                .References(x => x.Id);
        }
    }

    internal class SharedChildConfigurator : IEntityConfigurator<SharedChild>
    {
        public void Configure(IClassMappingConfigurator<SharedChild> config)
        {
            config.TableName("SharedChild");
            config.Key(x => x.Id).AutoIncrement();
            config.MapRemainingProperties();
        }
    }
}

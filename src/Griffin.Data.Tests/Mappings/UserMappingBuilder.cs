using Griffin.Data.Configuration;
using Griffin.Data.Mapper.Mappings;
using Griffin.Data.Tests.Subjects;

namespace Griffin.Data.Tests.Mappings;

internal class RegistryBuilder
{
    public static MappingRegistry Build()
    {
        var configs = new List<object>();
        var userConfig = new ClassMappingConfigurator<User>();
        userConfig.Key(x => x.Id);

        userConfig.HasMany(x => x.Addresses)
            .ForeignKey(x => x.UserId)
            .References(x => x.Id);

        userConfig.HasOne(x => x.Data)
            .Discriminator(x => x.State, state => typeof(AdminData))
            .ForeignKey(x => x!.UserId) //TODO: How to remove the warning.
            .References(x => x.Id);

        userConfig.MapRemainingProperties();
        configs.Add(userConfig);

        var addressConfig = new ClassMappingConfigurator<Address>();
        addressConfig.MapRemainingProperties();
        configs.Add(addressConfig);

        var userData = new ClassMappingConfigurator<UserData>();
        addressConfig.MapRemainingProperties();
        configs.Add(userData);

        var ad = new ClassMappingConfigurator<AdminData>();
        addressConfig.MapRemainingProperties();
        configs.Add(ad);

        var data = new ClassMappingConfigurator<Subjects.Data>();
        data.MapRemainingProperties();
        configs.Add(data);

        var mappings = BuildMappings(configs);
        var reg = new MappingRegistry();
        foreach (var mapping in mappings)
        {
            reg.Add(mapping);
        }

        BuildRelations(reg, configs);

        return reg;
    }

    public static IEnumerable<ClassMapping> BuildMappings(IReadOnlyList<object> items)
    {
        foreach (var item in items)
        {
            yield return ((IMappingBuilder)item).BuildMapping(true);
        }
    }

    public static void BuildRelations(IMappingRegistry registry, IReadOnlyList<object> items)
    {
        foreach (var item in items)
        {
            ((IMappingBuilder)item).BuildRelations(registry);
        }
    }
}

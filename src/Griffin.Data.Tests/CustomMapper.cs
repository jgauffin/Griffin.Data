using Griffin.Data.Converters;
using Griffin.Data.Mappings;

namespace Griffin.Data.Tests
{
    public class CustomMapper : SimpleMapper<User>
    {
        public CustomMapper()
        {
            Add(x => x.FirstName, "first_name");
            Add(x => x.Age, "age", new DelegateConverter<string, int>(int.Parse));
            Add(x => x.Id, "user_id", new DotNetConverter<int>());
        }
    }
}
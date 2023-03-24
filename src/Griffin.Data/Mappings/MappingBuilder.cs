using System.Collections.Generic;
using Griffin.Data.Mappings.Properties;

namespace Griffin.Data.Mappings;

public class MappingBuilder
{
    public ClassMapping ClassMapping { get; set; }
    public List<PropertyMapping> Properties { get; set; }
    public string TableName { get; set; }
}
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Griffin.Data.Mappings;

namespace Griffin.Data.Dialects;

public interface ISqlDialect
{
    Task Insert(ClassMapping mapping, object entity, IDbCommand command);
    Task Update(ClassMapping mapping, object entity, DbCommand command);
}
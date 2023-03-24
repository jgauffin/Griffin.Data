using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;

namespace Griffin.Data.Converters;

public interface IRecordToValueConverter<TTo>
{
    [return: NotNull]
    TTo Convert(IDataRecord value);

    void ConvertToColumns([NotNull] TTo entity, IDictionary<string, object> columns);
}
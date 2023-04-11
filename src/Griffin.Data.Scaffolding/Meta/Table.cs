using System;
using System.Collections.Generic;
using Griffin.Data.Helpers;
using Griffin.Data.Scaffolding.Helpers;

namespace Griffin.Data.Scaffolding.Meta;

public class Table
{
    public Table(string tableName)
    {
        TableName = tableName ?? throw new ArgumentNullException(nameof(tableName));
        ClassName = tableName.ToPascalCase().Singularize();
    }

    public string ClassName { get; }
    public List<Column> Columns { get; set; } = new();
    public ICollection<ForeignKeyColumn> ForeignKeys { get; set; } = new List<ForeignKeyColumn>();
    public string Namespace { get; set; } = "";
    public ICollection<Reference> References { get; set; } = new List<Reference>();

    public string TableName { get; set; }
}

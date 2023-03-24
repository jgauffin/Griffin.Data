using System;
using System.Collections.Generic;
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

    public string TableName { get; set; }
    public List<Column> Columns { get; set; } = new();
    public ICollection<ForeignKeyColumn> ForeignKeys { get; set; } = new List<ForeignKeyColumn>();
    public ICollection<Reference> References { get; set; } = new List<Reference>();
    public string Namespace { get; set; } = "";
}
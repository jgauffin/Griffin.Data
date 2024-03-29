﻿using Griffin.Data.Queries;

#pragma warning disable CS8618

namespace TestApp.Users.Queries;

public class ListUsers : IQuery<ListUsersResult>, IPagedQuery, ISortedQuery
{
    public string NameToFind { get; set; }
    public int? PageNumber { get; set; }
    public int? PageSize { get; set; }
    public IList<SortEntry> SortEntries { get; set; } = new List<SortEntry>();
}

using System;
using System.Collections.Generic;

namespace Shared.DataTypes
{
    /// <summary>
    /// Output Paged Result DTO trả về cho GUI DataGrid
    /// </summary>
    public class PagedResult<T>
    {
        public List<T> Items { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / (PageSize > 0 ? PageSize : 1));
    }
}


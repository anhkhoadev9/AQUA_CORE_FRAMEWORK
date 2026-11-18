using System.Collections.Generic;

namespace Shared.DataTypes
{
    /// <summary>
    /// DTO Nạp Dữ liệu Dùng chung phục vụ Server-Side PFSS
    /// </summary>
    public class GridDataRequest
    {
        public int PageIndex { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchText { get; set; }
        public string? SortColumn { get; set; }
        public bool IsSortAscending { get; set; } = true;
        public Dictionary<string, object> Filters { get; set; } = new();
    }
}

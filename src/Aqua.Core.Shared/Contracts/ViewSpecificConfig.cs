using System.Collections.Generic;

namespace Shared.GUI.Contracts
{
    /// <summary>
    /// Cấu hình chi tiết cho từng ViewKey cụ thể
    /// </summary>
    public class ViewSpecificConfig
    {
        public PaginationStrategy Strategy { get; set; } = PaginationStrategy.ServerSide;
        public int PageSize { get; set; } = 10;
        public List<int> PageSizeOptions { get; set; } = new() { 10, 20, 50 };
        public Dictionary<string, object> DefaultFilters { get; set; } = new();
    }
}


using System.Collections.Generic;

namespace Shared.GUI.Contracts
{
    /// <summary>
    /// Lớp ánh xạ toàn bộ Section "ListViewSettings" từ appsettings.json
    /// </summary>
    public class ListViewSettings
    {
        public PaginationStrategy DefaultStrategy { get; set; } = PaginationStrategy.ServerSide;
        public int DefaultPageSize { get; set; } = 10;
        public Dictionary<string, ViewSpecificConfig> Views { get; set; } = new();
    }
}


using Shared.DataTypes;
using Shared.GUI.Contracts;
using System.Collections.Generic;

namespace Aqua.Core.GUI.Service.Adapters
{
    /// <summary>
    /// Contract Adapter chuyển đổi cấu hình ViewSpecificConfig thành GridDataRequest (/skill 30)
    /// </summary>
    public interface IGridDataRequestAdapter
    {
        GridDataRequest CreateInitialRequest(ViewSpecificConfig config);

        GridDataRequest CreateRequest(
            ViewSpecificConfig config,
            int pageIndex = 1,
            int? pageSize = null,
            string? searchText = null,
            string? sortColumn = null,
            bool isSortAscending = true,
            Dictionary<string, object>? additionalFilters = null);
    }
}

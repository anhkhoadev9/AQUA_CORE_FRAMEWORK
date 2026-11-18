using Shared.DataTypes;
using Shared.GUI.Contracts;
using System;
using System.Collections.Generic;

namespace Aqua.Core.GUI.Service.Adapters
{
    /// <summary>
    /// Triển khai Adapter Khởi tạo GridDataRequest theo chuẩn US-01.1 (AC-01, AC-02, AC-03)
    /// </summary>
    public class GridDataRequestAdapter : IGridDataRequestAdapter
    {
        public GridDataRequest CreateInitialRequest(ViewSpecificConfig config)
        {
            if (config == null)
            {
                config = new ViewSpecificConfig();
            }

            return CreateRequest(
                config: config,
                pageIndex: 1,
                pageSize: config.PageSize > 0 ? config.PageSize : 10,
                searchText: null,
                sortColumn: null,
                isSortAscending: true,
                additionalFilters: null);
        }

        public GridDataRequest CreateRequest(
            ViewSpecificConfig config,
            int pageIndex = 1,
            int? pageSize = null,
            string? searchText = null,
            string? sortColumn = null,
            bool isSortAscending = true,
            Dictionary<string, object>? additionalFilters = null)
        {
            if (config == null)
            {
                config = new ViewSpecificConfig();
            }

            int resolvedPageSize = pageSize.HasValue && pageSize.Value > 0
                ? pageSize.Value
                : (config.PageSize > 0 ? config.PageSize : 10);

            var request = new GridDataRequest
            {
                PageIndex = pageIndex > 0 ? pageIndex : 1,
                PageSize = resolvedPageSize,
                SearchText = searchText,
                SortColumn = sortColumn,
                IsSortAscending = isSortAscending,
                Filters = new Dictionary<string, object>()
            };

            // AC-03: Tự động áp dụng DefaultFilters ngay từ lần nạp đầu tiên
            if (config.DefaultFilters != null && config.DefaultFilters.Count > 0)
            {
                foreach (var kvp in config.DefaultFilters)
                {
                    request.Filters[kvp.Key] = kvp.Value;
                }
            }

            // Bổ sung thêm các filter động từ giao diện nếu có
            if (additionalFilters != null && additionalFilters.Count > 0)
            {
                foreach (var kvp in additionalFilters)
                {
                    request.Filters[kvp.Key] = kvp.Value;
                }
            }

            return request;
        }
    }
}

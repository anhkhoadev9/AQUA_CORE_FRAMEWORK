using Aqua.Core.GUI.Service.Localization;
using Core.BLL.Constants;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Logging;
using Shared.DataTypes;
using Shared.GUI.Contracts;
using Shared.GUI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Shared.GUI.Components
{
    /// <summary>
    /// Generic Blazor Component AquaDataGrid tuân thủ US-01.1 Tech Spec Section 7 & /skill 40
    /// </summary>
    public partial class AquaDataGrid<TItem> : ComponentBase
    {
        [Inject] public IListViewConfigProvider ConfigProvider { get; set; } = default!;
        [Inject] public IErrorLocalizer? ErrorLocalizer { get; set; }
        [Inject] public ILogger<AquaDataGrid<TItem>>? Logger { get; set; }

        [Parameter] public string ViewKey { get; set; } = string.Empty;
        [Parameter] public RenderFragment? ChildContent { get; set; }
        [Parameter] public bool ThrowOnMissingViewKey { get; set; } = false;
        [Parameter] public Func<GridDataRequest, Task>? OnLoadData { get; set; }

        public ViewSpecificConfig ActiveConfig { get; private set; } = new();
        public GridDataRequest CurrentRequest { get; private set; } = new();
        public bool IsLoading { get; private set; } = false;
        public string? ErrorCode { get; private set; }
        public string? ErrorMessage { get; private set; }
        public bool InitialLoadExecuted { get; private set; } = false;

        protected override async Task OnInitializedAsync()
        {
            await InitializeComponentAsync();
        }

        public async Task InitializeComponentAsync()
        {
            // 1. Kiểm tra tính hợp lệ của ViewKey (TC-GRID-1.1-EC02)
            if (string.IsNullOrWhiteSpace(ViewKey))
            {
                ErrorCode = DomainErrorCodes.ListView.ViewKeyRequired;
                ErrorMessage = ErrorLocalizer?.GetLocalizedMessage(ErrorCode) ?? "ViewKey is required.";
                Logger?.LogWarning(ErrorCode);

                if (ThrowOnMissingViewKey)
                {
                    throw new InvalidOperationException(DomainErrorCodes.ListView.ViewKeyRequired);
                }
            }

            // 2. Đọc cấu hình theo ViewKey (hoặc lấy fallback) (AC-01, AC-02, TC-GRID-1.1-EC01)
            ActiveConfig = ConfigProvider != null
                ? ConfigProvider.GetConfiguration(ViewKey)
                : new ViewSpecificConfig();

            // 3. Khởi tạo đối tượng GridDataRequest ban đầu
            CurrentRequest = new GridDataRequest
            {
                PageIndex = 1,
                PageSize = ActiveConfig.PageSize > 0 ? ActiveConfig.PageSize : 10,
                SortColumn = null,
                IsSortAscending = true,
                Filters = new Dictionary<string, object>()
            };

            // 4. Nạp Default Filters nếu có trong cấu hình (AC-03, TC-GRID-1.1-HP01, HP02)
            if (ActiveConfig.DefaultFilters != null && ActiveConfig.DefaultFilters.Any())
            {
                foreach (var kvp in ActiveConfig.DefaultFilters)
                {
                    CurrentRequest.Filters[kvp.Key] = kvp.Value;
                }
            }

            // 5. Kích hoạt tải dữ liệu lần đầu
            await ExecuteInitialLoadAsync();
        }

        public async Task ExecuteInitialLoadAsync()
        {
            IsLoading = true;
            try
            {
                if (OnLoadData != null)
                {
                    await OnLoadData(CurrentRequest);
                }

                // Thực thi nạp dữ liệu tùy theo ActiveConfig.Strategy
                await LoadDataAsync();
                InitialLoadExecuted = true;
            }
            finally
            {
                IsLoading = false;
            }
        }

        protected virtual async Task LoadDataAsync()
        {
            if (ActiveConfig.Strategy == PaginationStrategy.ClientSide)
            {
                // Chiến lược ClientSide (TC-GRID-1.1-HP02): Tải toàn bộ mảng dữ liệu 1 lần vào RAM
                await LoadClientSideDataAsync();
            }
            else
            {
                // Chiến lược ServerSide (TC-GRID-1.1-HP01): Đẩy Skip/Take xuống Backend SQL Server
                await LoadServerSideDataAsync();
            }
        }

        protected virtual async Task LoadClientSideDataAsync()
        {
            await Task.CompletedTask;
        }

        protected virtual async Task LoadServerSideDataAsync()
        {
            await Task.CompletedTask;
        }
    }
}

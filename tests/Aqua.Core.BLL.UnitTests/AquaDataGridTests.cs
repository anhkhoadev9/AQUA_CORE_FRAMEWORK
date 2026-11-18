#pragma warning disable BL0005

using Aqua.Core.GUI.Service.Localization;
using Core.BLL.Constants;
using Microsoft.Extensions.Configuration;
using Shared.GUI.Components;
using Shared.GUI.Contracts;
using Shared.GUI.Services;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Aqua.Core.BLL.UnitTests
{
    public class AquaDataGridTests
    {
        private const string SampleJson = @"
{
  ""ListViewSettings"": {
    ""DefaultStrategy"": ""ServerSide"",
    ""DefaultPageSize"": 10,
    ""Views"": {
      ""CustomerListView"": {
        ""Strategy"": ""ServerSide"",
        ""PageSize"": 20,
        ""PageSizeOptions"": [10, 20, 50, 100],
        ""DefaultFilters"": {
          ""Status"": ""Active""
        }
      },
      ""LookupTypeListView"": {
        ""Strategy"": ""ClientSide"",
        ""PageSize"": 5,
        ""PageSizeOptions"": [5, 10, 20],
        ""DefaultFilters"": {
          ""IsActive"": true
        }
      }
    }
  }
}";

        private IListViewConfigProvider CreateConfigProvider(string json = SampleJson)
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(json)));
            return new ListViewConfigProvider(builder.Build());
        }

        [Fact]
        public async Task TC_GRID_1_1_HP01_CustomerListView_OnInitialized_ShouldConfigureCorrectly()
        {
            // Given: appsettings.json định nghĩa CustomerListView
            var provider = CreateConfigProvider();
            var component = new AquaDataGrid<object>
            {
                ConfigProvider = provider,
                ViewKey = "CustomerListView"
            };

            bool loadCalled = false;
            component.OnLoadData = req =>
            {
                loadCalled = true;
                return Task.CompletedTask;
            };

            // When: Component AquaDataGrid được khởi tạo
            await component.InitializeComponentAsync();

            // ExpectedResult:
            // 1. ActiveConfig.Strategy bằng ServerSide
            Assert.Equal(PaginationStrategy.ServerSide, component.ActiveConfig.Strategy);
            // 2. CurrentRequest.PageSize bằng 20, PageIndex bằng 1
            Assert.Equal(20, component.CurrentRequest.PageSize);
            Assert.Equal(1, component.CurrentRequest.PageIndex);
            // 3. CurrentRequest.Filters['Status'] bằng 'Active'
            Assert.Equal("Active", component.CurrentRequest.Filters["Status"]?.ToString());
            // 4. Kích hoạt gọi hàm tải dữ liệu server
            Assert.True(loadCalled);
            Assert.True(component.InitialLoadExecuted);
        }

        [Fact]
        public async Task TC_GRID_1_1_HP02_LookupTypeListView_OnInitialized_ShouldConfigureClientSide()
        {
            // Given: appsettings.json định nghĩa LookupTypeListView
            var provider = CreateConfigProvider();
            var component = new AquaDataGrid<object>
            {
                ConfigProvider = provider,
                ViewKey = "LookupTypeListView"
            };

            // When: Component AquaDataGrid được khởi tạo
            await component.InitializeComponentAsync();

            // ExpectedResult:
            // 1. ActiveConfig.Strategy bằng ClientSide
            Assert.Equal(PaginationStrategy.ClientSide, component.ActiveConfig.Strategy);
            // 2. CurrentRequest.PageSize bằng 5
            Assert.Equal(5, component.CurrentRequest.PageSize);
            // 3. Mảng dữ liệu được tải toàn bộ 1 lần
            Assert.True(component.InitialLoadExecuted);
        }

        [Fact]
        public async Task TC_GRID_1_1_EC01_UnconfiguredListView_ShouldFallbackToDefault()
        {
            // Given: ViewKey='UnconfiguredListView' không được cấu hình riêng
            var provider = CreateConfigProvider();
            var component = new AquaDataGrid<object>
            {
                ConfigProvider = provider,
                ViewKey = "UnconfiguredListView"
            };

            // When: Khởi tạo Component
            await component.InitializeComponentAsync();

            // ExpectedResult:
            // 1. Hệ thống không crash
            // 2. Tự động fallback về Strategy = ServerSide, PageSize = 10
            Assert.Equal(PaginationStrategy.ServerSide, component.ActiveConfig.Strategy);
            Assert.Equal(10, component.CurrentRequest.PageSize);
            // 3. DefaultFilters khởi tạo rỗng
            Assert.Empty(component.CurrentRequest.Filters);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task TC_GRID_1_1_EC02_EmptyViewKey_ShouldApplyFallbackAndSetErrorCode(string emptyKey)
        {
            // Given: ViewKey rỗng hoặc whitespace
            var provider = CreateConfigProvider();
            var localizer = new ErrorLocalizer();
            var component = new AquaDataGrid<object>
            {
                ConfigProvider = provider,
                ErrorLocalizer = localizer,
                ViewKey = emptyKey,
                ThrowOnMissingViewKey = false
            };

            // When: Khởi tạo Component
            await component.InitializeComponentAsync();

            // ExpectedResult:
            // 1. Áp dụng cấu hình Default của framework
            Assert.Equal(PaginationStrategy.ServerSide, component.ActiveConfig.Strategy);
            Assert.Equal(10, component.CurrentRequest.PageSize);
            // 2. Ghi cảnh báo hoặc gán mã lỗi chuẩn LISTVIEW_VIEWKEY_REQUIRED
            Assert.Equal(DomainErrorCodes.ListView.ViewKeyRequired, component.ErrorCode);
            Assert.Equal("Thuộc tính ViewKey bắt buộc phải được khai báo trên Component.", component.ErrorMessage);
        }

        [Fact]
        public async Task TC_GRID_1_1_EC02_EmptyViewKey_WhenThrowConfigured_ShouldThrowException()
        {
            var provider = CreateConfigProvider();
            var component = new AquaDataGrid<object>
            {
                ConfigProvider = provider,
                ViewKey = "",
                ThrowOnMissingViewKey = true
            };

            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => component.InitializeComponentAsync());
            Assert.Equal(DomainErrorCodes.ListView.ViewKeyRequired, ex.Message);
        }
    }
}

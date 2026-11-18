using Microsoft.Extensions.Configuration;
using Shared.GUI.Contracts;
using Shared.GUI.Services;
using System.IO;
using System.Text;
using Xunit;

namespace Aqua.Core.BLL.UnitTests
{
    public class ListViewConfigProviderTests
    {
        private IConfiguration CreateConfiguration(string json)
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(json)));
            return builder.Build();
        }

        private const string SampleJsonConfig = @"
{
  ""ListViewSettings"": {
    ""DefaultStrategy"": ""ServerSide"",
    ""DefaultPageSize"": 10,
    ""Views"": {
      ""LookupTypeListView"": {
        ""Strategy"": ""ClientSide"",
        ""PageSize"": 5,
        ""PageSizeOptions"": [5, 10, 20],
        ""DefaultFilters"": {
          ""IsActive"": true
        }
      },
      ""CustomerListView"": {
        ""Strategy"": ""ServerSide"",
        ""PageSize"": 20,
        ""PageSizeOptions"": [10, 20, 50, 100],
        ""DefaultFilters"": {
          ""Status"": ""Active""
        }
      },
      ""InvalidPageSizeListView"": {
        ""Strategy"": ""ServerSide"",
        ""PageSize"": -5,
        ""PageSizeOptions"": [10, 20],
        ""DefaultFilters"": {}
      }
    }
  }
}";

        [Fact]
        public void TC_GRID_1_1_HP01_CustomerListView_ShouldReturnConfiguredSettings()
        {
            // Given: appsettings.json định nghĩa view CustomerListView
            var configuration = CreateConfiguration(SampleJsonConfig);
            var provider = new ListViewConfigProvider(configuration);

            // When: Lấy cấu hình cho CustomerListView
            var config = provider.GetConfiguration("CustomerListView");

            // ExpectedResult:
            Assert.NotNull(config);
            Assert.Equal(PaginationStrategy.ServerSide, config.Strategy);
            Assert.Equal(20, config.PageSize);
            Assert.True(config.DefaultFilters.ContainsKey("Status"));
            Assert.Equal("Active", config.DefaultFilters["Status"]?.ToString());
        }

        [Fact]
        public void TC_GRID_1_1_HP02_LookupTypeListView_ShouldReturnClientSideAndPageSize5()
        {
            // Given: appsettings.json định nghĩa view LookupTypeListView
            var configuration = CreateConfiguration(SampleJsonConfig);
            var provider = new ListViewConfigProvider(configuration);

            // When: Lấy cấu hình cho LookupTypeListView
            var config = provider.GetConfiguration("LookupTypeListView");

            // ExpectedResult:
            Assert.NotNull(config);
            Assert.Equal(PaginationStrategy.ClientSide, config.Strategy);
            Assert.Equal(5, config.PageSize);
            Assert.True(config.DefaultFilters.ContainsKey("IsActive"));
        }

        [Fact]
        public void TC_GRID_1_1_EC01_UnconfiguredListView_ShouldFallbackToDefaultSettings()
        {
            // Given: Không có mục khai báo cho UnconfiguredListView
            var configuration = CreateConfiguration(SampleJsonConfig);
            var provider = new ListViewConfigProvider(configuration);

            // When: Khởi tạo với ViewKey='UnconfiguredListView'
            var config = provider.GetConfiguration("UnconfiguredListView");

            // ExpectedResult: Fallback về Strategy = ServerSide, PageSize = 10, DefaultFilters rỗng
            Assert.NotNull(config);
            Assert.Equal(PaginationStrategy.ServerSide, config.Strategy);
            Assert.Equal(10, config.PageSize);
            Assert.NotNull(config.DefaultFilters);
            Assert.Empty(config.DefaultFilters);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData(null)]
        public void TC_GRID_1_1_EC02_EmptyOrWhitespaceViewKey_ShouldFallbackToDefault(string? viewKey)
        {
            // Given: ViewKey rỗng hoặc whitespace
            var configuration = CreateConfiguration(SampleJsonConfig);
            var provider = new ListViewConfigProvider(configuration);

            // When:
            var config = provider.GetConfiguration(viewKey!);

            // ExpectedResult:
            Assert.NotNull(config);
            Assert.Equal(PaginationStrategy.ServerSide, config.Strategy);
            Assert.Equal(10, config.PageSize);
            Assert.Empty(config.DefaultFilters);
        }

        [Fact]
        public void TC_GRID_1_1_NC01_MissingListViewSettingsSection_ShouldGracefullyFallback()
        {
            // Given: File appsettings không có node ListViewSettings
            var configuration = CreateConfiguration("{}");
            var provider = new ListViewConfigProvider(configuration);

            // When: Lấy cấu hình ViewKey bất kỳ
            var config = provider.GetConfiguration("AnyView");

            // ExpectedResult: Không crash, tự tạo mặc định ServerSide, PageSize = 10
            Assert.NotNull(config);
            Assert.Equal(PaginationStrategy.ServerSide, config.Strategy);
            Assert.Equal(10, config.PageSize);
            Assert.Empty(config.DefaultFilters);
        }

        [Fact]
        public void TC_GRID_1_1_NC02_InvalidPageSize_ShouldFallbackToDefaultPageSize()
        {
            // Given: Cấu hình PageSize = -5 trong appsettings.json
            var configuration = CreateConfiguration(SampleJsonConfig);
            var provider = new ListViewConfigProvider(configuration);

            // When: Khởi tạo với InvalidPageSizeListView
            var config = provider.GetConfiguration("InvalidPageSizeListView");

            // ExpectedResult: BLL chặn giá trị không hợp lệ, fallback về 10
            Assert.NotNull(config);
            Assert.Equal(10, config.PageSize);
        }
    }
}

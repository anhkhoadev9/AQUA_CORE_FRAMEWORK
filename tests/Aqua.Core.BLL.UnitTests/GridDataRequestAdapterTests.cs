using Aqua.Core.GUI.Service.Adapters;
using Aqua.Core.GUI.Service.Localization;
using Aqua.Core.GUI.Service.Services;
using Core.BLL.Constants;
using Microsoft.Extensions.Configuration;
using Shared.DataTypes;
using Shared.GUI.Contracts;
using Shared.GUI.Services;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Aqua.Core.BLL.UnitTests
{
    public class GridDataRequestAdapterTests
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
        ""PageSizeOptions"": [10, 20, 50],
        ""DefaultFilters"": {
          ""Status"": ""Active""
        }
      }
    }
  }
}";

        [Fact]
        public void CreateInitialRequest_WithValidConfig_ShouldApplyPageSizeAndDefaultFilters()
        {
            // Given:
            var config = new ViewSpecificConfig
            {
                Strategy = PaginationStrategy.ServerSide,
                PageSize = 25,
                DefaultFilters = new Dictionary<string, object>
                {
                    { "Status", "Active" },
                    { "TenantId", 100 }
                }
            };
            var adapter = new GridDataRequestAdapter();

            // When:
            var request = adapter.CreateInitialRequest(config);

            // Then:
            Assert.NotNull(request);
            Assert.Equal(1, request.PageIndex);
            Assert.Equal(25, request.PageSize);
            Assert.Equal(2, request.Filters.Count);
            Assert.Equal("Active", request.Filters["Status"]);
            Assert.Equal(100, request.Filters["TenantId"]);
        }

        [Fact]
        public void CreateRequest_WithAdditionalFilters_ShouldMergeWithoutMutatingDefaultFilters()
        {
            // Given:
            var config = new ViewSpecificConfig
            {
                PageSize = 15,
                DefaultFilters = new Dictionary<string, object>
                {
                    { "Role", "Admin" }
                }
            };
            var adapter = new GridDataRequestAdapter();
            var uiFilters = new Dictionary<string, object>
            {
                { "SearchKeyword", "John" }
            };

            // When:
            var request = adapter.CreateRequest(
                config: config,
                pageIndex: 2,
                pageSize: 30,
                searchText: "Test",
                sortColumn: "CreatedDate",
                isSortAscending: false,
                additionalFilters: uiFilters);

            // Then:
            Assert.Equal(2, request.PageIndex);
            Assert.Equal(30, request.PageSize);
            Assert.Equal("Test", request.SearchText);
            Assert.Equal("CreatedDate", request.SortColumn);
            Assert.False(request.IsSortAscending);
            Assert.Equal(2, request.Filters.Count);
            Assert.Equal("Admin", request.Filters["Role"]);
            Assert.Equal("John", request.Filters["SearchKeyword"]);
            // Ensure default filters in config was not mutated
            Assert.Single(config.DefaultFilters);
        }

        [Fact]
        public async Task ListViewGuiService_GetInitialRequestAsync_ShouldSucceedWithCorrectContract()
        {
            // Given:
            var configBuilder = new ConfigurationBuilder();
            configBuilder.AddJsonStream(new MemoryStream(Encoding.UTF8.GetBytes(SampleJson)));
            var configProvider = new ListViewConfigProvider(configBuilder.Build());
            var adapter = new GridDataRequestAdapter();
            var localizer = new ErrorLocalizer();
            var guiService = new ListViewGuiService(configProvider, adapter, localizer);

            // When:
            var result = await guiService.GetInitialRequestAsync("CustomerListView");

            // Then:
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Data);
            Assert.Equal(1, result.Data.PageIndex);
            Assert.Equal(20, result.Data.PageSize);
            Assert.Equal("Active", result.Data.Filters["Status"]);
        }

        [Fact]
        public void ErrorLocalizer_ShouldTranslateCodesAndPlaceholders()
        {
            // Given:
            var localizer = new ErrorLocalizer();

            // When:
            var msg1 = localizer.GetLocalizedMessage(DomainErrorCodes.ListView.ViewKeyRequired);
            var msg2 = localizer.GetLocalizedMessage(DomainErrorCodes.System.FatalException, new Dictionary<string, object>
            {
                { "ExceptionMessage", "Database timeout" }
            });

            // Then:
            Assert.Equal("Thuộc tính ViewKey bắt buộc phải được khai báo trên Component.", msg1);
            Assert.Contains("Database timeout", msg2);
        }
    }
}

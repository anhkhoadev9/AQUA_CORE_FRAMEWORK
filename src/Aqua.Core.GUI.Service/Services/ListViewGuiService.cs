using Aqua.Core.GUI.Service.Adapters;
using Aqua.Core.GUI.Service.Localization;
using Core.BLL.Constants;
using Shared.DataTypes;
using Shared.GUI.Contracts;
using Shared.GUI.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Aqua.Core.GUI.Service.Services
{
    /// <summary>
    /// Triển khai ListViewGuiService theo /skill 30 với Safe Exception Handling
    /// </summary>
    public class ListViewGuiService : IListViewGuiService
    {
        private readonly IListViewConfigProvider _configProvider;
        private readonly IGridDataRequestAdapter _requestAdapter;
        private readonly IErrorLocalizer _errorLocalizer;

        public ListViewGuiService(
            IListViewConfigProvider configProvider,
            IGridDataRequestAdapter requestAdapter,
            IErrorLocalizer errorLocalizer)
        {
            _configProvider = configProvider ?? throw new ArgumentNullException(nameof(configProvider));
            _requestAdapter = requestAdapter ?? throw new ArgumentNullException(nameof(requestAdapter));
            _errorLocalizer = errorLocalizer ?? throw new ArgumentNullException(nameof(errorLocalizer));
        }

        public ViewSpecificConfig GetConfiguration(string viewKey)
        {
            return _configProvider.GetConfiguration(viewKey);
        }

        public async Task<BOProcessResult<GridDataRequest>> GetInitialRequestAsync(string viewKey)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(viewKey))
                {
                    var fallbackConfig = _configProvider.GetConfiguration(viewKey);
                    var fallbackRequest = _requestAdapter.CreateInitialRequest(fallbackConfig);
                    return await Task.FromResult(BOProcessResult<GridDataRequest>.Success(fallbackRequest));
                }

                var config = _configProvider.GetConfiguration(viewKey);
                var request = _requestAdapter.CreateInitialRequest(config);

                return await Task.FromResult(BOProcessResult<GridDataRequest>.Success(request));
            }
            catch (Exception ex)
            {
                // Safe Exception Handling (/skill 30): Không nuốt lỗi, đóng gói thành DomainErrorCodes.System.FatalException
                var args = new Dictionary<string, object>
                {
                    { "ExceptionMessage", ex.Message }
                };
                return await Task.FromResult(BOProcessResult<GridDataRequest>.Failed(DomainErrorCodes.System.FatalException, args));
            }
        }
    }
}

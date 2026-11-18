using Core.BLL.Constants;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Shared.GUI.Contracts;
using System;
using System.Collections.Generic;

namespace Shared.GUI.Services
{
    /// <summary>
    /// Service Đọc Cấu hình (Provider Pattern) tuân thủ AC-01, AC-02, AC-03 và các kịch bản Edge/Negative Cases
    /// </summary>
    public class ListViewConfigProvider : IListViewConfigProvider
    {
        private readonly ListViewSettings _settings;
        private readonly ILogger<ListViewConfigProvider>? _logger;

        public ListViewConfigProvider(IConfiguration configuration, ILogger<ListViewConfigProvider>? logger = null)
        {
            _logger = logger;

            if (configuration == null)
            {
                _logger?.LogWarning(DomainErrorCodes.ListView.ConfigSectionMissing);
                _settings = new ListViewSettings();
                return;
            }

            var section = configuration.GetSection("ListViewSettings");
            if (!section.Exists())
            {
                // TC-GRID-1.1-NC01: Không có node ListViewSettings -> log warning mã lỗi và fallback mặc định
                _logger?.LogWarning(DomainErrorCodes.ListView.ConfigSectionMissing);
                _settings = new ListViewSettings();
            }
            else
            {
                _settings = section.Get<ListViewSettings>() ?? new ListViewSettings();
            }

            // Kiểm tra tính hợp lệ của DefaultPageSize
            if (_settings.DefaultPageSize <= 0)
            {
                _logger?.LogWarning(DomainErrorCodes.ListView.InvalidPageSize);
                _settings.DefaultPageSize = 10;
            }

            if (_settings.Views == null)
            {
                _settings.Views = new Dictionary<string, ViewSpecificConfig>();
            }
        }

        public ViewSpecificConfig GetConfiguration(string viewKey)
        {
            // TC-GRID-1.1-EC02: ViewKey rỗng hoặc whitespace
            if (string.IsNullOrWhiteSpace(viewKey))
            {
                _logger?.LogWarning(DomainErrorCodes.ListView.ViewKeyRequired);
                return CreateFallbackConfig();
            }

            // TC-GRID-1.1-HP01 & HP02: Tìm cấu hình theo ViewKey
            if (_settings.Views.TryGetValue(viewKey, out var config) && config != null)
            {
                // TC-GRID-1.1-NC02: BLL chặn giá trị không hợp lệ (PageSize <= 0)
                if (config.PageSize <= 0)
                {
                    _logger?.LogWarning(DomainErrorCodes.ListView.InvalidPageSize);
                    config.PageSize = _settings.DefaultPageSize > 0 ? _settings.DefaultPageSize : 10;
                }

                if (config.PageSizeOptions == null || config.PageSizeOptions.Count == 0)
                {
                    config.PageSizeOptions = new List<int> { 10, 20, 50 };
                }

                if (config.DefaultFilters == null)
                {
                    config.DefaultFilters = new Dictionary<string, object>();
                }

                return config;
            }

            // TC-GRID-1.1-EC01: ViewKey không được cấu hình riêng -> Fallback về mặc định
            return CreateFallbackConfig();
        }

        private ViewSpecificConfig CreateFallbackConfig()
        {
            return new ViewSpecificConfig
            {
                Strategy = _settings.DefaultStrategy,
                PageSize = _settings.DefaultPageSize > 0 ? _settings.DefaultPageSize : 10,
                PageSizeOptions = new List<int> { 10, 20, 50 },
                DefaultFilters = new Dictionary<string, object>()
            };
        }
    }
}

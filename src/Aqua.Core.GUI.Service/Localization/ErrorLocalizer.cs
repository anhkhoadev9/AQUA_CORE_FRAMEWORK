using Core.BLL.Constants;
using Shared.DataTypes;
using System.Collections.Generic;
using System.Linq;

namespace Aqua.Core.GUI.Service.Localization
{
    /// <summary>
    /// Triển khai Dịch thuật & Thay thế tham số động {Key} theo chuẩn /skill 31
    /// </summary>
    public class ErrorLocalizer : IErrorLocalizer
    {
        private static readonly Dictionary<string, string> DefaultTranslations = new()
        {
            [DomainErrorCodes.ListView.ConfigSectionMissing] = "Không tìm thấy cấu hình ListViewSettings trong file appsettings.json.",
            [DomainErrorCodes.ListView.ViewKeyRequired] = "Thuộc tính ViewKey bắt buộc phải được khai báo trên Component.",
            [DomainErrorCodes.ListView.InvalidPageSize] = "Kích thước trang (PageSize) không hợp lệ.",
            [DomainErrorCodes.System.FatalException] = "Đã xảy ra lỗi hệ thống: {ExceptionMessage}. Vui lòng thử lại sau.",
            [DomainErrorCodes.System.Unauthorized] = "Bạn không có quyền truy cập chức năng này.",
            [DomainErrorCodes.System.Forbidden] = "Truy cập bị từ chối.",
            [DomainErrorCodes.Common.NotFound] = "Không tìm thấy dữ liệu yêu cầu.",
            [DomainErrorCodes.Common.InvalidInput] = "Dữ liệu đầu vào không hợp lệ."
        };

        private readonly Dictionary<string, string> _customTranslations;

        public ErrorLocalizer(Dictionary<string, string>? customTranslations = null)
        {
            _customTranslations = customTranslations ?? new Dictionary<string, string>();
        }

        public string GetLocalizedMessage(BOProcessResult result)
        {
            if (result == null || result.IsSuccess) return string.Empty;
            return GetLocalizedMessage(result.Code ?? string.Empty, result.Args);
        }

        public string GetLocalizedMessage(string errorCode, Dictionary<string, object>? args = null)
        {
            if (string.IsNullOrWhiteSpace(errorCode)) return string.Empty;

            string messageTemplate;
            if (_customTranslations.TryGetValue(errorCode, out var customMsg))
            {
                messageTemplate = customMsg;
            }
            else if (DefaultTranslations.TryGetValue(errorCode, out var defaultMsg))
            {
                messageTemplate = defaultMsg;
            }
            else
            {
                // Fallback nếu không tìm thấy key: trả về mã lỗi
                messageTemplate = $"[{errorCode}]";
            }

            if (args != null && args.Any())
            {
                foreach (var kvp in args)
                {
                    string placeholder = $"{{{kvp.Key}}}";
                    string valueStr = kvp.Value?.ToString() ?? string.Empty;
                    messageTemplate = messageTemplate.Replace(placeholder, valueStr);
                }
            }

            return messageTemplate;
        }
    }
}

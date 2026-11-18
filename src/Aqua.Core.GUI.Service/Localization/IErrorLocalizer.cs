using Shared.DataTypes;
using System.Collections.Generic;

namespace Aqua.Core.GUI.Service.Localization
{
    /// <summary>
    /// Contract dịch mã lỗi đa ngôn ngữ tại tầng GUI (/skill 31)
    /// </summary>
    public interface IErrorLocalizer
    {
        string GetLocalizedMessage(BOProcessResult result);
        string GetLocalizedMessage(string errorCode, Dictionary<string, object>? args = null);
    }
}

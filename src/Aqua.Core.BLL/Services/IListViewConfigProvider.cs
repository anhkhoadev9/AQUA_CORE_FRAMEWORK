using Shared.GUI.Contracts;

namespace Shared.GUI.Services
{
    /// <summary>
    /// Contract cung cấp cấu hình ListView theo ViewKey (Provider Pattern)
    /// </summary>
    public interface IListViewConfigProvider
    {
        ViewSpecificConfig GetConfiguration(string viewKey);
    }
}

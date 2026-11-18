using Shared.DataTypes;
using Shared.GUI.Contracts;
using System.Threading.Tasks;

namespace Aqua.Core.GUI.Service.Services
{
    /// <summary>
    /// GUI Service điều phối khởi tạo cấu hình và tạo GridDataRequest cho Presentation Layer
    /// </summary>
    public interface IListViewGuiService
    {
        ViewSpecificConfig GetConfiguration(string viewKey);
        Task<BOProcessResult<GridDataRequest>> GetInitialRequestAsync(string viewKey);
    }
}

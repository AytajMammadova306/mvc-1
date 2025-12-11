using Microsoft.CodeAnalysis.FlowAnalysis.DataFlow;
using Pronia_self.ViewModels;

namespace Pronia_self.Services.Interfaces
{
    public interface ILayoutService
    {
        Task<Dictionary<string, string>> GetSettingAsync();
        Task<BasketVM> GetBasketVMAsync();
    }
}

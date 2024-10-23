using Microsoft.AspNetCore.Mvc.Rendering;
using PCNW.Models.ResponseContracts;
using PCNW.ViewModel;

namespace PCNW.Data.Repository
{
    public interface IImplementRepository
    {
        List<AutoCompleteViewModel> AutoCompleteAsync(string prefix);
        Int64 GetProjectCodeAsync();
        CopyCenterMemberInfo GetCompanyAddress(int Id);
        Task<List<SelectListItem>> GetStates(string SelectedValue = "");
        Task<List<SelectListItem>> GetDistinctState(string SelectedValue = "");
        Task<List<SelectListItem>> GetDistinctStateOfTab(int SelectedTab = 0, DisplayLoginInfo info = null, string SelectedValue = "");
    }
}

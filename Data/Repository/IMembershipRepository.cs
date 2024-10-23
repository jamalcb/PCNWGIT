using Microsoft.AspNetCore.Mvc.Rendering;
using PCNW.Models;
using PCNW.Models.ContractModels;
using PCNW.Models.ResponseContracts;
using PCNW.ViewModel;

namespace PCNW.Data.Repository
{
    public interface IMembershipRepository
    {
        Task<List<SelectListItem>> GetStates(string SelectedValue = "");
        Task<List<SelectListItem>> GetMemberDivisionAsync(string SelectedValue = "");
        Task<dynamic> AddDivisionAsync(MemberShipRegistration model);
        Task<dynamic> UpdateDivisionAsync(MemberShipRegistration model);
        Task<dynamic> GetstrMessage(MemberShipRegistration model);
        string GetSelectedText(string SelectedValue);
        string GetSelectedStateText(string SelectedValue);
        int GetWebDiv(string SelectedValue);
        Task<dynamic> RegisterMembershipAsync(MemberShipRegistration model);
        Task<dynamic> AddContactAsync(MemberShipRegistration model);
        Task<dynamic> UpdateMemberDivAsync(MemberShipRegistration model);
        Task<FindProjectModel> GetDashboardProjectsAsync(DisplayLoginInfo info);
        Task<IEnumerable<ProjectInformation>> GetMemberDashboardProjectsAsync(string username);
        Task<List<TblProjectPreview>> GetProjectdetail(int id);
        Task<MemberShipRegistration> GetMemberProfileAsync(int id, int ConId, string UserName);
        Task<dynamic> autoRenewOn(string sId, bool val);
        Task<dynamic> autoRenewOff(string sId, bool val);
        Task<dynamic> activemember(string sId, bool val);
        Task<dynamic> inactivemember(string sId, bool val);
        Task<IEnumerable<ProjectInformation>> GetMemberDashboardSearchProjectsAsync(string searchTxt, string usern);
        Task<dynamic> EditMemberProfile(MemberShipRegistration model);
        Task<dynamic> AddUserNew(MemberShipRegistration model);
        Task<IEnumerable<ProjectInformation>> GetSearchFindProjectsAsync(string searchTxt);
        Task<IEnumerable<MemberShipRegistration>> GetMembersSearchAsync(string SearchTxt);
        Task<dynamic> UpdateNote(NoteInfo model);
        Task<dynamic> AddNote(NoteInfo model);
        Task<int> DeleteNote(int NoteId);
        Task<dynamic> AddToDashboard(bool Change, string Id, string usern);
        Task<IEnumerable<PaymentInfo>> FindPriceDetail();
        Task<List<StaffDashboardViewModel>> GetProjectDashboardInfoAsync(string username);
        Task<List<string>> ListPdfContent(int ProjId);
        Task<dynamic> SaveProjectInfoAsync(MemberProjectInfo model);
        Task<FindProjectModel> GetSortByStateFindProjectsAsync(List<ProjectInformation> project, SearchViewModel model, int from, DisplayLoginInfo info);
        Task<List<string>> AddendaListPdfContent(int ProjId);
        Task<dynamic> AddToCalendar(int ProjId, DateTime BidDate, DisplayLoginInfo model, bool Ischecked);
        Task<dynamic> SaveSearch(SearchViewModel Model, DisplayLoginInfo loginInfo);
        Task<List<SearchViewModel>> GetSavedSearch(DisplayLoginInfo loginInfo);
        Task<SearchViewModel> GoToSavedSearch(int id);
        Task<dynamic> SaveInCompleteSignUpAsync(MemberShipRegistration model);
        Task<dynamic> DeleteInCompleteSignUp(int id);
        Task<List<OrderTables>> GetCopyCenterData(int ConId);
        Task<AutoFillData> AutoFill(int ConId);
        Task<IEnumerable<MemberShipRegistration>> GetMemberDirectoryAsync();
        Task<dynamic> ShowCard(int Id);
        Task<dynamic> SaveFreeTrialMemberAsync(MemberShipRegistration model);
        Task<IEnumerable<ProjectInformation>> GetProjectUpdatePacificAsync(int ConId);
        Task<IEnumerable<ProjectInformation>> GetProjectUpdateAsync(List<int> Pids, int ConId);
        Task<List<SelectListItem>> GetMemberLocAsync(int MemId, string SelectedValue = "");
        Task<dynamic> EditUser(MemberShipRegistration model);
        Task<dynamic> RadiusSearchAsync(SearchViewModel model, string username);
        Task<IEnumerable<TblFAQ>> GetFaqAsync();
        Task<MemberShipRegistration> GetTrialMemberAsync(int id);
        Task<MemberShipRegistration> UpdateTrialMembershipAsync(MemberShipRegistration model);
        Task<dynamic> UpdateInCompleteSignUpAsync(MemberShipRegistration model, int id);
        Task<MemberDashboard> GetMemberDashboardProjects(DisplayLoginInfo info);
        Task<dynamic> RemoveFromDashboardProj(int ProjId, DisplayLoginInfo info);
        Task<dynamic> AddBiddingProj(string Id, DisplayLoginInfo info);
        Task<dynamic> EditSaveSearch(SearchViewModel Model, DisplayLoginInfo loginInfo);
        Task<dynamic> DeleteSaveSearchAsync(int id);
        Task<List<TblProject>> GetFilteredProjectsAsync(DisplayLoginInfo info);
        Task<List<TblProject>> GetFilteredProjectsData(SearchViewModel model);
    }
}
using PCNW.Models.ContractModels;

namespace PCNW.Data.Repository
{
    public interface IStaffRepository
    {
        Task<List<OrderTables>> GetDashboardProjectsAsync(string returnUrl = "");
        Task<dynamic> UpdateDoneDt(string Id, OrderTables data);
        Task<dynamic> UpdateShipDt(string Id, OrderTables model);
        Task<dynamic> UpdateViewed(string Id);
        Task<List<string>> ViewDoc(string Id);
        Task<dynamic> ChangePublish(bool Change, string Id);
        Task<dynamic> SaveCopyCenterInfoAsync(OrderTables model);
        Task<dynamic> DeleteProject(int Id);
        IEnumerable<MemberShipRegistration> GetContractorArchitect();
        //IEnumerable<MemberShipRegistration> GetArchitect();
        IEnumerable<EntityTypeViewModel> GetEntity();
        Task<MemberShipRegistration> GetContractorProfileAsync(int id);
        Task<dynamic> EditContractorProfileAsync(MemberShipRegistration model);
        Task<dynamic> AddNewUserAsync(MemberShipRegistration model);
        Task<MemberShipRegistration> GetArchitectProfileAsync(int id);
        Task<dynamic> UpdateArchitectProfileAsync(MemberShipRegistration model);
        Task<dynamic> GetViewOrderDocAsync(int OrderId);
        Task<dynamic> BidResultAsync(int ProjId, string name);
        IEnumerable<MemberShipRegistration> GetMembers();
        Task<dynamic> DeleteUserManagementAsync(int Id);
        Task<dynamic> SaveRenewalPaymentAsync(MemberShipRegistration model);
        Task<dynamic> SaveErrorRenewalPaymentAsync(MemberShipRegistration model);
        Task<MemberManagement> GetMemberManagementData();
        Task<MemberManagement> GetOtherTabsData(int page, int pageSize, string searchTerm);
        Task<bool> GetContactData(string Email);
        Task<dynamic> DeleteMemberAsync(int id);
        Task<dynamic> GetPKGListAsync();
        Task<dynamic> GetPkgDetailsAsync(string pkg, string term);
        Task<dynamic> SaveNewRegMemberAsync(MemberShipRegistration model);
        Task<dynamic> RegContractor(MemberShipRegistration model);
        Task<dynamic> DeleteInactiveMemberAsync(int id);
        Task<dynamic> DeleteContractorAsync(int id);
        Task<dynamic> DeleteArchitectAsync(int id);
        Task<dynamic> UpdateGracePeriodAsync(MemberShipRegistration model);
        Task<MemberManagement> GetEntitiesData();
        Task<MemberManagement> GetOtherTabsSearchData(string searchText);
    }
}

using PCNW.Models;
using PCNW.Models.ContractModels;
using PCNW.ViewModel;

namespace PCNW.Data.Repository
{
    public interface ICommonRepository
    {
        Task<int> GetCurrentBiddingProject();
        string CheckUniqueEmail(string email);
        DisplayLoginInfo GetUserInfo(string email);
        Task<List<MemberDashboardViewModel>> GetDashboardInfo(string username);
        Task<dynamic> GetLogActivityAsync(LoginViewModel model);
        Task<dynamic> GetLogOutActivityAsync(string name, bool IsAutoLogout);
        Task<dynamic> GetCopyCenterPriceDetail();
        Task<dynamic> Reorder(int OrderId);
        Task<dynamic> SaveDirectory(TblDirectoryCheck model);
        Task<dynamic> SaveLic(List<int> lstState, string LicNum, string LicDesc, int MemId);
        Task<dynamic> AddLocation(TblLocList model);
        Task<DisplayLoginInfo> GetUserInfoAsync(string email);
        Task<dynamic> SaveProjectInfoAsync(MemberProjectInfo model);
        Task<dynamic> ConfirmBiddingProj(string Id, string usern, string status);
        Task<dynamic> UpdateDirectoryAsync(MemberShipRegistration model);
        Task<dynamic> AdminUserContactAsync(MemberShipRegistration model);
        Task<dynamic> MemberUserDailyReportAsync(MemberShipRegistration model);
        Task<tblIncompleteSignUp> GetSignUpData(int Id);
        Task<TblProject> GetProjByNumber(string projNo);
        Task<TblAddenda> CheckAddenda(string AddendaNo, int ProjId, string ParentFolder, int ParentId);
        Task<tblSubAddenda> CheckSubAddenda(tblSubAddenda tblSub);
        Task<dynamic> PopulateAddenda(TblAddenda tblAdd);
        Task<dynamic> PopulateSubAddenda(tblSubAddenda tblSub);
        Task<List<tblPaymentCardDetail>> GetPacificCardDetailsAsync();
        Task<dynamic> GetSpecialMsgAsync();
        Task<dynamic> GetSpecialMsgMainAsync();
        Task<dynamic> CheckStateAsync(string State);
        Task<dynamic> SendProjectFilesAsync(MemberProjectInfo model);
        Task<dynamic> GetCopyCenterPriceListAsync();
        Task<dynamic> GetDeliveryListAsync();
        Task<MemberShipRegistration> GetMemberAsync(int id);
        Task UpdateMemberMemId(int id, int memId);
        Task<dynamic> PasswordUpdateAsync(Resetpassword resetpassword);
        Task<dynamic> SaveStaffMember(StaffManageViewModel model);
        Task<dynamic> GetStaffMember();
        Task<dynamic> GetEditData(int ConId);
    }
}

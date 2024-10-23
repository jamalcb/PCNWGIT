using PCNW.Models;
using PCNW.Models.ContractModels;
using PCNW.ViewModel;

namespace PCNW.Data.Repository
{
    public interface IGlobalRepository
    {
        Task<List<SuperAdminViewModel>> GetRenewProj();
        Task<List<SuperAdminViewModel>> GetRenewProjRange(DateTime startDate, DateTime endDate);
        Task<List<SuperAdminViewModel>> GetRenewProjMonth(string month, string year);
        Task<IEnumerable<PaymentInfo>> GetMembershipPackage();
        Task<dynamic> SavePackageData(PaymentInfo model);
        Task<IEnumerable<CountyDescriptionViewModel>> GetPackage();
        Task<IEnumerable<TblCounty>> GetCounties();
        Task<List<int?>> OnPackageSelect(int MemberType);
        Task<dynamic> UpdateCounty(List<ManageCountyViewModel> model, string Package);
        Task<IEnumerable<CountyDescriptionViewModel>> GetCountyText();
        Task<dynamic> SaveStateText(CountyDescriptionViewModel model);
        Task<IEnumerable<CopyCenterAdminViewModel>> GetCopyCenterDetail();
        Task<dynamic> EditPageSize(CopyCenterAdminViewModel model);
        Task<dynamic> SavePageSize(CopyCenterAdminViewModel model);
        Task<dynamic> EditPagePrice(CopyCenterAdminViewModel model);
        Task<IEnumerable<DiscountViewModel>> GetDiscountDetails();
        Task<dynamic> UpdateDiscount(DiscountViewModel model);
        Task<dynamic> SaveDiscount(DiscountViewModel model);
        Task<dynamic> SaveBulletPoints(CountyDescriptionViewModel model);
        Task<IEnumerable<DailyMailerViewModel>> DailyMailer();
        Task<dynamic> DailyMailerListAsync();
        Task<dynamic> UpdateDailyMailer(DailyMailerViewModel model);
        Task<dynamic> SaveDailyMailer(DailyMailerViewModel model);
        Task<dynamic> RemoveImage(int Id, string path);
        Task<dynamic> GetAllCopyCenterSizeListAsync();
        Task<dynamic> GetActiveSizeListAsync();
    }
}

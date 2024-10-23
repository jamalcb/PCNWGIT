using PCNW.Models;
using PCNW.Models.ContractModels;

namespace PCNW.Data.Repository
{
    public interface IGlobalMasterRepository
    {
        Task<dynamic> GetProjectTypeAsync();
        Task<dynamic> HasMaster(int id);
        Task<dynamic> SaveProjectTypeAsync(ProjectTypeViewModel model);
        Task<dynamic> UpdateProjectTypeAsync(ProjectTypeViewModel model);
        Task<dynamic> DeleteProjectTypeAsync(int id);
        Task<dynamic> GetProjectSubTypeAsync();
        Task<dynamic> HasMaster1(int id);
        Task<dynamic> SaveProjectSubTypeAsync(ProjectSubTypeViewModel model);
        Task<dynamic> UpdateProjectSubTypeAsync(ProjectSubTypeViewModel model);
        Task<dynamic> DeleteProjectSubTypesync(int id);
        Task<dynamic> GetEntityTypeAsync();
        Task<dynamic> HasMaster2(int id);
        Task<dynamic> SaveEntityTypeAsync(EntityTypeViewModel model);
        Task<dynamic> UpdateEntityTypeAsync(EntityTypeViewModel model);
        Task<dynamic> DeleteEntityTypeAsync(int id);
        Task<dynamic> GetPHLTypeAsync();
        Task<dynamic> HasMaster3(int id);
        Task<dynamic> SavePHLTypeAsync(PHLTypeViewModel model);
        Task<dynamic> UpdatePHLTypeAsync(PHLTypeViewModel model);
        Task<dynamic> DeletePHLTypeAsync(int id);

        Task<dynamic> GetProjNotificationAsync();
        Task<dynamic> SaveProjNotificationAsync(ProjNotificationViewModel model);
        Task<dynamic> DeleteProjNotificationAsync(int id);

        Task<dynamic> GetMemberSignUpAsync();
        Task<dynamic> SaveMemberSignUpAsync(MemberSignUpViewModel model);
        Task<dynamic> DeleteMemberSignUpAsync(int id);

        Task<dynamic> GetPrintOrderAsync();
        Task<dynamic> SavePrintOrderAsync(PrintOrderViewModel model);
        Task<dynamic> DeletePrintOrderAsync(int id);

        Task<dynamic> GetPHLUpdateAsync();
        Task<dynamic> SavePHLUpdateAsync(PHLUpdateViewModel model);
        Task<dynamic> DeletePHLUpdateAsync(int id);

        Task<dynamic> GetMembershipExpireAsync();
        Task<dynamic> SaveMembershipExpireAsync(MembershipExpireViewModel model);
        Task<dynamic> DeleteMembershipExpireAsync(int id);

        Task<dynamic> SaveLogOffAsync(LogOffViewModel model);
        Task<dynamic> GetLogOffAsync();
        Task<dynamic> UpdateLogOffAsync(LogOffViewModel model);

        Task<dynamic> SaveFaqAsync(FAQViewModel model);
        Task<dynamic> GetFaqAsync();
        Task<dynamic> UpdateFaqAsync(FAQViewModel model);

        Task<dynamic> SaveFileAsync(FileStorageViewModel model);
        Task<dynamic> GetFileAsync();
        Task<dynamic> UpdateFileAsync(FileStorageViewModel model);

        Task<dynamic> SaveCopyCenterAsync(CopyCenterViewModel model);
        Task<dynamic> GetCopyCenterAsync();
        Task<dynamic> UpdateCopyCenterAsync(CopyCenterViewModel model);

        Task<dynamic> GetCareerPostingAsync();
        Task<dynamic> SaveCareerPostingAsync(CareerPostingViewModel model);
        Task<dynamic> UpdateCareerPostingAsync(CareerPostingViewModel model);
        Task<dynamic> DeleteCareerPostingAsync(int Id);

        Task<dynamic> GetHolidaySettingAsync();
        Task<dynamic> SaveHolidaySettingAsync(TblDailyInfoHoliday model);
        Task<dynamic> UpdateHolidaySettingAsync(TblDailyInfoHoliday model);
        Task<dynamic> DeleteHolidaySettingAsync(int Id);

        Task<dynamic> GetLoginReportAsync();

        Task<dynamic> GetSpecialMsgAsync();
        Task<dynamic> SaveSpecialMsgAsync(TblSpecialMsg model);
        Task<dynamic> UpdateSpecialMsgAsync(TblSpecialMsg model);
        Task<dynamic> DeleteSpecialMsgAsync(int id);
        Task<dynamic> GetTabData();
        Task<dynamic> SetTabData(bool SetTab);
        Task<IEnumerable<PaymentInfo>> GetAdditionalPackage();
        Task<dynamic> SaveAddPackageData(PaymentInfo model);
        Task<dynamic> GetDeliveryTypeAsync();
        Task<dynamic> SaveDeliveryTypeAsync(tblDeliveryMaster model);
        Task<dynamic> UpdateDeliveryTypeAsync(tblDeliveryMaster model);
        Task<dynamic> DeleteDeliveryTypeAsync(int id);
        Task<dynamic> GetDeliverySubTypeAsync();
        Task<dynamic> SaveDeliverySubTypeAsync(tblDeliveryOption model);
        Task<dynamic> UpdateDeliverySubTypeAsync(tblDeliveryOption model);
        Task<dynamic> DeleteDeliverySubTypeAsync(int id);
    }
}

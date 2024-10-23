using PCNW.Models;
using PCNW.Models.ContractModels;
using PCNW.Models.ProcessContracts;

namespace PCNW.Services
{
    public interface IEmailServiceManager
    {
        Task<string> GetEmailForRegistration(EmailViewModel emailObj, MemberShipRegistration model);
        Task<string> ChangeBidDateToTracking(EmailViewModel emailObj, ProjectInformation model);
        Task<EmailViewModel> Updateproject(EmailViewModel emailObj, ProjectInformation model);
        Task<EmailViewModel> SiteMaintenance(EmailViewModel emailObj, TblSpecialMsg model);

        Task<dynamic> SendEmail(EmailViewModel model);
        Task<dynamic> SendEmailTrialMember(EmailViewModel model);
        Task<string> SavePrintOrder(EmailViewModel emailObj, OrderTables model);
        Task<string> GetEmailForCreateUser(EmailViewModel useremailObj, OrderTables model);
        Task<dynamic> SendEmailSavePrintOrder(EmailViewModel model);
        Task<string> ReadyForPickup(EmailViewModel emailObj, OrderTables model);
        Task<dynamic> SendEmailForPickup(EmailViewModel model);
        Task<string> UploadPostProject(EmailViewModel emailObj, MemberProjectInfo model);
        Task<string> CompleteSendNotice(EmailViewModel emailObj, OrderTables data);
        Task<dynamic> SendForwardMailAsync(EmailViewModel model);
    }
}

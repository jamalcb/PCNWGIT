using PCNW.Models.ContractModels;
using PCNW.ViewModel;

namespace PCNW.Data.Repository
{
    public interface IReportRepository
    {
        Task<IEnumerable<AllMembers>> GetAllMembers();
        Task<IEnumerable<ArchEngineerViewModel>> GetArchEngineers();
        Task<IEnumerable<ContractorViewModel>> GetContractors();
        Task<IEnumerable<ActiveMemberViewModel>> GetActiveMembers();
        Task<IEnumerable<InactiveMemberViewModel>> GetInactiveMembers();
        Task<dynamic> GetMemberSummaryReport();
        Task<dynamic> GetCompanyTypes();
        Task<dynamic> GetSelectedCompanyCount(string MemberType, string SubscriptionLevel = "", string RenewalType = "", string CancellationType = "");
        Task<dynamic> GetNewMemberListAsync(DateTime st, DateTime et);
        Task<dynamic> GetNewMemberDailyAsync();
        Task<dynamic> GetInCompleteMemberAsync();
        Task<dynamic> GetTrialMembersAsync();
        Task<dynamic> GetMemberUsageAsync();
        Task<dynamic> GetSearchReportAsync();
        Task<dynamic> GetDailySubscriptionsAsync();
        Task<dynamic> GetAllCommunicationAsync();
        Task<dynamic> GetCommunicationAsync(string value);
        Task<dynamic> GetExpiringListAsync(DateTime st, DateTime et);
    }
}

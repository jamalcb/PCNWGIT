using PCNW.ViewModel;

namespace PCNW.Data.Repository
{
    public interface IAdminProjectRepository
    {
        Task<ProjectStatsViewModel> GetProjectCounts();
        Task<ProjectStatsViewModel> GetProjectCountsRange(DateTime st, DateTime et);
        Task<ProjectStatsViewModel> GetProjectStatsDaily();
        Task<List<UnpublishedProject>> GetUnpublishedProject();

        Task<List<ActiveProject>> GetActiveProject();

        Task<List<ActiveProject>> GetPastProject();

    }
}

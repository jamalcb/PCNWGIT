using Microsoft.EntityFrameworkCore;
using PCNW.Models;
using PCNW.ViewModel;
using System.Data;

namespace PCNW.Data.Repository
{
    public class AdminProjectRepository : IAdminProjectRepository
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<AdminProjectRepository> _logger;
        private readonly string _connectionString;
        public string url;

        public AdminProjectRepository(ApplicationDbContext dbContext, ILogger<AdminProjectRepository> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
            _connectionString = _dbContext.Database.GetDbConnection().ConnectionString;
            url = "";
        }
        /// <summary>
        /// Get Values of count in project statistics(ProjectStats) page
        /// </summary>
        /// <returns></returns>
        public async Task<ProjectStatsViewModel> GetProjectCounts()
        {
            ProjectStatsViewModel model = new();
            try
            {
                model.ActiveCount = await _dbContext.tblProject.Where(x => x.IsActive != false && x.BidDt >= DateTime.Now).CountAsync();
                model.ArchiveCount = await _dbContext.tblProject.Where(x => (x.IsActive == false || x.IsActive == null) && x.BidDt <= DateTime.Now).CountAsync();
                model.BActiveCount = await _dbContext.tblProject.Where(x => x.IsActive != false && x.BidDt >= DateTime.Now && x.ProjTypeId == 1).CountAsync();
                model.BArchiveCount = await _dbContext.tblProject.Where(x => (x.IsActive == false || x.IsActive == null) && x.BidDt <= DateTime.Now && x.ProjTypeId == 1).CountAsync();
                model.UActiveCount = await _dbContext.tblProject.Where(x => x.IsActive != false && x.BidDt >= DateTime.Now && x.ProjTypeId == 2).CountAsync();
                model.UArchiveCount = await _dbContext.tblProject.Where(x => (x.IsActive == false || x.IsActive == null) && x.BidDt <= DateTime.Now && x.ProjTypeId == 2).CountAsync();
                model.RActiveCount = await _dbContext.tblProject.Where(x => x.IsActive != false && x.BidDt >= DateTime.Now && x.ProjTypeId == 3).CountAsync();
                model.RArchiveCount = await _dbContext.tblProject.Where(x => (x.IsActive == false || x.IsActive == null) && x.BidDt <= DateTime.Now && x.ProjTypeId == 3).CountAsync();
                model.VActiveCount = await _dbContext.tblProject.Where(x => x.IsActive != false && x.BidDt >= DateTime.Now && x.ProjTypeId == 4).CountAsync();
                model.VArchiveCount = await _dbContext.tblProject.Where(x => (x.IsActive == false || x.IsActive == null) && x.BidDt <= DateTime.Now && x.ProjTypeId == 4).CountAsync();
                model.AKCount = await _dbContext.tblProject.Where(x => x.LocState == "AK").CountAsync();
                model.CACount = await _dbContext.tblProject.Where(x => x.LocState == "CA").CountAsync();
                model.WACount = await _dbContext.tblProject.Where(x => x.LocState == "WA").CountAsync();
                model.IDCount = await _dbContext.tblProject.Where(x => x.LocState == "ID").CountAsync();
                model.ORCount = await _dbContext.tblProject.Where(x => x.LocState == "OR").CountAsync();
                model.MTCount = await _dbContext.tblProject.Where(x => x.LocState == "MT").CountAsync();

            }
            catch (Exception)
            {

                throw;
            }
            return model;
        }
        /// <summary>
        /// Get list of unpublished project in UnpublishedProjectRpt page under reportprojectcontrller
        /// </summary>
        /// <returns></returns>
        public async Task<List<UnpublishedProject>> GetUnpublishedProject()
        {
            List<UnpublishedProject> model = new List<UnpublishedProject>();
            List<TblProject> tblProjects = new List<TblProject>();
            try
            {
                tblProjects = await _dbContext.tblProject.Where(x => x.IsActive != false && (x.Publish == false || x.Publish == null) && (x.BidDt >= DateTime.Now || x.BidDt == null)).ToListAsync();
                model = tblProjects.Select(x => new UnpublishedProject
                {
                    ProjectId = x.ProjId,
                    ArrivalDate = string.IsNullOrEmpty(x.ArrivalDt.ToString()) ? "" : Convert.ToDateTime(x.ArrivalDt).ToString("MM/dd/yyyy"),
                    ProjectName = x.Title,
                    BidDate = string.IsNullOrEmpty(x.BidDt.ToString()) ? "" : (string.IsNullOrEmpty(x.StrBidDt) ? Convert.ToDateTime(x.BidDt).ToString("MM/dd/yyyy") : Convert.ToDateTime(x.BidDt).ToString("MM/dd/yyyy") + " " + x.StrBidDt),
                }).ToList();
            }
            catch (Exception Ex)
            {

                _logger.LogWarning(Ex.Message);
            }
            return model;
        }
        /// <summary>
        /// Get list of Active project in ActiveProjectRpt page under reportprojectcontrller
        /// </summary>
        /// <returns></returns>
        public async Task<List<ActiveProject>> GetActiveProject()
        {
            List<ActiveProject> model = new List<ActiveProject>();
            List<TblProject> tblProjects = new List<TblProject>();
            try
            {
                tblProjects = await _dbContext.tblProject.Where(x => x.IsActive != false && (x.BidDt >= DateTime.Now || x.BidDt == null)).Take(500).ToListAsync();
                model = tblProjects.Select(x => new ActiveProject
                {
                    ProjectId = x.ProjId,
                    ArrivalDate = string.IsNullOrEmpty(x.ArrivalDt.ToString()) ? "" : Convert.ToDateTime(x.ArrivalDt).ToString("MM/dd/yyyy"),
                    ProjectName = x.Title,
                    BidDate = string.IsNullOrEmpty(x.BidDt.ToString()) ? "" : (string.IsNullOrEmpty(x.StrBidDt) ? Convert.ToDateTime(x.BidDt).ToString("MM/dd/yyyy") : Convert.ToDateTime(x.BidDt).ToString("MM/dd/yyyy") + " " + x.StrBidDt),
                    ProjNumber = string.IsNullOrEmpty(x.ProjNumber) ? "" : x.ProjNumber,
                    AddendaCount = _dbContext.TblAddenda.Where(m => m.ProjId == x.ProjId).Count(),
                    ProjTypeIdString = (x.ProjTypeId != null ? (_dbContext.TblProjType.SingleOrDefault(y => y.ProjTypeId == x.ProjTypeId) != null ? _dbContext.TblProjType.SingleOrDefault(y => y.ProjTypeId == x.ProjTypeId).ProjType : "") : "")
                }).ToList();
            }
            catch (Exception Ex)
            {

                _logger.LogWarning(Ex.Message);
            }
            return model;
        }
        /// <summary>
        /// Get list of Active project in PastProjectRpt page under reportprojectcontrller
        /// </summary>
        /// <returns></returns>
        public async Task<List<ActiveProject>> GetPastProject()
        {
            List<ActiveProject> model = new List<ActiveProject>();
            List<TblProject> tblProjects = new List<TblProject>();
            try
            {
                tblProjects = await _dbContext.tblProject.Where(x => x.BidDt < DateTime.Now).Take(200).ToListAsync();
                model = tblProjects.Select(x => new ActiveProject
                {
                    ProjectId = x.ProjId,
                    ArrivalDate = string.IsNullOrEmpty(x.ArrivalDt.ToString()) ? "" : Convert.ToDateTime(x.ArrivalDt).ToString("MM/dd/yyyy"),
                    ProjectName = x.Title,
                    BidDate = string.IsNullOrEmpty(x.BidDt.ToString()) ? "" : (string.IsNullOrEmpty(x.StrBidDt) ? Convert.ToDateTime(x.BidDt).ToString("MM/dd/yyyy") : Convert.ToDateTime(x.BidDt).ToString("MM/dd/yyyy") + " " + x.StrBidDt),
                    ProjNumber = string.IsNullOrEmpty(x.ProjNumber) ? (x.PlanNo == null ? "" : x.PlanNo.ToString()) : x.ProjNumber,
                    AddendaCount = _dbContext.TblAddenda.Where(m => m.ProjId == x.ProjId).Count(),
                    ProjTypeIdString = (x.ProjTypeId != null ? (_dbContext.TblProjType.SingleOrDefault(y => y.ProjTypeId == x.ProjTypeId) != null ? _dbContext.TblProjType.SingleOrDefault(y => y.ProjTypeId == x.ProjTypeId).ProjType : "") : "")
                }).ToList();
            }
            catch (Exception Ex)
            {

                _logger.LogWarning(Ex.Message);
            }
            return model;
        }
        /// <summary>
        /// Filter Project by date range on ProjectStats page on report project controller
        /// </summary>
        /// <param name="st"></param>
        /// <param name="et"></param>
        /// <returns></returns>
        public async Task<ProjectStatsViewModel> GetProjectCountsRange(DateTime st, DateTime et)
        {
            ProjectStatsViewModel model = new();
            try
            {
                model.ActiveCount = await _dbContext.tblProject.Where(x => x.IsActive != false && x.BidDt >= DateTime.Now).CountAsync();
                model.ArchiveCount = await _dbContext.tblProject.Where(x => (x.IsActive == false || x.IsActive == null) && x.BidDt <= DateTime.Now).CountAsync();
                model.BActiveCount = await _dbContext.tblProject.Where(x => x.IsActive != false && x.BidDt >= DateTime.Now && x.ProjTypeId == 1 && (x.ArrivalDt != null && x.ArrivalDt >= st && x.ArrivalDt <= et)).CountAsync();
                model.BArchiveCount = await _dbContext.tblProject.Where(x => (x.IsActive == false || x.IsActive == null) && x.BidDt <= DateTime.Now && x.ProjTypeId == 1 && (x.ArrivalDt != null && x.ArrivalDt >= st && x.ArrivalDt <= et)).CountAsync();
                model.UActiveCount = await _dbContext.tblProject.Where(x => x.IsActive != false && x.BidDt >= DateTime.Now && x.ProjTypeId == 2 && (x.ArrivalDt != null && x.ArrivalDt >= st && x.ArrivalDt <= et)).CountAsync();
                model.UArchiveCount = await _dbContext.tblProject.Where(x => (x.IsActive == false || x.IsActive == null) && x.BidDt <= DateTime.Now && x.ProjTypeId == 2 && (x.ArrivalDt != null && x.ArrivalDt >= st && x.ArrivalDt <= et)).CountAsync();
                model.RActiveCount = await _dbContext.tblProject.Where(x => x.IsActive != false && x.BidDt >= DateTime.Now && x.ProjTypeId == 3 && (x.ArrivalDt != null && x.ArrivalDt >= st && x.ArrivalDt <= et)).CountAsync();
                model.RArchiveCount = await _dbContext.tblProject.Where(x => (x.IsActive == false || x.IsActive == null) && x.BidDt <= DateTime.Now && x.ProjTypeId == 3 && (x.ArrivalDt != null && x.ArrivalDt >= st && x.ArrivalDt <= et)).CountAsync();
                model.VActiveCount = await _dbContext.tblProject.Where(x => x.IsActive != false && x.BidDt >= DateTime.Now && x.ProjTypeId == 4 && (x.ArrivalDt != null && x.ArrivalDt >= st && x.ArrivalDt <= et)).CountAsync();
                model.VArchiveCount = await _dbContext.tblProject.Where(x => (x.IsActive == false || x.IsActive == null) && x.BidDt <= DateTime.Now && x.ProjTypeId == 4 && (x.ArrivalDt != null && x.ArrivalDt >= st && x.ArrivalDt <= et)).CountAsync();
                model.AKCount = await _dbContext.tblProject.Where(x => x.LocState == "AK" && (x.ArrivalDt != null && x.ArrivalDt >= st && x.ArrivalDt <= et)).CountAsync();
                model.CACount = await _dbContext.tblProject.Where(x => x.LocState == "CA" && (x.ArrivalDt != null && x.ArrivalDt >= st && x.ArrivalDt <= et)).CountAsync();
                model.WACount = await _dbContext.tblProject.Where(x => x.LocState == "WA" && (x.ArrivalDt != null && x.ArrivalDt >= st && x.ArrivalDt <= et)).CountAsync();
                model.IDCount = await _dbContext.tblProject.Where(x => x.LocState == "ID" && (x.ArrivalDt != null && x.ArrivalDt >= st && x.ArrivalDt <= et)).CountAsync();
                model.ORCount = await _dbContext.tblProject.Where(x => x.LocState == "OR" && (x.ArrivalDt != null && x.ArrivalDt >= st && x.ArrivalDt <= et)).CountAsync();
                model.MTCount = await _dbContext.tblProject.Where(x => x.LocState == "MT" && (x.ArrivalDt != null && x.ArrivalDt >= st && x.ArrivalDt <= et)).CountAsync();

            }
            catch (Exception)
            {

                throw;
            }
            return model;
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <returns></returns>
        public async Task<ProjectStatsViewModel> GetProjectStatsDaily()
        {
            ProjectStatsViewModel model = new();
            DateTime dt = DateTime.Now.Date;
            try
            {
                model.ActiveCount = await _dbContext.tblProject.Where(x => x.IsActive != false && x.BidDt >= DateTime.Now).CountAsync();
                model.ArchiveCount = await _dbContext.tblProject.Where(x => (x.IsActive == false || x.IsActive == null) && x.BidDt <= DateTime.Now).CountAsync();
                model.BActiveCount = await _dbContext.tblProject.Where(x => x.IsActive != false && x.BidDt >= DateTime.Now && x.ProjTypeId == 1 && (x.ArrivalDt != null && x.ArrivalDt >= dt)).CountAsync();
                model.BArchiveCount = await _dbContext.tblProject.Where(x => (x.IsActive == false || x.IsActive == null) && x.BidDt <= DateTime.Now && x.ProjTypeId == 1 && (x.ArrivalDt != null && x.ArrivalDt >= dt)).CountAsync();
                model.UActiveCount = await _dbContext.tblProject.Where(x => x.IsActive != false && x.BidDt >= DateTime.Now && x.ProjTypeId == 2 && (x.ArrivalDt != null && x.ArrivalDt >= dt)).CountAsync();
                model.UArchiveCount = await _dbContext.tblProject.Where(x => (x.IsActive == false || x.IsActive == null) && x.BidDt <= DateTime.Now && x.ProjTypeId == 2 && (x.ArrivalDt != null && x.ArrivalDt >= dt)).CountAsync();
                model.RActiveCount = await _dbContext.tblProject.Where(x => x.IsActive != false && x.BidDt >= DateTime.Now && x.ProjTypeId == 3 && (x.ArrivalDt != null && x.ArrivalDt >= dt)).CountAsync();
                model.RArchiveCount = await _dbContext.tblProject.Where(x => (x.IsActive == false || x.IsActive == null) && x.BidDt <= DateTime.Now && x.ProjTypeId == 3 && (x.ArrivalDt != null && x.ArrivalDt >= dt)).CountAsync();
                model.VActiveCount = await _dbContext.tblProject.Where(x => x.IsActive != false && x.BidDt >= DateTime.Now && x.ProjTypeId == 4 && (x.ArrivalDt != null && x.ArrivalDt >= dt)).CountAsync();
                model.VArchiveCount = await _dbContext.tblProject.Where(x => (x.IsActive == false || x.IsActive == null) && x.BidDt <= DateTime.Now && x.ProjTypeId == 4 && (x.ArrivalDt != null && x.ArrivalDt >= dt)).CountAsync();
                model.AKCount = await _dbContext.tblProject.Where(x => x.LocState == "AK" && (x.ArrivalDt != null && x.ArrivalDt >= dt)).CountAsync();
                model.CACount = await _dbContext.tblProject.Where(x => x.LocState == "CA" && (x.ArrivalDt != null && x.ArrivalDt >= dt)).CountAsync();
                model.WACount = await _dbContext.tblProject.Where(x => x.LocState == "WA" && (x.ArrivalDt != null && x.ArrivalDt >= dt)).CountAsync();
                model.IDCount = await _dbContext.tblProject.Where(x => x.LocState == "ID" && (x.ArrivalDt != null && x.ArrivalDt >= dt)).CountAsync();
                model.ORCount = await _dbContext.tblProject.Where(x => x.LocState == "OR" && (x.ArrivalDt != null && x.ArrivalDt >= dt)).CountAsync();
                model.MTCount = await _dbContext.tblProject.Where(x => x.LocState == "MT" && (x.ArrivalDt != null && x.ArrivalDt >= dt)).CountAsync();

            }
            catch (Exception)
            {

                throw;
            }
            return model;
        }

    }
}

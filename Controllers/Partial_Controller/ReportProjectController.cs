using Microsoft.AspNetCore.Mvc;
using PCNW.Data.Repository;
using PCNW.Models;
using PCNW.ViewModel;

namespace PCNW.Controllers.Partial_Controller
{
    public partial class ReportController : BaseController
    {
        private readonly IAdminProjectRepository _adminProjectRepository;
        private readonly ILogger<ReportController> _logger;
        public ReportController(IAdminProjectRepository adminProjectRepository, ILogger<ReportController> logger, ApplicationDbContext applicationDbContext, ApplicationDbContext dbContex)
        {
            _adminProjectRepository = adminProjectRepository;
            _logger = logger;
        }
        /// <summary>
        /// To Go to Report/ProjectStats (Project Statistics)
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ProjectStats()
        {
            ProjectStatsViewModel model = new();
            //model = await _adminProjectRepository.GetProjectCountsRange(DateTime.Now, DateTime.Now);
            model = await _adminProjectRepository.GetProjectCounts();
            return View(model);
        }
        /// <summary>
        /// Filter Project by date range on ProjectStats page on report project controller
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<JsonResult> ProjectStatsSelect(string value)
        {
            ProjectStatsViewModel model = new();
            DateTime endTime = DateTime.Now;
            DateTime startTime = value == "Daily" ? DateTime.Now : (value == "Week" ? DateTime.Now.AddDays(-7) : (value == "Month" ? DateTime.Now.AddMonths(-1) : (value == "Year" ? DateTime.Now.AddMonths(-12) : DateTime.Now)));
            model = await _adminProjectRepository.GetProjectCountsRange(startTime, endTime);
            //model = await _adminProjectRepository.GetProjectCounts();
            return Json(model);
        }
        /// <summary>
        /// No Use
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<JsonResult> ProjectStatsDaily(string value)
        {
            ProjectStatsViewModel model = new();
            DateTime endTime = DateTime.Now;
            DateTime startTime = value == "Daily" ? DateTime.Now : (value == "Week" ? DateTime.Now.AddDays(-7) : (value == "Month" ? DateTime.Now.AddMonths(-1) : (value == "Year" ? DateTime.Now.AddMonths(-12) : DateTime.Now)));
            model = await _adminProjectRepository.GetProjectStatsDaily();
            //model = await _adminProjectRepository.GetProjectCounts();
            return Json(model);
        }
        /// <summary>
        /// Filter Project by date range on Report/ProjectStats
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<JsonResult> ProjectStatsRange(DateTime startDate, DateTime endDate)
        {
            ProjectStatsViewModel model = new();
            //model = await _adminProjectRepository.GetProjectCountsRange(DateTime.Now, DateTime.Now);
            model = await _adminProjectRepository.GetProjectCountsRange(startDate, endDate);
            return Json(model);
        }
        /// <summary>
        /// To go to Report/UnpublishedProjectRpt (Unpublished Project Report)
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> UnpublishedProjectRpt()
        {
            List<UnpublishedProject> model = new();
            model = await _adminProjectRepository.GetUnpublishedProject();
            return View(model);
        }
        /// <summary>
        /// To go to Report/ActiveProjectRpt (Active Project Report)
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ActiveProjectRpt()
        {
            List<ActiveProject> model = new();
            model = await _adminProjectRepository.GetActiveProject();
            return View(model);
        }
        /// <summary>
        /// To go to Report/PastProjectRpt (Past Project Report)
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> PastProjectRpt()
        {
            List<ActiveProject> model = new();
            model = await _adminProjectRepository.GetPastProject();
            return View(model);
        }

    }
}

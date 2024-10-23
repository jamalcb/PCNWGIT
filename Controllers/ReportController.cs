using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCNW.Data.Repository;
using PCNW.Helpers;
using PCNW.Models.ContractModels;
using PCNW.ViewModel;

namespace PCNW.Controllers
{
    [Authorize(Roles = "Staff,Admin")]
    public class ReportController : BaseController
    {
        private readonly IReportRepository _reportRepository;
        private readonly ILogger<ReportController> _logger;

        public ReportController(IReportRepository reportRepository, ILogger<ReportController> logger)
        {
            _reportRepository = reportRepository;
            _logger = logger;
        }
        /// <summary>
        /// Not use
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// For going to all member screen/page.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> AllMembers()
        {
            IEnumerable<AllMembers> model = (IEnumerable<AllMembers>)await _reportRepository.GetAllMembers();
            return View(model);
            //return View();
        }
        /// <summary>
        /// For going to Architect/Engineers screen/page.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ArchEngineer()
        {
            IEnumerable<ArchEngineerViewModel> model = await _reportRepository.GetArchEngineers();
            return View(model);
            //return View();
        }
        /// <summary>
        /// For going to contractor screen/page.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Contractor()
        {
            IEnumerable<ContractorViewModel> model = (IEnumerable<ContractorViewModel>)await _reportRepository.GetContractors();
            return View(model);
            //return View();
        }
        /// <summary>
        /// For going to activemembers screen/page.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ActiveMembers()
        {
            IEnumerable<ActiveMemberViewModel> model = (IEnumerable<ActiveMemberViewModel>)await _reportRepository.GetActiveMembers();
            return View(model);
            //return View();
        }
        /// <summary>
        /// For going to inactive member screen/page.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> InactiveMembers()
        {
            IEnumerable<InactiveMemberViewModel> model = (IEnumerable<InactiveMemberViewModel>)await _reportRepository.GetInactiveMembers();
            return View(model);
            //return View();
        }
        /// <summary>
        /// Not use
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetMemberSummary()
        {
            HttpResponseDetail<dynamic> response = await _reportRepository.GetMemberSummaryReport();
            return Json(response);
        }
        /// <summary>
        /// Not use
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetCompanyType()
        {
            HttpResponseDetail<dynamic> response = await _reportRepository.GetCompanyTypes();
            return Json(response);
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <param name="MemberType"></param>
        /// <param name="SubscriptionLevel"></param>
        /// <param name="RenewalType"></param>
        /// <param name="CancellationType"></param>
        /// <returns></returns>
        public async Task<JsonResult> GetCompanyCount(string MemberType, string SubscriptionLevel, string RenewalType, string CancellationType)
        {
            HttpResponseDetail<dynamic> response = await _reportRepository.GetSelectedCompanyCount(MemberType, SubscriptionLevel, RenewalType, CancellationType);
            return Json(response);
        }
        #region New Members Report
        public IActionResult NewMembers()
        {
            return View();
        }
        public async Task<JsonResult> NewMemberList(string value)
        {
            HttpResponseDetail<dynamic> model = new();
            DateTime endTime = DateTime.Now;
            DateTime startTime = value == "Daily" ? DateTime.Now : (value == "Week" ? DateTime.Now.AddDays(-7) : (value == "Month" ? DateTime.Now.AddMonths(-1) : (value == "Year" ? DateTime.Now.AddMonths(-12) : DateTime.Now)));
            model = await _reportRepository.GetNewMemberListAsync(startTime, endTime);
            return Json(model);
        }
        public async Task<JsonResult> NewMemberDaily(string value)
        {
            HttpResponseDetail<dynamic> model = new();
            model = await _reportRepository.GetNewMemberDailyAsync();
            return Json(model);
        }
        public async Task<JsonResult> NewMemberRange(DateTime startDate, DateTime endDate)
        {
            HttpResponseDetail<dynamic> model = new();

            model = await _reportRepository.GetNewMemberListAsync(startDate, endDate);
            return Json(model);
        }
        #endregion
        #region InComplete Member
        /// <summary>
        /// To go Report/InCompleteMembers (Incomplete signup)
        /// </summary>
        /// <returns></returns>
        public IActionResult InCompleteMembers()
        {
            return View();
        }
        /// <summary>
        /// To get data for Report/InCompleteMembers (Incomplete signup)
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetInCompleteMemberList()
        {
            return Json(await _reportRepository.GetInCompleteMemberAsync());
        }
        #endregion
        #region Trial Members
        /// <summary>
        /// To go Report/TrialMembers (Free trial Member Report)
        /// </summary>
        /// <returns></returns>
        public IActionResult TrialMembers()
        {
            return View();
        }
        /// <summary>
        /// To get data for Report/TrialMembers (Free trial Member Report)
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetTrialMembersList()
        {
            return Json(await _reportRepository.GetTrialMembersAsync());
        }
        #endregion
        #region Member Usage Report
        /// <summary>
        /// No use
        /// </summary>
        /// <returns></returns>
        public IActionResult MemberUsage()
        {
            return View();
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetMemberUsageList()
        {
            return Json(await _reportRepository.GetMemberUsageAsync());
        }
        #endregion
        #region Search Report
        /// <summary>
        /// To go to Report/SearchReport(Auto Search)
        /// </summary>
        /// <returns></returns>
        public IActionResult SearchReport()
        {
            return View();
        }
        /// <summary>
        /// To get data for to Report/SearchReport
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetSearchReportList()
        {
            return Json(await _reportRepository.GetSearchReportAsync());
        }
        #endregion
        #region Daily Subscriptions
        /// <summary>
        /// To get go to Report/DailySubscriptions
        /// </summary>
        /// <returns></returns>
        public IActionResult DailySubscriptions()
        {
            return View();
        }
        /// <summary>
        /// To get data for Report/DailySubscriptions
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetDailySubscriptionsList()
        {
            return Json(await _reportRepository.GetDailySubscriptionsAsync());
        }
        #endregion
        #region Communication Report
        /// <summary>
        /// To go to Report/Communication
        /// </summary>
        /// <returns></returns>
        public IActionResult Communication()
        {
            return View();
        }
        /// <summary>
        /// To get data for Report/Communication
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<JsonResult> CommunicationList(string value)
        {
            HttpResponseDetail<dynamic> model = new();
            model = await _reportRepository.GetCommunicationAsync(value);
            return Json(model);
        }
        /// <summary>
        /// To get data for all member on Report/Communication
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<JsonResult> AllCommunication(string value)
        {
            HttpResponseDetail<dynamic> model = new();
            model = await _reportRepository.GetAllCommunicationAsync();
            return Json(model);
        }
        /// <summary>
        /// To get data for Expiring member on Report/Communication
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public async Task<JsonResult> ExpiringCommunicationList(string value)
        {
            HttpResponseDetail<dynamic> model = new();
            DateTime startTime = DateTime.Now;
            DateTime endTime = value == "Expiring" ? DateTime.Now.AddMonths(+1) : DateTime.Now;
            model = await _reportRepository.GetExpiringListAsync(startTime, endTime);
            return Json(model);
        }
        #endregion
        /// <summary>
        /// For going to report dashboard screen/page.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Dashboard()
        {
            return View();
            //return View();
        }
    }
}

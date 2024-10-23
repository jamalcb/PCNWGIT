using Microsoft.AspNetCore.Mvc;
using PCNW.Data.Repository;
using PCNW.Helpers;
using PCNW.Models;
using PCNW.Models.ContractModels;
using PCNW.ViewModel;

namespace PCNW.Controllers
{
    public class ImplementController : BaseController
    {
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly IImplementRepository _implementRepository;
        private readonly ICommonRepository _commonRepository;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _environment;
        private readonly ILogger<ImplementController> _logger;
        private readonly IEntityRepository _entityRepository;

        public ImplementController(ApplicationDbContext applicationDbContext, ICommonRepository commonRepository, IImplementRepository implementRepository, Microsoft.AspNetCore.Hosting.IHostingEnvironment environment, ILogger<ImplementController> logger, IEntityRepository entityRepository)
        {
            _applicationDbContext = applicationDbContext;
            _implementRepository = implementRepository;
            _environment = environment;
            _commonRepository = commonRepository;
            _logger = logger;
            _entityRepository = entityRepository;
        }
        public IActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public JsonResult AutoComplete(string prefix)
        {
            HttpResponseDetail<dynamic> response = new();
            var result = _implementRepository.AutoCompleteAsync(prefix);
            response.data = result;
            return Json(response);
        }
        [HttpPost]
        public JsonResult GetProjectCode()
        {
            HttpResponseDetail<dynamic> response = new();
            var result = _implementRepository.GetProjectCodeAsync();
            response.data = result;
            return Json(response);
        }
        /// <summary>
        /// No use just testing purpose
        /// </summary>
        /// <returns></returns>
        public IActionResult UploadPdf()
        {
            return View();
        }
        /// <summary>
        /// Get company name suggestion on print order form of staff copy center and staff preview page
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public JsonResult GetCompanyName(string prefix)
        {
            dynamic result = null;
            if (!string.IsNullOrEmpty(prefix))
            {
                result = (from member in _entityRepository.GetEntities()
                          where member.Company.StartsWith(prefix)
                          select new
                          {
                              label = member.Company,
                              val = member.Id
                          }).ToList();
            }
            else
            {
                result = (from member in _entityRepository.GetEntities()
                          select new
                          {
                              label = member.Company,
                              val = member.Id
                          }).ToList();
            }
            return Json(result);
        }
        /// <summary>
        /// Get company address on preview(staffaccount page) print order form, staff copy center
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public JsonResult GetCompanyAddress(int Id)
        {
            HttpResponseDetail<dynamic> response = new();
            var result = _implementRepository.GetCompanyAddress(Id);
            response.data = result;
            return Json(response);
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetStateList()
        {
            HttpResponseDetail<dynamic> response = new();
            var result = await _implementRepository.GetStates();
            response.data = result;
            return Json(response);
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public JsonResult GetContactName(int Id)
        {
            var response = _implementRepository.GetCompanyAddress(Id);
            List<SelectedInfo> result = new();
            if (Id != 0)
            {
                try
                {
                    result = response.ContactList.Select(x => new SelectedInfo
                    {
                        label = x.Contact,
                        val = x.UID
                    }).ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogInformation(ex.Message);
                }
            }
            else
            {
                result.Add(new SelectedInfo { label = "", val = "0" });
            }
            return Json(result);
        }
        /// <summary>
        /// Get the list of state which is present in active project list on find project here state list and also in geographical search
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetDistinctState()
        {
            HttpResponseDetail<dynamic> response = new();
            var result = await _implementRepository.GetDistinctState();
            response.data = result;
            return Json(response);
        }
        public async Task<JsonResult> GetDistinctStateOfTab(int SelectedTab)
        {
            DisplayLoginInfo info = _commonRepository.GetUserInfo(User.Identity.Name);
            HttpResponseDetail<dynamic> response = new();
            var result = await _implementRepository.GetDistinctStateOfTab(Convert.ToInt32(SelectedTab), info);
            response.data = result;
            return Json(response);
        }
    }
}


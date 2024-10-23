using Microsoft.AspNetCore.Mvc;
using PCNW.Data.Repository;
using PCNW.Helpers;
using PCNW.Models;
using PCNW.Models.ContractModels;
using PCNW.ViewModel;
using System.Globalization;

namespace PCNW.Controllers.Partial_Controller
{

    public partial class GlobalMasterController : BaseController
    {
        private readonly IGlobalRepository _globalRepository;
        private readonly ILogger<GlobalMasterController> _logger;
        private readonly IWebHostEnvironment _Environment;
        public GlobalMasterController(IGlobalRepository globalRepository, ILogger<GlobalMasterController> logger, IWebHostEnvironment Environment)
        {
            _globalRepository = globalRepository;
            _logger = logger;
            _Environment = Environment;
        }
        public async Task<IActionResult> RenewalProjection()
        {
            decimal revenue = 0;
            List<SuperAdminViewModel> model = new();
            model = await _globalRepository.GetRenewProj();
            if (model != null)
            {
                foreach (var item in model)
                {
                    revenue = revenue + Convert.ToDecimal(item.MemberShip);
                }
            }
            string strRevenue = revenue.ToString("N", CultureInfo.InvariantCulture);
            TempData["strRevenue"] = strRevenue;
            return View(model);
        }
        /// <summary>
        /// Filter by radio button
        /// </summary>
        /// <param name="month"></param>
        /// <param name="year"></param>
        /// <returns></returns>
        public async Task<JsonResult> RenewalProjectionMonth(string month, string year)
        {
            decimal revenue = 0;
            HttpResponseDetail<dynamic> response = new();
            List<SuperAdminViewModel> model = new();
            model = await _globalRepository.GetRenewProjMonth(month, year);
            if (model != null)
            {
                foreach (var item in model)
                {
                    revenue = revenue + Convert.ToDecimal(item.MemberShip);
                }
            }
            string strRevenue = revenue.ToString("N", CultureInfo.InvariantCulture);
            response.data = model;
            response.statusMessage = strRevenue;
            return Json(response);
        }
        /// <summary>
        /// Search Projrvtion by start date and end date     
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public async Task<IActionResult> RenewalProjectionRange(DateTime startDate, DateTime endDate)
        {
            decimal revenue = 0;
            List<SuperAdminViewModel> model = new();
            model = await _globalRepository.GetRenewProjRange(startDate, endDate);
            if (model != null)
            {
                foreach (var item in model)
                {
                    revenue = revenue + Convert.ToDecimal(item.MemberShip);
                }
            }
            string strRevenue = revenue.ToString("N", CultureInfo.InvariantCulture);
            TempData["strRevenue"] = strRevenue;
            return View("RenewalProjection", model);
        }
        /// <summary>
        /// Go to view ManagePricing
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ManagePricing()
        {
            IEnumerable<PaymentInfo> model = new List<PaymentInfo>();
            model = await _globalRepository.GetMembershipPackage();
            return View(model);
        }
        /// <summary>
        /// Update package data from View ManagePricing 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> SavePackageData(PaymentInfo model)
        {
            dynamic response = await _globalRepository.SavePackageData(model);
            return Json(response);
        }
        /// <summary>
        /// Return ManageCounties view
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ManageCounties()
        {
            return View();
        }
        /// <summary>
        /// Get Package on load of ManageCounties view for package select
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetPackage()
        {
            IEnumerable<CountyDescriptionViewModel> model = await _globalRepository.GetPackage();
            return Json(model);
        }
        /// <summary>
        /// Get All counties on load of ManageCounties view for package select
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetCounties()
        {
            IEnumerable<TblCounty> model = await _globalRepository.GetCounties();
            return Json(model);
        }
        /// <summary>
        /// Manage counties  package select of ManageCounties view 
        /// </summary>
        /// <param name="MemberType"></param>
        /// <returns></returns>
        public async Task<JsonResult> OnPackageSelect(int MemberType)
        {
            List<int?> model = await _globalRepository.OnPackageSelect(MemberType);
            return Json(model);
        }
        /// <summary>
        /// Update counties for selected package of ManageCounties view 
        /// </summary>
        /// <param name="model"></param>
        /// <param name="Package"></param>
        /// <returns></returns>
        public async Task<JsonResult> UpdateCounty(List<ManageCountyViewModel> obj, string Package)
        {
            HttpResponseDetail<dynamic> response = new();
            int status = await _globalRepository.UpdateCounty(obj, Package);
            if (status == 1)
            {
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Nothing to update";
            }
            else if (status > 1)
            {
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Value updated for " + Package + " successfully";
            }
            else if (status == 0)
            {
                response.success = false;
                response.statusCode = "404";
                response.statusMessage = "Something went wrong";
            }
            return Json(response);
        }
        /// <summary>
        /// Go to ManageCountyContent view
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ManageCountyContent()
        {
            IEnumerable<CountyDescriptionViewModel> model = await _globalRepository.GetCountyText();
            return View(model);
        }
        /// <summary>
        /// Save data from ManageCountyContent view
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> SaveStateText(CountyDescriptionViewModel model)
        {
            dynamic response = await _globalRepository.SaveStateText(model);
            return Json(response);
        }
        /// <summary>
        /// Go to ManageCopyCenterSize View (Copy Center Price)
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ManageCopyCenterSize()
        {
            //IEnumerable<CopyCenterAdminViewModel> model = new List<CopyCenterAdminViewModel>();
            //model = await _globalRepository.GetCopyCenterDetail();
            return View();
        }
        /// <summary>
        /// Get data for ManageCopyCenterSize View
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetAllCopyCenterSizeList()
        {
            return Json(await _globalRepository.GetAllCopyCenterSizeListAsync());
        }
        /// <summary>
        /// Switch Active/Inactive for ManageCopyCenterSize View
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetActiveSizeList()
        {
            return Json(await _globalRepository.GetActiveSizeListAsync());
        }
        /// <summary>
        /// Edit page size from ManageCopyCenterSize View
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> EditPageSize(CopyCenterAdminViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            response = await _globalRepository.EditPageSize(model);
            return Json(response);
        }
        /// <summary>
        /// Save page size from ManageCopyCenterSize View
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> SavePageSize(CopyCenterAdminViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            response = await _globalRepository.SavePageSize(model);
            return Json(response);
        }
        /// <summary>
        /// Go to ManageCopyCenterPricing view (Copy Center Price)
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ManageCopyCenterPricing()
        {
            IEnumerable<CopyCenterAdminViewModel> model = new List<CopyCenterAdminViewModel>();
            model = await _globalRepository.GetCopyCenterDetail();
            return View(model);
        }
        /// <summary>
        /// Edit details from ManageCopyCenterPricing view (Copy Center Price)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> EditPagePrice(CopyCenterAdminViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            response = await _globalRepository.EditPagePrice(model);
            return Json(response);
        }
        /// <summary>
        /// Go to ManageDiscount (Manage Discount) View
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ManageDiscount()
        {
            IEnumerable<DiscountViewModel> model = new List<DiscountViewModel>();
            model = await _globalRepository.GetDiscountDetails();
            return View(model);
        }
        /// <summary>
        /// Update Discount detail from ManageDiscount (Manage Discount) View
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> UpdateDiscount(DiscountViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            response = await _globalRepository.UpdateDiscount(model);
            return Json(response);
        }
        /// <summary>
        /// Add new Discount detail from ManageDiscount View
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> SaveDiscount(DiscountViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            response = await _globalRepository.SaveDiscount(model);
            return Json(response);
        }
        /// <summary>
        /// Go to ManageBulletPoint (Bullet Points) view
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ManageBulletPoint()
        {
            IEnumerable<CountyDescriptionViewModel> model = await _globalRepository.GetCountyText();
            return View(model);
        }
        /// <summary>
        /// Update/save details from ManageBulletPoint (Bullet Points) view 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> SaveBulletPoints(CountyDescriptionViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            response = await _globalRepository.SaveBulletPoints(model);
            return Json(response);
        }
        /// <summary>
        /// Go to DailyMailer View (Daily Mailer)
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> DailyMailer()
        {
            IEnumerable<DailyMailerViewModel> model = new List<DailyMailerViewModel>();
            model = await _globalRepository.DailyMailer();
            return View(model);
        }
        //public async Task<JsonResult> UpdateDailyMaile(DailyMailerViewModel model)
        //{
        //    HttpResponseDetail<dynamic> response = new();
        //    response = await _globalRepository.UpdateDailyMailer(model);
        //    return Json(response);
        //}
        /// <summary>
        /// Save/Update details from  DailyMailer View (Daily Mailer)
        /// </summary>
        /// <param name="model"></param>
        /// <param name="pathChK"></param>
        /// <returns></returns>
        public async Task<JsonResult> SaveDailyMailer(DailyMailerViewModel model, string pathChK)
        {
            HttpResponseDetail<dynamic> response = new();
            if (model.Id > 0)
            {
                response = await _globalRepository.UpdateDailyMailer(model);
                if (response.success == true)
                {


                    if (pathChK != response.data)
                    {
                        string pathPhoto = this._Environment.WebRootPath + pathChK;
                        if (System.IO.File.Exists(pathPhoto))
                            System.IO.File.Delete(pathPhoto);
                    }
                }

            }
            else
            {
                response = await _globalRepository.SaveDailyMailer(model);
            }
            return Json(response);
        }
        /// <summary>
        /// Remove image from DailyMailer View (Daily Mailer)
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="Path"></param>
        /// <returns></returns>
        public async Task<JsonResult> RemoveImage(int Id, string Path)
        {
            HttpResponseDetail<dynamic> response = new();
            response = await _globalRepository.RemoveImage(Id, Path);
            return Json(response);
        }
        /// <summary>
        /// Upload image from DailyMailer View (Daily Mailer)
        /// </summary>
        /// <param name="pdfFile"></param>
        /// <returns></returns>
        public JsonResult UploadPdf(IFormFile pdfFile)
        {
            string pathPhoto = Path.Combine(this._Environment.WebRootPath, "MailerImage");
            string fileName = Path.GetFileName(DateTime.Now.ToString("yyyyMMddHHmmssFFF") + pdfFile.FileName);
            using (FileStream stream = new FileStream(Path.Combine(pathPhoto, fileName), FileMode.Create))
                pdfFile.CopyTo(stream);
            string data = "\\MailerImage\\" + fileName;
            return Json(new { Status = "success", data = data, Flag = 'Y' });
        }
    }
}

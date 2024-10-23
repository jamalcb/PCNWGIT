using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PCNW.Data.Repository;
using PCNW.Helpers;
using PCNW.Models;
using PCNW.Models.ContractModels;
using PCNW.Models.ProcessContracts;
using PCNW.Services;

namespace PCNW.Controllers
{
    [Authorize(Roles = "Admin")]
    public class GlobalMasterController : BaseController
    {
        private readonly IGlobalMasterRepository _globalMasterRepository;
        private readonly ILogger<GlobalMasterController> _logger;

        private readonly IEmailServiceManager _emailServiceManager;

        public GlobalMasterController(IGlobalMasterRepository globalMasterRepository, ILogger<GlobalMasterController> logger, IEmailServiceManager emailServiceManager)
        {
            _globalMasterRepository = globalMasterRepository;
            _logger = logger;
            _emailServiceManager = emailServiceManager;
        }

        #region Project Type Master
        /// <summary>
        /// To go to GlobalMaster/Projecttype page
        /// </summary>
        /// <returns></returns>
        public IActionResult ProjectType()
        {
            return View();
        }
        /// <summary>
        /// To get data on GlobalMaster/Projecttype page
        /// </summary>
        /// <returns></returns>
		public async Task<JsonResult> GetProjectTypeList()
        {
            return Json(await _globalMasterRepository.GetProjectTypeAsync());
        }
        /// <summary>
        /// Save new project type from GlobalMaster/Projecttype page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> SaveProjectType(ProjectTypeViewModel model)
        {
            dynamic response;
            if (model.ProjTypeId > 0)
                response = await _globalMasterRepository.UpdateProjectTypeAsync(model);
            else
                response = await _globalMasterRepository.SaveProjectTypeAsync(model);
            return Json(response);
        }
        /// <summary>
        /// delete projecttype from GlobalMaster/Projecttype page
        /// </summary>
        /// <param name="ProjTypeId"></param>
        /// <returns></returns>
        public async Task<JsonResult> DeleteProjectType(int ProjTypeId)
        {
            return Json(await _globalMasterRepository.DeleteProjectTypeAsync(ProjTypeId));
        }
        #endregion
        #region Project Sub Type Master
        /// <summary>
        /// To go to GlobalMaster/ProjectSubType page
        /// </summary>
        /// <returns></returns>
        public IActionResult ProjectSubType()
        {
            return View();
        }
        /// <summary>
        /// To get data on GlobalMaster/ProjectSubType page
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetProjectSubTypeList()
        {
            return Json(await _globalMasterRepository.GetProjectSubTypeAsync());
        }
        /// <summary>
        /// To save new ProjectSubType from GlobalMaster/ProjectSubType page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> SaveProjectSubType(ProjectSubTypeViewModel model)
        {
            dynamic response;
            if (model.ProjSubTypeID > 0)
                response = await _globalMasterRepository.UpdateProjectSubTypeAsync(model);
            else
                response = await _globalMasterRepository.SaveProjectSubTypeAsync(model);
            return Json(response);
        }
        /// <summary>
        /// To delete ProjectSubType from GlobalMaster/ProjectSubType page
        /// </summary>
        /// <param name="ProjSubTypeID"></param>
        /// <returns></returns>
        public async Task<JsonResult> DeleteProjectSubType(int ProjSubTypeID)
        {
            return Json(await _globalMasterRepository.DeleteProjectSubTypesync(ProjSubTypeID));
        }
        #endregion
        #region Entity Type Master
        /// <summary>
        /// To go to GlobalMaster/EntityType page
        /// </summary>
        /// <returns></returns>
        public IActionResult EntityType()
        {
            return View();
        }
        /// <summary>
        /// To get data on GlobalMaster/EntityType page
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetEntityTypeList()
        {
            return Json(await _globalMasterRepository.GetEntityTypeAsync());
        }
        /// <summary>
        /// To save/update EntityType from GlobalMaster/EntityType page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> SaveEntityType(EntityTypeViewModel model)
        {
            dynamic response;
            if (model.EntityID > 0)
                response = await _globalMasterRepository.UpdateEntityTypeAsync(model);
            else
                response = await _globalMasterRepository.SaveEntityTypeAsync(model);
            return Json(response);
        }
        /// <summary>
        /// To Delete EntityType from GlobalMaster/EntityType page
        /// </summary>
        /// <param name="EntityID"></param>
        /// <returns></returns>
        public async Task<JsonResult> DeleteEntityType(int EntityID)
        {
            return Json(await _globalMasterRepository.DeleteEntityTypeAsync(EntityID));
        }
        #endregion
        #region PHL Type Master
        /// <summary>
        /// To go to on GlobalMaster/PHLType
        /// </summary>
        /// <returns></returns>
        public IActionResult PHLType()
        {
            return View();
        }
        /// <summary>
        /// To get data on GlobalMaster/PHLType
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetPHlTypeList()
        {
            return Json(await _globalMasterRepository.GetPHLTypeAsync());
        }
        /// <summary>
        /// To save/Edit PHLType on GlobalMaster/PHLType
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> SavePHLType(PHLTypeViewModel model)
        {
            dynamic response;
            if (model.PHLID > 0)
                response = await _globalMasterRepository.UpdatePHLTypeAsync(model);
            else
                response = await _globalMasterRepository.SavePHLTypeAsync(model);
            return Json(response);
        }
        /// <summary>
        /// To delete PHLType on GlobalMaster/PHLType
        /// </summary>
        /// <param name="PHLID"></param>
        /// <returns></returns>
        public async Task<JsonResult> DeletePHLType(int PHLID)
        {
            return Json(await _globalMasterRepository.DeletePHLTypeAsync(PHLID));
        }
        #endregion

        #region Upload Project Notification
        /// <summary>
        /// To go to GlobalMaster/ProjectNotification
        /// </summary>
        /// <returns></returns>
        public IActionResult ProjectNotification()
        {
            return View();
        }
        /// <summary>
        /// To Save data from GlobalMaster/ProjectNotification
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> SaveProjNotification(ProjNotificationViewModel model)
        {

            return Json(await _globalMasterRepository.SaveProjNotificationAsync(model));

        }
        /// <summary>
        /// To get data for GlobalMaster/ProjectNotification
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetProjNotificationList()
        {
            return Json(await _globalMasterRepository.GetProjNotificationAsync());
        }
        /// <summary>
        /// To delete data from GlobalMaster/ProjectNotification
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<JsonResult> DeleteProjNotification(int Id)
        {
            return Json(await _globalMasterRepository.DeleteProjNotificationAsync(Id));
        }
        #endregion
        #region Member SignUp 
        /// <summary>
        /// To go to to GlobalMaster/MemberSignUp
        /// </summary>
        /// <returns></returns>
        public IActionResult MemberSignUp()
        {
            return View();
        }
        /// <summary>
        /// To Get data for GlobalMaster/MemberSignUp
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetMemberSignUpList()
        {
            return Json(await _globalMasterRepository.GetMemberSignUpAsync());
        }
        /// <summary>
        /// To Save for GlobalMaster/MemberSignUp
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> SaveMemberSignUp(MemberSignUpViewModel model)
        {
            return Json(await _globalMasterRepository.SaveMemberSignUpAsync(model));
        }
        /// <summary>
        /// To delete data from GlobalMaster/MemberSignUp
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<JsonResult> DeleteMemberSignUp(int Id)
        {
            return Json(await _globalMasterRepository.DeleteMemberSignUpAsync(Id));
        }
        #endregion
        #region Print Order 
        /// <summary>
        /// To go to on GlobalMaster/PrintOrder
        /// </summary>
        /// <returns></returns>
        public IActionResult PrintOrder()
        {
            return View();
        }
        /// <summary>
        /// To Get data on GlobalMaster/PrintOrder
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetPrintOrderList()
        {
            return Json(await _globalMasterRepository.GetPrintOrderAsync());
        }
        /// <summary>
        /// To save data data from GlobalMaster/PrintOrder
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> SavePrintOrder(PrintOrderViewModel model)
        {
            return Json(await _globalMasterRepository.SavePrintOrderAsync(model));
        }
        /// <summary>
        /// To delete data from GlobalMaster/DeletePrintOrder
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<JsonResult> DeletePrintOrder(int Id)
        {
            return Json(await _globalMasterRepository.DeletePrintOrderAsync(Id));
        }
        #endregion
        #region PHL Update 
        /// <summary>
        /// To go to GlobalMaster/PHLUpdate
        /// </summary>
        /// <returns></returns>
        public IActionResult PHLUpdate()
        {
            return View();
        }
        /// <summary>
        /// To get data on GlobalMaster/PHLUpdate
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetPHLUpdateList()
        {
            return Json(await _globalMasterRepository.GetPHLUpdateAsync());
        }
        /// <summary>
        /// To save data from GlobalMaster/PHLUpdate
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> SavePHLUpdate(PHLUpdateViewModel model)
        {
            return Json(await _globalMasterRepository.SavePHLUpdateAsync(model));
        }
        /// <summary>
        /// To delete data from GlobalMaster/PHLUpdate
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<JsonResult> DeletePHLUpdate(int Id)
        {
            return Json(await _globalMasterRepository.DeletePHLUpdateAsync(Id));
        }
        #endregion
        #region Membership Expire 
        /// <summary>
        /// To go to GlobalMaster/MembershipExpire
        /// </summary>
        /// <returns></returns>
        public IActionResult MembershipExpire()
        {
            return View();
        }
        /// <summary>
        /// To get data on GlobalMaster/MembershipExpire
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetMembershipExpireList()
        {
            return Json(await _globalMasterRepository.GetMembershipExpireAsync());
        }
        /// <summary>
        /// To save data on GlobalMaster/MembershipExpire 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> SaveMembershipExpire(MembershipExpireViewModel model)
        {
            return Json(await _globalMasterRepository.SaveMembershipExpireAsync(model));
        }
        /// <summary>
        /// To save data on GlobalMaster/MembershipExpire 
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<JsonResult> DeleteMembershipExpire(int Id)
        {
            return Json(await _globalMasterRepository.DeleteMembershipExpireAsync(Id));
        }
        #endregion
        #region LogOff
        /// <summary>
        /// To go to GlobalMaster/LogOff 
        /// </summary>
        /// <returns></returns>
        public IActionResult LogOff()
        {
            return View();
        }
        /// <summary>
        /// To update/save data from GlobalMaster/LogOff
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> SaveLogOff(LogOffViewModel model)
        {
            dynamic response;
            if (model.Id > 0)
                response = await _globalMasterRepository.UpdateLogOffAsync(model);
            else
                response = await _globalMasterRepository.SaveLogOffAsync(model);
            return Json(response);
        }
        /// <summary>
        /// To get data from autogoff table
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<JsonResult> GetLogOffList()
        {
            return Json(await _globalMasterRepository.GetLogOffAsync());
        }
        #endregion
        #region FAQ
        /// <summary>
        /// To go to GlobalMaster/Faq
        /// </summary>
        /// <returns></returns>
        public IActionResult Faq()
        {
            return View();
        }
        /// <summary>
        /// To save/update data from GlobalMaster/Faq
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<JsonResult> SaveFaq(FAQViewModel model)
        {
            if (!string.IsNullOrEmpty(model.Answer))
            {
                model.Answer = model.Answer
                        .Replace("&", "&amp;")
                        .Replace("\"", "&quot;")
                        .Replace("'", "&#039;")
                        .Replace("<", "&lt;")
                        .Replace(">", "&gt;");
            }
            dynamic response;
            if (model.Id > 0)
                response = await _globalMasterRepository.UpdateFaqAsync(model);
            else
                response = await _globalMasterRepository.SaveFaqAsync(model);
            return Json(response);
        }
        /// <summary>
        /// To get data on GlobalMaster/Faq
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetFaqList()
        {
            return Json(await _globalMasterRepository.GetFaqAsync());
        }
        #endregion
        #region File Storage Control
        /// <summary>
        /// To go to GlobalMaster/PastProject 
        /// </summary>
        /// <returns></returns>
        public IActionResult PastProject()
        {
            return View();
        }
        /// <summary>
        /// To get data on GlobalMaster/PastProject 
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetFileList()
        {
            return Json(await _globalMasterRepository.GetFileAsync());
        }
        /// <summary>
        /// To edit/save data on GlobalMaster/PastProject
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> SaveFile(FileStorageViewModel model)
        {
            dynamic response;
            if (model.Id > 0)
                response = await _globalMasterRepository.UpdateFileAsync(model);
            else
                response = await _globalMasterRepository.SaveFileAsync(model);
            return Json(response);
        }
        /// <summary>
        /// To go to GlobalMaster/CopyCenter
        /// </summary>
        /// <returns></returns>
        public IActionResult CopyCenter()
        {
            return View();
        }
        /// <summary>
        /// To get data on GlobalMaster/CopyCenter
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetCopyCenterList()
        {
            return Json(await _globalMasterRepository.GetCopyCenterAsync());
        }
        /// <summary>
        /// To save data from GlobalMaster/CopyCenter
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> SaveCopyCenter(CopyCenterViewModel model)
        {
            dynamic response;
            if (model.Id > 0)
                response = await _globalMasterRepository.UpdateCopyCenterAsync(model);
            else
                response = await _globalMasterRepository.SaveCopyCenterAsync(model);
            return Json(response);
        }
        #endregion
        #region Career Postings
        /// <summary>
        /// To go to GlobalMaster/CareerPosting
        /// </summary>
        /// <returns></returns>
        public IActionResult CareerPosting()
        {
            return View();
        }
        /// <summary>
        /// To get data from GlobalMaster/CareerPosting
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetCareerPostingList()
        {
            return Json(await _globalMasterRepository.GetCareerPostingAsync());
        }
        /// <summary>
        /// To save data from GlobalMaster/CareerPosting
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> SaveCareerPosting(CareerPostingViewModel model)
        {
            dynamic response;
            if (model.Id > 0)
                response = await _globalMasterRepository.UpdateCareerPostingAsync(model);
            else
                response = await _globalMasterRepository.SaveCareerPostingAsync(model);
            return Json(response);
        }
        /// <summary>
        /// To delete data from GlobalMaster/CareerPosting
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<JsonResult> DeleteCareerPosting(int Id)
        {
            return Json(await _globalMasterRepository.DeleteCareerPostingAsync(Id));
        }
        #endregion
        #region HolidaySettings
        /// <summary>
        /// To go to GlobalMaster/HolidaySetting
        /// </summary>
        /// <returns></returns>
        public IActionResult HolidaySetting()
        {
            return View();
        }
        public async Task<JsonResult> GetHolidaySettingList()
        {
            return Json(await _globalMasterRepository.GetHolidaySettingAsync());
        }
        /// <summary>
        /// To save data from GlobalMaster/HolidaySetting
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> SaveHolidaySetting(TblDailyInfoHoliday model)
        {
            dynamic response;
            if (model.DiholidayId > 0)
                response = await _globalMasterRepository.UpdateHolidaySettingAsync(model);
            else
                response = await _globalMasterRepository.SaveHolidaySettingAsync(model);
            return Json(response);
        }
        /// <summary>
        /// To delete data from GlobalMaster/HolidaySetting
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<JsonResult> DeleteHolidaySetting(int Id)
        {
            return Json(await _globalMasterRepository.DeleteHolidaySettingAsync(Id));
        }
        #endregion
        #region Login Reports
        /// <summary>
        /// To go to GlobalMaster/LoginReport
        /// </summary>
        /// <returns></returns>
        public IActionResult LoginReport()
        {
            return View();
        }
        /// <summary>
        /// To get data for GlobalMaster/LoginReport
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetLoginReportList()
        {
            return Json(await _globalMasterRepository.GetLoginReportAsync());
        }
        #endregion
        #region Special Message
        /// <summary>
        /// To go to GlobalMaster/SpecialMsg
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> SpecialMsg()
        {
            return View();
        }
        /// <summary>
        /// To get data on GlobalMaster/SpecialMsg
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetSpecialMsgList()
        {
            return Json(await _globalMasterRepository.GetSpecialMsgAsync());
        }
        /// <summary>
        /// To save/update detail from GlobalMaster/SpecialMsg
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> SaveSpecialMsg(TblSpecialMsg model)
        {
            dynamic response;
            if (model.Id > 0)
                response = await _globalMasterRepository.UpdateSpecialMsgAsync(model);
            else
                response = await _globalMasterRepository.SaveSpecialMsgAsync(model);

            if (response.success && model.IsActive && model.Type?.ToLower() == "maintenance")
            {
                EmailViewModel emailObj = new();
                var data = await _emailServiceManager.SiteMaintenance(emailObj, model);
                if (data.EmailTos.Count > 0)
                {
                    await _emailServiceManager.SendEmail(data);
                }
            }
            return Json(response);
        }
        /// <summary>
        /// To delete detail from GlobalMaster/SpecialMsg
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<JsonResult> DeleteSpecialMsg(int Id)
        {
            return Json(await _globalMasterRepository.DeleteSpecialMsgAsync(Id));
        }
        #endregion
        /// <summary>
        /// For going to admin dashboard screen/page.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Dashboard()
        {
            return View();
            //return View();
        }
        /// <summary>
        /// Manage free trial tab on register/signup new user modal from admin site.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> ManageFreeTrialTab()
        {
            return View();
            //return View();
        }
        /// <summary>
        /// Get data for freetrial tab in payment card.
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetTabData()
        {
            HttpResponseDetail<dynamic> httpResponse = await _globalMasterRepository.GetTabData();
            return Json(httpResponse);
            //return View();
        }
        /// <summary>
        /// set tab data for freetrial tab in payment card.
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> SetTabData(bool SetTab)
        {
            HttpResponseDetail<dynamic> httpResponse = await _globalMasterRepository.SetTabData(SetTab);
            return Json(httpResponse);
            //return View();
        }
        /// <summary>
        /// For going to additional pricing screen/page to update/add membership card price details.
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> AdditionalPricing()
        {
            IEnumerable<PaymentInfo> model = new List<PaymentInfo>();
            model = await _globalMasterRepository.GetAdditionalPackage();
            return View(model);
        }
        /// <summary>
        /// For going to additional pricing post method
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> SaveAddPackageData(PaymentInfo model)
        {
            dynamic response = await _globalMasterRepository.SaveAddPackageData(model);
            return Json(response);
        }
        #region Pickup Delivery Type
        /// <summary>
        /// To go GlobalMaster/DeliveryType
        /// </summary>
        /// <returns></returns>
        public IActionResult DeliveryType()
        {
            return View();
        }
        /// <summary>
        /// To get data on GlobalMaster/DeliveryType
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetDeliveryTypeList()
        {
            return Json(await _globalMasterRepository.GetDeliveryTypeAsync());
        }
        /// <summary>
        /// To save/edit data from GlobalMaster/DeliveryType
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> SaveDeliveryType(tblDeliveryMaster model)
        {
            dynamic response;
            if (model.DelivId > 0)
                response = await _globalMasterRepository.UpdateDeliveryTypeAsync(model);
            else
                response = await _globalMasterRepository.SaveDeliveryTypeAsync(model);
            return Json(response);
        }
        /// <summary>
        /// To delete data from GlobalMaster/DeliveryType
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<JsonResult> DeleteDeliveryType(int Id)
        {
            return Json(await _globalMasterRepository.DeleteDeliveryTypeAsync(Id));
        }
        #endregion
        #region Pickup Delivery SubType
        /// <summary>
        /// To go to GlobalMaster/DeliverySubType
        /// </summary>
        /// <returns></returns>
        public IActionResult DeliverySubType()
        {
            return View();
        }
        /// <summary>
        /// To get data on GlobalMaster/DeliverySubType
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetDeliverySubTypeList()
        {
            return Json(await _globalMasterRepository.GetDeliverySubTypeAsync());
        }
        /// <summary>
        /// To save/edit data from GlobalMaster/DeliverySubType
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> SaveDeliverySubType(tblDeliveryOption model)
        {
            dynamic response;
            if (model.DelivOptId > 0)
                response = await _globalMasterRepository.UpdateDeliverySubTypeAsync(model);
            else
                response = await _globalMasterRepository.SaveDeliverySubTypeAsync(model);
            return Json(response);
        }
        /// <summary>
        /// To delete data from GlobalMaster/DeliverySubType
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<JsonResult> DeleteDeliverySubType(int Id)
        {
            return Json(await _globalMasterRepository.DeleteDeliverySubTypeAsync(Id));
        }
        #endregion
    }
}

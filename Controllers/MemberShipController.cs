using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PCNW.Data.Repository;
using PCNW.Helpers;
using PCNW.Models;
using PCNW.Models.ContractModels;
using PCNW.Models.ProcessContracts;
using PCNW.Services;
using System.Text;

namespace PCNW.Controllers
{
    public partial class MemberShipController : BaseController
    {
        private readonly IMembershipRepository _membershipRepository;
        private readonly ILogger<MemberController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IEmailServiceManager _emailServiceManager;
        private readonly ICommonRepository _commonRepository;
        private readonly IWebHostEnvironment Environment;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IProjectRepository _projectRepository;
        private readonly IEntityRepository _entityRepository;
        public MemberShipController(IMembershipRepository membershipRepository, IWebHostEnvironment _Environment, ILogger<MemberController> logger, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ApplicationDbContext dbContext, IEmailServiceManager emailServiceManager, ICommonRepository commonRepository, RoleManager<IdentityRole> roleManager, IProjectRepository projectRepository, IEntityRepository entityRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
            _emailServiceManager = emailServiceManager;
            _membershipRepository = membershipRepository;
            _logger = logger;
            _commonRepository = commonRepository;
            Environment = _Environment;
            _roleManager = roleManager;
            _projectRepository = projectRepository;
            _entityRepository = entityRepository;
        }
        /// <summary>
        /// For check unique email(post method).
        /// </summary>
        /// <param name="uniqueName"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        public async Task<JsonResult> UniqueEmail(string uniqueName)
        {
            HttpResponseDetail<dynamic> response = new();
            var tblmember = await _dbContext.Users.Where(m => m.Email == uniqueName).FirstOrDefaultAsync();
            if (tblmember == null)
            {

            }
            else
            {
                response.success = true;
                response.statusMessage = "Email already exists";
            }
            return Json(response);
        }
        /// <summary>
        /// For check unique company name(Post method).
        /// </summary>
        /// <param name="uniqueCompany"></param>
        /// <returns></returns>
        [HttpPost]
        [Produces("application/json")]
        public async Task<JsonResult> UniqueCompany(string uniqueCompany)
        {
            HttpResponseDetail<dynamic> response = new();
            var tblmember = _entityRepository.GetEntities().Where(m => m.Company == uniqueCompany).FirstOrDefault();
            if (tblmember == null)
            {

            }
            else
            {
                response.success = true;
                response.statusMessage = "Company already exists";
            }
            return Json(response);
        }
        /// <summary>
        /// For check unique company name for contractor or architect (Post method).
        /// </summary>
        /// <param name="uniqueCompany"></param>
        /// <returns></returns>
        public async Task<JsonResult> UniqueComForConOrArch(string uniqueCompany, int id)
        {
            HttpResponseDetail<dynamic> response = new();
            var tblmember = _entityRepository.GetEntities().Where(m => m.Company == uniqueCompany && m.Id != id).FirstOrDefault();
            if (tblmember == null)
            {

            }
            else
            {
                response.success = true;
                response.statusMessage = "Company already exists";
            }
            return Json(response);
        }
        #region Member Registertion
        /// <summary>
        /// No use
        /// </summary>
        /// <param name="isFromFree"></param>
        /// <returns></returns>
        public async Task<IActionResult> OldRegister(bool isFromFree = false)
        {
            MemberShipRegistration model = new();
            model.hdnpreStep = 0;
            model.hdnnextStep = 2;
            model.hdncurrentStep = 1;
            model.Term = "";
            model.CheckRadio = "";

            if (isFromFree)
            {
                model.isFromFree = true;
            }
            List<string> objList = new List<string>();
            objList.Add("WBE");
            objList.Add("MBE");
            objList.Add("DBE");
            objList.Add("ESB");
            model.MinorityStatusList = objList;
            List<SelectListItem> divisionList = await _membershipRepository.GetMemberDivisionAsync();
            divisionList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Division Name--" });
            ViewBag.Divisions = divisionList;
            List<SelectListItem> StateList = await _membershipRepository.GetStates();
            StateList.Insert(0, new SelectListItem { Value = "0", Text = "--Select State--" });
            ViewBag.States = StateList;
            return View(model);
        }
        /// <summary>
        /// For going to register post method.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public IActionResult Register(MemberShipRegistration model)
        {
            return View(model);
        }
        /// <summary>
        /// GoToStep1 for register/signup new member post method(No use)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> GoToStep1(MemberShipRegistration model)
        {
            model.hdnpreStep = 0;
            model.hdnnextStep = 2;
            model.hdncurrentStep = 1;
            List<string> objList = new List<string>();
            objList.Add("WBE");
            objList.Add("MBE");
            objList.Add("DBE");
            objList.Add("ESB");
            model.MinorityStatusList = objList;
            List<SelectListItem> divisionList = await _membershipRepository.GetMemberDivisionAsync();
            divisionList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Division Name--" });
            ViewBag.Divisions = divisionList;
            List<SelectListItem> StateList = await _membershipRepository.GetStates();
            StateList.Insert(0, new SelectListItem { Value = "0", Text = "--Select State--" });
            ViewBag.States = StateList;
            return View("Register", model);
        }
        /// <summary>
        /// GoToStep2 for register/signup new member post method(No use)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> GoToStep2(MemberShipRegistration model, List<string> MinorityStatusList, string Div, string MailState, string BillState)
        {
            // Store blank values
            if (string.IsNullOrEmpty(model.hdnPass))
                model.hdnPass = model.ContactPassword;
            if (string.IsNullOrEmpty(model.hdnPassConfirm))
                model.hdnPassConfirm = model.ContactPasswordCofirmation;
            model.MinorityStatusList = MinorityStatusList;
            model.hdnpreStep = 1;
            model.hdnnextStep = 3;
            model.hdncurrentStep = 2;
            StringBuilder sb = new StringBuilder();
            foreach (string s in model.MinorityStatusList)
            {
                sb.Append(s);
                sb.Append(" ");
            }
            sb.Append(model.Certification);
            model.MinorityStatus = sb.ToString();
            if (string.IsNullOrEmpty(Div) || Div == "0")
            {
                model.Div = "No division is selected";
            }
            else
            {
                model.Div = _membershipRepository.GetSelectedText(Div);
            }
            model.BillState = _membershipRepository.GetSelectedStateText(BillState);
            model.MailState = _membershipRepository.GetSelectedStateText(MailState);
            model.ContactName = model.FirstName + " " + model.LastName;
            List<SelectListItem> divisionList = await _membershipRepository.GetMemberDivisionAsync();
            divisionList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Division Name--" });
            ViewBag.Divisions = divisionList;
            List<SelectListItem> StateList = await _membershipRepository.GetStates();
            StateList.Insert(0, new SelectListItem { Value = "0", Text = "--Select State--" });
            ViewBag.States = StateList;

            return View("Register", model);
        }
        /// <summary>
        /// GoToStep3 for register/signup new member post method(No use)
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> GoToStep3(MemberShipRegistration model)
        {
            model.hdnpreStep = 2;
            model.hdnnextStep = 4;
            model.hdncurrentStep = 3;
            List<string> objList = new List<string>();
            objList.Add("WBE");
            objList.Add("MBE");
            objList.Add("DBE");
            objList.Add("ESB");
            model.MinorityStatusList = objList;
            List<SelectListItem> divisionList = await _membershipRepository.GetMemberDivisionAsync();
            divisionList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Division Name--" });
            ViewBag.Divisions = divisionList;
            List<SelectListItem> StateList = await _membershipRepository.GetStates();
            StateList.Insert(0, new SelectListItem { Value = "0", Text = "--Select State--" });
            ViewBag.States = StateList;
            return View("Register", model);
        }
        /// <summary>
        /// GoToStep4 for register/signup new member post method.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> GoToStep4(MemberShipRegistration model)
        {
            string amount = model.MemberCost;
            string clearAmount = amount.Replace("$", "").Replace(",", "");
            model.MemberCost = clearAmount;
            if (model.ASPUserId == null || model.ASPUserId == Guid.Empty)
            {
                model.ASPUserId = Guid.NewGuid();
            }
            if (model.ToDelete != null)
            {
                int id = model.ToDelete;
                await _membershipRepository.UpdateInCompleteSignUpAsync(model, id);
            }

            if (model.PayStatus == "N")
            {
                TempData["Model"] = model.ToDelete;
                TempData["PayModeRef"] = model.PayModeRef;
                return RedirectToAction("AddUserSubcription", "ChargeBee", model);
            }


            // Register the member information
            else if (model.MSPChk == "Y")
            {
                await _membershipRepository.RegisterMembershipAsync(model);
                var user = new IdentityUser { Id = model.ASPUserId.ToString(), UserName = model.ContactEmail, Email = model.ContactEmail };
                var result = await _userManager.CreateAsync(user, model.hdnPass);
                if (result.Succeeded)
                {
                    #region AssignRole
                    IdentityRole role = await _roleManager.FindByNameAsync("Member");
                    if (role != null)
                    {
                        IdentityResult? res = null;
                        var AssignUser = await _userManager.FindByEmailAsync(model.ContactEmail);
                        res = await _userManager.AddToRoleAsync(AssignUser, role.Name);
                    }
                    #endregion
                    model.REMOTE_ADDR = GetIPAddressOfClient();
                    EmailViewModel emailObj = new();
                    await _emailServiceManager.GetEmailForRegistration(emailObj, model);
                    List<string> emails = new();
                    emails.Add(model.ContactEmail == null ? "codingbrains36@gmail.com" : model.ContactEmail);
                    emailObj.EmailTos = emails;
                    var response = await _emailServiceManager.SendEmail(emailObj);
                    await DeleteInCompleteSignUp(model.ToDelete);

                }

                model.hdnpreStep = 3;
                model.hdnnextStep = 0;
                model.hdncurrentStep = 4;
                List<string> objList = new List<string>();
                objList.Add("WBE");
                objList.Add("MBE");
                objList.Add("DBE");
                objList.Add("ESB");
                model.MinorityStatusList = objList;
                List<SelectListItem> divisionList = await _membershipRepository.GetMemberDivisionAsync();
                divisionList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Division Name--" });
                ViewBag.Divisions = divisionList;
                List<SelectListItem> StateList = await _membershipRepository.GetStates();
                StateList.Insert(0, new SelectListItem { Value = "0", Text = "--Select State--" });
                ViewBag.States = StateList;
                return View("Register", model);
                //return RedirectToAction("Register", "MemberShip");
            }
            else
            {
                model.hdnpreStep = 3;
                model.hdnnextStep = 0;
                model.hdncurrentStep = 4;
                List<string> objList = new List<string>();
                objList.Add("WBE");
                objList.Add("MBE");
                objList.Add("DBE");
                objList.Add("ESB");
                model.MinorityStatusList = objList;
                List<SelectListItem> divisionList = await _membershipRepository.GetMemberDivisionAsync();
                divisionList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Division Name--" });
                ViewBag.Divisions = divisionList;
                List<SelectListItem> StateList = await _membershipRepository.GetStates();
                StateList.Insert(0, new SelectListItem { Value = "0", Text = "--Select State--" });
                ViewBag.States = StateList;
                //ViewBag.PaymentTab = 4;
                return View("Register", model);
            }
        }
        #endregion
        /// <summary>
        /// For save incomplete signup post method.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> SaveInCompleteSignUp(MemberShipRegistration model)
        {
            return Json(await _membershipRepository.SaveInCompleteSignUpAsync(model));
        }
        /// <summary>
        /// For delete incomplete signup post method when signup completed.
        /// </summary>
        /// <param name="ToDelete"></param>
        /// <returns></returns>
        public async Task DeleteInCompleteSignUp(int ToDelete)
        {
            await _membershipRepository.DeleteInCompleteSignUp(ToDelete);

        }
        /// <summary>
        /// For going to the free trial member screen/page
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> FreeTrialMember()
        {
            List<SelectListItem> StateList = await _membershipRepository.GetStates();
            StateList.Insert(0, new SelectListItem { Value = "0", Text = "--Select State--" });
            ViewBag.States = StateList;
            return View();
        }
        /// <summary>
        /// For going to save free trial member post method. 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> SaveFreeTrialMember(MemberShipRegistration model)
        {
            HttpResponseDetail<dynamic> response = new();
            var Pass = model.FirstName + "@123";
            model.hdnPass = Pass.Replace(" ", "");
            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.hdnPass);
            if (result.Succeeded)
            {
                if (Guid.TryParse(user.Id, out Guid userIdGuid))
                {
                    model.ASPUserId = userIdGuid;
                    // Now model.ASPUserId holds the Guid representation of user.Id
                }
                response = await _membershipRepository.SaveFreeTrialMemberAsync(model);
                #region AssignRole
                IdentityRole role = await _roleManager.FindByNameAsync("Member");
                if (role != null)
                {
                    IdentityResult? res = null;
                    var AssignUser = await _userManager.FindByEmailAsync(model.Email);
                    res = await _userManager.AddToRoleAsync(AssignUser, role.Name);
                }
            }

            #endregion
            model.REMOTE_ADDR = GetIPAddressOfClient();
            EmailViewModel emailObj = new();
            await _emailServiceManager.GetEmailForRegistration(emailObj, model);
            List<string> emails = new();
            emails.Add(model.Email == null ? "codingbrains36@gmail.com" : model.Email);
            emailObj.EmailTos = emails;
            var response1 = _emailServiceManager.SendEmailTrialMember(emailObj);

            //var User = await _userManager.FindByEmailAsync(model.Email);
            //var token = await _userManager.GeneratePasswordResetTokenAsync(User);
            //var link = Url.Action("Resetpassword", "Account", new { token, email = model.Email }, Request.Scheme);

            //bool emailResponse = false;
            //if (!string.IsNullOrEmpty(link))
            //{
            //    EmailMessageSender emailHelper = new EmailMessageSender();
            //    emailResponse = emailHelper.SendEmailPasswordReset(model.Email, link);
            //}


            return Json(response);
        }
        /// <summary>
        /// For going to check county.
        /// </summary>
        /// <param name="City"></param>
        /// <param name="State"></param>
        /// <returns></returns>
        public async Task<JsonResult> CheckCounty(string City, string State)
        {
            return Json(await _projectRepository.CheckCountyAsync(City, State));
        }
        /// <summary>
        /// For going to check state.
        /// </summary>
        /// <param name="State"></param>
        /// <returns></returns>
        public async Task<JsonResult> CheckState(string State)
        {
            return Json(await _commonRepository.CheckStateAsync(State));
        }
        /// <summary>
        /// For going to register a new user screen/page.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="isFromFree"></param>
        /// <returns></returns>
        public async Task<IActionResult> Register(MemberShipRegistration model, bool isFromFree = false)
        {
            //MemberShipRegistration model = new();

            model.hdnpreStep = 0;
            model.hdnnextStep = 2;
            model.hdncurrentStep = 1;
            model.Term = "";
            model.CheckRadio = "";
            if (isFromFree)
            {
                model.isFromFree = true;
            }
            List<string> objList = new List<string>();
            objList.Add("WBE");
            objList.Add("MBE");
            objList.Add("DBE");
            objList.Add("ESB");
            model.MinorityStatusList = objList;
            List<SelectListItem> divisionList = await _membershipRepository.GetMemberDivisionAsync();
            divisionList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Division Name--" });
            ViewBag.Divisions = divisionList;
            List<SelectListItem> StateList = await _membershipRepository.GetStates();
            StateList.Insert(0, new SelectListItem { Value = "0", Text = "--Select State--" });
            ViewBag.States = StateList;
            return View(model);
        }
    }
}
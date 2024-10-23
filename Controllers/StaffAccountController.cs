using DocumentFormat.OpenXml.Packaging;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PCNW.Data.Repository;
using PCNW.Helpers;
using PCNW.Models;
using PCNW.Models.ContractModels;
using PCNW.Models.ProcessContracts;
using PCNW.Services;
using PCNW.ViewModel;
using System.Net;

namespace PCNW.Controllers
{
    [DisableRequestSizeLimit]
    [Authorize(Roles = "Staff")]
    public partial class StaffAccountController : BaseController
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment Environment;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IImplementRepository _implementRepository;
        private readonly IEntityRepository _entityrepository;
        private readonly ICommonRepository _commonRepository;
        private readonly IEmailServiceManager _emailServiceManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IGlobalMasterRepository _globalMasterRepository;
        private readonly IConfiguration _configuration;
        private readonly AppSettings _appSettings;
        private readonly ChargeBeeAPIService _chargebeeAPI;
        private readonly string _fileUploadPath;


        public StaffAccountController(ChargeBeeAPIService chargebeeAPI, IConfiguration configuration, IStaffRepository staffRepository, IProjectRepository projectRepository, IWebHostEnvironment _Environment, IMembershipRepository membershipRepository, IImplementRepository implementRepository, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ICommonRepository commonRepository, IEmailServiceManager emailServiceManager, RoleManager<IdentityRole> roleManager, IGlobalMasterRepository globalMasterRepository, IOptions<AppSettings> appSettings, IEntityRepository entityrepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _membershipRepository = membershipRepository;
            _staffRepository = staffRepository;
            _configuration= configuration;
            _implementRepository = implementRepository;
            _projectRepository = projectRepository;
            Environment = _Environment;
            _commonRepository = commonRepository;
            _emailServiceManager = emailServiceManager;
            _roleManager = roleManager;
            _globalMasterRepository = globalMasterRepository;
            _appSettings = appSettings.Value;
            _entityrepository = entityrepository;
            _chargebeeAPI = chargebeeAPI;
            _fileUploadPath = _configuration.GetSection("AppSettings")["FileUploadPath"]; ;
        }
        /// <summary>
        /// For going to staff's login screen/page.
        /// </summary>
        /// <param name="ReturnUrl"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("StaffAccount/Login")]
        public IActionResult Login(string ReturnUrl)
        {
            LoginViewModel model = new();
            ViewData["returnedUrl"] = ReturnUrl;
            model.ReturnUrl = ReturnUrl;
            return View(model);
        }
        /// <summary>
        /// For going to login post method or verify login.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("StaffAccount/Login")]
        public async Task<IActionResult> VerifyLogin(LoginViewModel model)
        {

            #region Check as Role
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                {
                    ModelState.AddModelError("", "Invalid Login Attempt");
                    model.RedirectFromLogin = String.Format("You have enterd wrong username or password");
                    return View("Login", model);
                }
                if (await _userManager.IsInRoleAsync(user, "Administration"))
                {
                    ModelState.AddModelError("", "Invalid Login Attempt");
                    model.RedirectFromLogin = String.Format("You have enterd wrong username or password");
                    return View("Login", model);
                }
                if (await _userManager.IsInRoleAsync(user, "Member"))
                {
                    ModelState.AddModelError("", "Invalid Login Attempt");
                    model.RedirectFromLogin = String.Format("You have enterd wrong username or password");
                    return View("Login", model);
                }
            }
            #endregion
            var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(model.ReturnUrl))
                {
                    string ControllerName = string.Empty;
                    string ActionName = string.Empty;
                    string[] Arr = model.ReturnUrl.Replace("%2F", "/").Split('/');
                    ControllerName = Arr[1];
                    ActionName = Arr[2];
                    await _commonRepository.GetLogActivityAsync(model);
                    return Redirect(model.ReturnUrl);
                }
                else
                {
                    TempData["Email"] = model.Email;
                    await _commonRepository.GetLogActivityAsync(model);
                    return RedirectToAction("Dashboard", "StaffAccount");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid Login Attept");
                model.RedirectFromLogin = String.Format("You have enterd wrong username or password");
            }


            return View("Login", model);
        }
        /// <summary>
        /// For logout staff session/module and going to staff login screen/page.
        /// </summary>
        /// <param name="IsAutoLogout"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Logout(bool IsAutoLogout)
        {
            await _commonRepository.GetLogOutActivityAsync(User.Identity.Name, IsAutoLogout);
            await _signInManager.SignOutAsync();
            if (Request.Cookies["loggedinname"] != null)
            {
                Response.Cookies.Delete("loggedinname");
            }
            return RedirectToAction("Login", "Account");
        }
        /// <summary>
        /// For auto logout page when user remain inactive in the screen and save activity record in TblLogActivity and going to staff login page.
        /// </summary>
        /// <param name="IsAutoLogout"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        public async Task<IActionResult> AutoLogout(bool IsAutoLogout, string returnUrl)
        {
            await _commonRepository.GetLogOutActivityAsync(User.Identity.Name, IsAutoLogout);
            await _signInManager.SignOutAsync();
            if (Request.Cookies["loggedinname"] != null)
            {
                Response.Cookies.Delete("loggedinname");
            }
            return RedirectToAction("Login", "Account", new { ReturnUrl = returnUrl });
        }
        /// <summary>
        /// Not use (Old)
        /// </summary>
        /// <param name="ReturnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> MemberManagement1(string ReturnUrl = "")
        {
            var ActiveText = Request.Cookies["dtActiveText"];
            ViewBag.ActiveTab = ActiveText != null ? ActiveText.ToString() : "";
            if (ViewBag.ActiveTab != null)
            {
                ReturnUrl = ViewBag.ActiveTab;
            }
            Response.Cookies.Delete("dtActiveText");
            var members = _staffRepository.GetMembers();
            var contractors = _staffRepository.GetContractorArchitect();
            var entities = _staffRepository.GetEntity();
            var viewModel = new StaffManagementViewModel
            {
                Members = members,
                Contractors = contractors,
                Entities = entities,
                ReturnUrl = ReturnUrl
            };
            return View(viewModel);
        }
        /// <summary>
        /// For going to membermanagement screen/page.It is  populate member/nonmember/inactive member/contractor/architect data.
        /// </summary>
        /// <param name="ReturnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> MemberManagement(string ReturnUrl = "")
        {
            var ActiveText = Request.Cookies["dtActiveText"];
            ViewBag.ActiveTab = ActiveText != null ? ActiveText.ToString() : "";
            if (ViewBag.ActiveTab != null)
            {
                ReturnUrl = ViewBag.ActiveTab;
            }
            Response.Cookies.Delete("dtActiveText");
            MemberManagement model = await _staffRepository.GetMemberManagementData();
            if (ReturnUrl != null)
                model.ReturnUrl = ReturnUrl;
            return View(model);
        }
        public async Task<IActionResult> Entities(int page = 1, string searchTerm = "", string ReturnUrl = "")
        {
            const int pageSize = 100; // Number of items per page
            MemberManagement model = await _staffRepository.GetOtherTabsData(page, pageSize, searchTerm);
            if (ReturnUrl != null)
                model.ReturnUrl = ReturnUrl;
            return View(model);
        }

        //[HttpPost]
        //public async Task<IActionResult> Entities(string searchtext = "",string getdata="")
        //{
        //    MemberManagement model = await _staffRepository.GetOtherTabsSearchData(searchtext);            
        //    return View("Entities", model);
        //}
        /// <summary>
        /// For going to dashboard screen page and populate data pending/active/past poroject.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            var searchText = Request.Cookies["dtSearchText"];
            var ActiveText = Request.Cookies["dtActiveText"];
            var ActivesearchText = Request.Cookies["dtActiveSearchText"];
            var PastsearchText = Request.Cookies["dtPastSearchText"];
            //var ser = TempData["value"].ToString();
            string usern = User.Identity.Name;
            ViewBag.SearchText = searchText != null ? searchText.ToString() : "";
            ViewBag.ActiveTab = ActiveText != null ? ActiveText.ToString() : "";
            ViewBag.ActivesearchText = ActivesearchText != null ? ActivesearchText.ToString() : "";
            ViewBag.PastSearchText = PastsearchText != null ? PastsearchText.ToString() : "";
            ViewBag.SuccessMessage = TempData["SuccessMessage"] != null ? TempData["SuccessMessage"].ToString() : "";
            Response.Cookies.Delete("dtSearchText");
            Response.Cookies.Delete("dtActiveText");
            Response.Cookies.Delete("dtActiveSearchText");
            Response.Cookies.Delete("dtPastSearchText");
            var model = await _membershipRepository.GetProjectDashboardInfoAsync(usern);
            return View(model);
        }
        /// <summary>
        /// Not use
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> DashboardMember()
        {
            string usern = User.Identity.Name;
            IEnumerable<ProjectInformation> model = await _membershipRepository.GetMemberDashboardProjectsAsync(usern);
            return View(model);
            //return View();
        }
        /// <summary>
        /// For going to staff's preview screen/page.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="LoadValChk"></param>
        /// <returns></returns>
        public async Task<IActionResult> Preview(int id, string LoadValChk = "NoValue")
        {
            ViewBag.LoadValChk = LoadValChk;
            List<SelectListItem> StateList = await _membershipRepository.GetStates();
            StateList.Insert(0, new SelectListItem { Value = "0", Text = "--Select State--" });
            ViewBag.States = StateList;
            ProjectInformation model = await _projectRepository.GetProjectPreview(id);
            if (model is null)
            {
                TempData["Message"] = "No Project Found.";
                return RedirectToAction("Index", "Home");
            }
            if (!string.IsNullOrEmpty(model.ProjNumber))
            {
                string basePath = Path.Combine(_fileUploadPath, "20" + model.ProjNumber.Substring(0, 2), model.ProjNumber.Substring(2, 2), model.ProjNumber);

                // Retrieve Addenda files information
                List<AddendaFileInfo> AddendaS3List = new List<AddendaFileInfo>();
                List<AddendaDIRInfo> AddendaDIRInfoList = new List<AddendaDIRInfo>();

                // Get list of Addenda files
                string addendaPath = Path.Combine(basePath, "Addenda");
                if (Directory.Exists(addendaPath))
                {
                    string[] addendaFiles = Directory.GetFiles(addendaPath);
                    foreach (string file in addendaFiles)
                    {
                        AddendaFileInfo fi = new AddendaFileInfo();
                        fi.IsFile = true;
                        fi.FileInfo = Path.GetFileName(file);
                        fi.PathInfo = file;
                        AddendaS3List.Add(fi);
                    }

                    string[] addendaDirectories = Directory.GetDirectories(addendaPath);
                    foreach (string directory in addendaDirectories)
                    {
                        AddendaDIRInfo di = new AddendaDIRInfo();
                        di.FileInfo = Path.GetFileName(directory);
                        di.PathInfo = directory;
                        di.addendaFileInfos = new List<AddendaFileInfo>();

                        string[] dirFiles = Directory.GetFiles(directory);
                        foreach (string dirFile in dirFiles)
                        {
                            AddendaFileInfo dFi = new AddendaFileInfo();
                            dFi.FileInfo = Path.GetFileName(dirFile);
                            dFi.PathInfo = dirFile;
                            di.addendaFileInfos.Add(dFi);
                        }

                        AddendaDIRInfoList.Add(di);
                    }
                }

                model.AddendaS3Files = AddendaS3List;
                model.AddendaDIRInfos = AddendaDIRInfoList;

                // Get list of PHL files
                List<PHLInfo> PHLS3List = new List<PHLInfo>();
                string phlPath = Path.Combine(basePath, "PHL");
                if (Directory.Exists(phlPath))
                {
                    string[] phlFiles = Directory.GetFiles(phlPath);
                    foreach (string file in phlFiles)
                    {
                        PHLInfo fi = new PHLInfo();
                        fi.FileInfo = Path.GetFileName(file);
                        fi.PathInfo = file;
                        PHLS3List.Add(fi);
                    }
                }
                model.PHLFiles = PHLS3List;

                // Get list of Plans files
                List<PlansInfo> PlansS3List = new List<PlansInfo>();
                string plansPath = Path.Combine(basePath, "Plans");
                if (Directory.Exists(plansPath))
                {
                    string[] plansFiles = Directory.GetFiles(plansPath);
                    foreach (string file in plansFiles)
                    {
                        PlansInfo fi = new PlansInfo();
                        fi.FileInfo = Path.GetFileName(file);
                        fi.PathInfo = file;
                        PlansS3List.Add(fi);
                    }
                }
                model.PlansFiles = PlansS3List;

                // Get list of Specs files
                List<SpecsInfo> SpecsS3List = new List<SpecsInfo>();
                string specsPath = Path.Combine(basePath, "Specs");
                if (Directory.Exists(specsPath))
                {
                    string[] specsFiles = Directory.GetFiles(specsPath);
                    foreach (string file in specsFiles)
                    {
                        SpecsInfo fi = new SpecsInfo();
                        fi.FileInfo = Path.GetFileName(file);
                        fi.PathInfo = file;
                        SpecsS3List.Add(fi);
                    }
                }
                model.SpecsFiles = SpecsS3List;

                // Get list of Bid Results files
                List<BidResultsInfo> BidResultsS3List = new List<BidResultsInfo>();
                string bidResultsPath = Path.Combine(basePath, "Bid Results");
                if (Directory.Exists(bidResultsPath))
                {
                    string[] bidResultsFiles = Directory.GetFiles(bidResultsPath);
                    foreach (string file in bidResultsFiles)
                    {
                        BidResultsInfo fi = new BidResultsInfo();
                        fi.FileInfo = Path.GetFileName(file);
                        fi.PathInfo = file;
                        BidResultsS3List.Add(fi);
                    }
                }
                model.BidResultsFiles = BidResultsS3List;



            }
            return View(model);
        }
        /// <summary>
        /// For going to copycenter screen/page.
        /// </summary>
        /// <param name="LoadValChk"></param>
        /// <returns></returns>
        public async Task<IActionResult> CopyCenter(string LoadValChk = "NoValue")
        {
            ViewBag.LoadValChk = LoadValChk;
            var returnurl = _appSettings.ViewDocUrl;
            // var redirectUrl = _configuration.GetSection("AppSettings")["BaseURL"];
            IEnumerable<OrderTables> model = await _staffRepository.GetDashboardProjectsAsync(returnurl);
            List<SelectListItem> StateList = await _membershipRepository.GetStates();
            StateList.Insert(0, new SelectListItem { Value = "0", Text = "--Select State--" });
            ViewBag.States = StateList;
            return View(model);
        }
        /// <summary>
        /// For Complete/Send Notice or mail from staff copy center
        /// </summary>
        /// <param name="ProjId"></param>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<IActionResult> UpdateDoneDt(string ProjId, OrderTables data)
        {
            string user = User.Identity.Name;
            DisplayLoginInfo info = new();
            await _staffRepository.UpdateDoneDt(ProjId, data);
            if (data.Email != null)
            {
                data.REMOTE_ADDR = GetIPAddressOfClient();
                data.AuthorizedBy = user;
                EmailViewModel emailObj = new();
                await _emailServiceManager.CompleteSendNotice(emailObj, data);
                emailObj.EmailTos = new List<string> { data.Email };
                var response = await _emailServiceManager.SendEmailForPickup(emailObj);
            }
            IEnumerable<OrderTables> model = await _staffRepository.GetDashboardProjectsAsync();
            return RedirectToAction("CopyCenter", model);
            //View(model);
        }
        /// <summary>
        /// For update copycenter order status(Ready for Pickup/Delivery)
        /// </summary>
        /// <param name="OrderId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> UpdateShipDt(string OrderId, OrderTables model)
        {
            string user = User.Identity.Name;
            DisplayLoginInfo info = new();
            await _staffRepository.UpdateShipDt(OrderId, model);
            model.REMOTE_ADDR = GetIPAddressOfClient();
            model.AuthorizedBy = info.Name;
            if (model.Email != null)
            {
                model.REMOTE_ADDR = GetIPAddressOfClient();
                model.AuthorizedBy = info.Name;
                EmailViewModel emailObj = new();
                await _emailServiceManager.ReadyForPickup(emailObj, model);
                emailObj.EmailTos = new List<string> { model.Email };
                var response = await _emailServiceManager.SendEmailForPickup(emailObj);
            }

            IEnumerable<OrderTables> model1 = await _staffRepository.GetDashboardProjectsAsync();
            return RedirectToAction("CopyCenter");
            //View(model);
        }
        /// <summary>
        /// For reorder copy
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public async Task<JsonResult> Reorder(int OrderId)
        {
            HttpResponseDetail<dynamic> response = await _commonRepository.Reorder(OrderId);
            try
            {
                int prev = response.data[0];
                int next = response.data[1];
                string rootPath = Environment.WebRootPath;
                string folderPath = Path.Combine(rootPath, "Storage");
                string sourcePath = Path.Combine(folderPath, prev.ToString());
                string destPath = Path.Combine(folderPath, next.ToString());
                Directory.CreateDirectory(destPath);
                var allFiles = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);
                foreach (string newPath in allFiles)
                {
                    System.IO.File.Copy(newPath, newPath.Replace(sourcePath, destPath), true);
                }
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "Project reordered successfully";
            }
            catch (Exception Ex)
            {
                response.success = false;
                response.statusMessage = "Something went wrong";
                response.statusCode = "400";
                response.data = Ex.Message;
            }
            return Json(response);
        }
        /// <summary>
        /// Open a modal for particular order and view order copy/details.
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public async Task<JsonResult> UpdateViewed(string OrderId)
        {
            string status = "";
            dynamic ProjOrder = await _staffRepository.UpdateViewed(OrderId);
            if (ProjOrder != null)
            {
                status = "success";
            }
            return Json(new { Status = status });
        }
        /// <summary>
        /// for view copy order doc in a new tab
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public async Task<IActionResult> ViewDoc(string OrderId)
        {
            List<string> lstPdf = new List<string>();
            lstPdf = await _staffRepository.ViewDoc(OrderId);
            //IEnumerable<OrderTables> model = await _staffRepository.GetDashboardProjectsAsync();
            return View(lstPdf);
            //View(model);
        }
        /// <summary>
        /// For change project status publish or unpublish(Pending/Active Project).
        /// </summary>
        /// <param name="Change"></param>
        /// <param name="ProjId"></param>
        /// <returns></returns>
        public async Task<IActionResult> ChangePublish(bool Change, string ProjId)
        {
            await _staffRepository.ChangePublish(Change, ProjId);
            IEnumerable<OrderTables> model = await _staffRepository.GetDashboardProjectsAsync();
            return RedirectToAction("CopyCenter");
            //View(model);
        }
        /// <summary>
        /// Upload pdf on copycenter page for order copy.
        /// </summary>
        /// <param name="pdfFile"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UploadPrintPdf(List<IFormFile> pdfFile, string time)
        {
            string wwwPath = this.Environment.WebRootPath;
            string contentPath = this.Environment.ContentRootPath;
            string pathPhoto = Path.Combine(this.Environment.WebRootPath, "Storage");
            //C:\imageDesktop\9march\New folder
            //pathPhoto = CreatePath(pathPhoto);
            string today = DateTime.Now.ToString("yyyyMMdd");
            pathPhoto = Path.Combine(pathPhoto, today);
            pathPhoto = pathPhoto + "_" + time;
            if (!Directory.Exists(pathPhoto))
            {
                Directory.CreateDirectory(pathPhoto);
            }
            string pathTemp = pathPhoto.Substring(0, pathPhoto.LastIndexOf("\\") + 1);
            pathTemp = pathPhoto.Replace(pathTemp, "");

            Dictionary<string, IFormFile> filesByName = new Dictionary<string, IFormFile>();

            // Iterate through each IFormFile
            foreach (var pdfitem in pdfFile)
            {
                string fileName = pdfitem.FileName;

                // Check if the file name is already in the dictionary
                if (filesByName.ContainsKey(fileName))
                {
                    // If yes, remove the old entry
                    filesByName.Remove(fileName);
                }

                // Add the current file to the dictionary
                filesByName.Add(fileName, pdfitem);
            }
            List<IFormFile> finalPdfFiles = new List<IFormFile>(filesByName.Values);
            pdfFile = finalPdfFiles;

            try
            {
                Directory.CreateDirectory(pathPhoto);
                foreach (IFormFile formFile in pdfFile)
                {
                    //using var stream1 = new MemoryStream(System.IO.File.ReadAllBytes("https://contractor-aws.s3.amazonaws.com/2023/01/23010008/Plans/103120000000007373.pdf").ToArray());
                    //IFormFile uploadfile = new FormFile(stream1, 0, stream1.Length, "streamFile", "103120000000007373.pdf");
                    string fileName = Path.GetFileName(formFile.FileName);
                    fileName = fileName.Contains(" ") ? fileName.Replace(" ", "_") : fileName;
                    using (FileStream stream = new FileStream(Path.Combine(pathPhoto, fileName), FileMode.Create))
                    {
                        formFile.CopyTo(stream);
                    }
                    string message = string.Format("{0}", fileName);
                }
            }
            catch (Exception ex)
            {

            }
            //return Json(new { Status = "success", Message = message });
            return Json(new { Status = "success", Data = pathTemp, FilePath = pathPhoto, Flag = 'Y' });
        }
        /// <summary>
        /// For CopyCenter post method or save order copy details.
        /// </summary>
        /// <param name="LoadValChk"></param>
        /// <returns></returns>
        public async Task<ActionResult> SaveCopyCenterInfo(string LoadValChk = "NoValue")
        {
            string modelJson = Request.Cookies["GetCopyModel"] != null ? Request.Cookies["GetCopyModel"] : "";
            string pathChK = Request.Cookies["pathChk"] != null ? Request.Cookies["pathChk"] : "";
            Response.Cookies.Delete("pathChk");
            Response.Cookies.Delete("GetCopyModel");
            Response.Cookies.Delete("copyCenterModel");
            Response.Cookies.Delete("PayType");
            Response.Cookies.Delete("ContPath");
            Response.Cookies.Delete("ActPath");
            Response.Cookies.Delete("SuccessPath");
            HttpResponseDetail<dynamic> httpResponse = new();
            string user = User.Identity.Name;
            DisplayLoginInfo info = new();
            OrderTables CopyModel = JsonConvert.DeserializeObject<OrderTables>(modelJson);
            info = _commonRepository.GetUserInfo(User.Identity.Name);
            httpResponse = await _staffRepository.SaveCopyCenterInfoAsync(CopyModel);
            string Oid = httpResponse.data.OrderId.ToString();
            string pathPhoto = Path.Combine(this.Environment.WebRootPath, "Storage");
            string ToReplace = Path.Combine(pathPhoto, pathChK);
            string Replaced = Path.Combine(pathPhoto, Oid);
            Directory.Move(ToReplace, Replaced);

            CopyModel.REMOTE_ADDR = GetIPAddressOfClient();
            CopyModel.AuthorizedBy = info.Name;
            EmailViewModel emailObj = new();
            EmailViewModel useremailObj = new();
            await _emailServiceManager.SavePrintOrder(emailObj, CopyModel);
            await _emailServiceManager.GetEmailForCreateUser(useremailObj, CopyModel);
            emailObj.EmailTos = CopyModel.EmailsTo;
            useremailObj.EmailTos = new List<string> { CopyModel.Email };
            var response = await _emailServiceManager.SendEmailSavePrintOrder(emailObj);
            var response1 = await _emailServiceManager.SendEmail(useremailObj);

            if (CopyModel.ProjOrderId != 0 && CopyModel.ProjOrderId != null)
            {
                return RedirectToAction("Preview", new { id = CopyModel.ProjOrderId, LoadValChk = "success" });
            }
            else
            {
                return RedirectToAction("CopyCenter", new { LoadValChk = "success" });
            }
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public async Task<JsonResult> DashboardMemberSearch(string searchText)
        {
            //HttpResponseDetail<dynamic> response = new();
            string usern = User.Identity.Name;
            IEnumerable<ProjectInformation> model = await _membershipRepository.GetMemberDashboardSearchProjectsAsync(searchText, usern);
            return Json(model);
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public async Task<JsonResult> GetProjectName(string searchText)
        {
            string model = _projectRepository.GetProjectName(searchText);
            return Json(model);
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <param name="searchText"></param>
        /// <returns></returns>
        public async Task<JsonResult> MemberManagementSearch(string searchText)
        {
            IEnumerable<MemberShipRegistration> model = await _membershipRepository.GetMembersSearchAsync(searchText);

            return Json(model);
        }
        /// <summary>
        /// For get plan holder list pdf/file on dashboard screen/page..
        /// </summary>
        /// <param name="ProjId"></param>
        /// <returns></returns>
        public async Task<JsonResult> ListPdfContent(int ProjId)
        {
            List<string> model = await _membershipRepository.ListPdfContent(ProjId);

            return Json(model);
        }
        /// <summary>
        /// For get addenda pdf/file list on dashboard screen/page.
        /// </summary>
        /// <param name="ProjId"></param>
        /// <returns></returns>
        public async Task<JsonResult> AddendaListPdfContent(int ProjId)
        {
            List<string> model = await _membershipRepository.AddendaListPdfContent(ProjId);

            return Json(model);
        }
        /// <summary>
        /// For delete project on staff's dashboard screen/page.(StaffAccount/Dashboard)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResult> DeleteProject(int id)
        {
            return Json(await _staffRepository.DeleteProject(id));
        }
        /// <summary>
        /// For going to memberprofile screen/page particular id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ActiveTab"></param>
        /// <returns></returns>
        public async Task<IActionResult> MemberProfile(int id, string ActiveTab = "")
        {
            MemberShipRegistration model = new();
            DisplayLoginInfo logInfo = new();
            int ConId = 0;
            bool IsMember = false;
            if (!string.IsNullOrEmpty(ActiveTab))
            {
                Response.Cookies.Append("dtActiveText", ActiveTab, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    HttpOnly = false,
                    Secure = Convert.ToBoolean(false)
                });
            }
            string successMessage = TempData["SuccessMessage"] as string;
            if (!string.IsNullOrEmpty(successMessage))
            {
                ViewBag.SuccessMessage = successMessage;
            }
            model = await _membershipRepository.GetMemberProfileAsync(id, ConId, "");
            List<SelectListItem> StateList = await _membershipRepository.GetStates();
            StateList.Insert(0, new SelectListItem { Value = "0", Text = "--Select State--" });
            ViewBag.States = StateList;
            List<SelectListItem> divisionList = await _membershipRepository.GetMemberDivisionAsync();
            divisionList.Insert(0, new SelectListItem { Value = "0", Text = "--Select Division Name--" });
            ViewBag.Divisions = divisionList;
            List<string> abc = model.DivisionList;
            ViewBag.abc = abc;
            List<string> objList = new List<string>();
            objList.Add("WBE");
            objList.Add("MBE");
            objList.Add("DBE");
            objList.Add("ESB");
            model.MinorityStatusList = objList;
            List<SelectListItem> LocUser = await _membershipRepository.GetMemberLocAsync(id);
            LocUser.Insert(0, new SelectListItem { Value = "0", Text = "--Select Location--" });
            ViewBag.LocUser = LocUser;
            if (IsMember)
                model.ProfileChk = "Y";
            return View(model);
        }
        /// <summary>
        /// For save directory or preview member details on memberprofile's (Member Directory Preview) screen/page.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> SaveDirectory(TblDirectoryCheck model)
        {
            HttpResponseDetail<dynamic> response = new();
            response = await _commonRepository.SaveDirectory(model);
            return Json(response);
        }
        /// <summary>
        /// For save License from staff memberprofile page
        /// </summary>
        /// <param name="lstState"></param>
        /// <param name="LicNum"></param>
        /// <param name="LicDesc"></param>
        /// <param name="MemId"></param>
        /// <returns></returns>
        public async Task<JsonResult> SaveLic(List<int> lstState, string LicNum, string LicDesc, int MemId)
        {
            HttpResponseDetail<dynamic> response = new();
            response = await _commonRepository.SaveLic(lstState, LicNum, LicDesc, MemId);
            return Json(response);
        }
        /// <summary>
        /// For memberprofile post method or update memberprofile from staff side.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> EditProfile(MemberShipRegistration model)
        {
            await _membershipRepository.EditMemberProfile(model);
            return RedirectToAction("MemberProfile", new { id = model.ID });
        }
        /// <summary>
        /// Add location from staff memberprofile screen/page 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> AddLocation(TblLocList model)
        {
            HttpResponseDetail<dynamic> response = new();
            response = await _commonRepository.AddLocation(model);
            return Json(response);
        }
        /// <summary>
        /// For autorenewalon post method from staff's memberprofile screen/page.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> AutoRenewOn(string id, bool val)
        {
            HttpResponseDetail<dynamic> response = new();
            await _membershipRepository.autoRenewOn(id, val);
            return Json(response);
        }
        /// <summary>
        /// For autorenewaloff post method from staff's memberprofile screen/page.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> AutoRenewOff(string id, bool val)
        {
            HttpResponseDetail<dynamic> response = new();
            await _membershipRepository.autoRenewOff(id, val);
            return Json(response);
        }
        /// <summary>
        /// For activemember post method from staff's memberprofile screen/page.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> Activemember(string id, bool val)
        {
            HttpResponseDetail<dynamic> response = new();
            await _membershipRepository.activemember(id, val);
            return Json(response);
        }
        /// <summary>
        /// For inactive member post method from staff's memberprofile screen/page.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> Inactivemember(string id, bool val)
        {
            HttpResponseDetail<dynamic> response = new();
            await _membershipRepository.inactivemember(id, val);
            return Json(response);
        }
        /// <summary>
        /// Add a new user for particular member/company on staff's User Management section.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="val"></param>
        /// <returns></returns>
        public async Task<JsonResult> AddUserNew(MemberShipRegistration model)
        {
            HttpResponseDetail<dynamic> response = new();
            response = await _membershipRepository.AddUserNew(model);
            if (response.success == true)
            {
                var user = new IdentityUser { UserName = model.ContactEmail, Email = model.ContactEmail };
                var result = await _userManager.CreateAsync(user, model.ContactPassword);

                _entityrepository.UpdateContactUserId(model.ContactEmail, user);
                IdentityRole role = await _roleManager.FindByNameAsync("Member");
                if (role != null)
                {
                    IdentityResult? res = null;
                    var AssignUser = await _userManager.FindByEmailAsync(model.ContactEmail);
                    res = await _userManager.AddToRoleAsync(AssignUser, role.Name);
                }
            }
            return Json(response);
        }
        /// <summary>
        /// For include directory in member directory screen/page.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> UpdateDirectory(MemberShipRegistration model)
        {
            return Json(await _commonRepository.UpdateDirectoryAsync(model));
        }
        /// <summary>
        /// For make  a user to admin on staff's memberprofile(User management section on check admin column check box).
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> AdminUserContact(MemberShipRegistration model)
        {
            return Json(await _commonRepository.AdminUserContactAsync(model));
        }
        /// <summary>
        /// for use user daily report checkbox on staff's memberprofile(User management section).
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> MemberUserDailyReport(MemberShipRegistration model)
        {
            return Json(await _commonRepository.MemberUserDailyReportAsync(model));
        }
        /// <summary>
        /// For onchange upload companylogo from staff's memberprofile screen/page.
        /// </summary>
        /// <param name="pdfFile"></param>
        /// <returns></returns>
        public JsonResult UploadLogo(IFormFile pdfFile)
        {
            string pathPhoto = Path.Combine(this.Environment.WebRootPath, "Profile");
            string fileName = Path.GetFileName(pdfFile.FileName);
            string extention = Path.GetExtension(fileName);
            using (FileStream stream = new FileStream(Path.Combine(pathPhoto, fileName), FileMode.Create))
                pdfFile.CopyTo(stream);
            string data = "\\Profile\\" + fileName;
            return Json(new { Status = "success", data = data, Flag = 'Y' });
        }
        /// <summary>
        /// For add note on staff's memberprofile screen/page.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> AddNote(NoteInfo model)
        {
            HttpResponseDetail<dynamic> response = new();
            DisplayLoginInfo logInfo = _commonRepository.GetUserInfo(User.Identity.Name);
            if (logInfo != null)
            {
                model.Name = logInfo.Name;
            }
            int res = await _membershipRepository.AddNote(model);
            if (res == 1)
            {
                response.statusMessage = "Note Added";
                response.statusCode = "400";
                response.success = true;
            }
            return Json(response);
        }
        /// <summary>
        /// For update/edit note on staff's memberprofile screen/page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> UpdateNote(NoteInfo model)
        {
            HttpResponseDetail<dynamic> response = new();
            DisplayLoginInfo logInfo = _commonRepository.GetUserInfo(User.Identity.Name);
            if (logInfo != null)
            {
                model.Name = logInfo.Name;
            }
            int res = await _membershipRepository.UpdateNote(model);
            if (res == 1)
            {
                response.success = true;
                response.statusMessage = "Note Updated";
                response.statusCode = "400";

            }

            return Json(response);
        }
        /// <summary>
        /// For delete note from staff's memberprofile screen/page
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> DeleteNote(NoteInfo model)
        {
            HttpResponseDetail<dynamic> response = new();
            int res = await _membershipRepository.DeleteNote(model.Id);
            if (res == 1)
            {
                response.statusMessage = "Note deleted";
                response.statusCode = "400";
                response.success = true;
            }
            return Json(response);
        }
        /// <summary>
        /// For edit/update member post method(User Management section).
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> EditUser(MemberShipRegistration model)
        {
            HttpResponseDetail<dynamic> response = new();
            response = await _membershipRepository.EditUser(model);
            return Json(response);
        }
        /// <summary>
        /// For order copies from staff preview screen/page.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public JsonResult OrderCopies(string filename, string filepath)
        {
            string pathPhoto = Path.Combine(this.Environment.WebRootPath, "Storage");
            pathPhoto = CreatePath(pathPhoto);
            Directory.CreateDirectory(pathPhoto);
            string pathTemp = pathPhoto.Substring(0, pathPhoto.LastIndexOf("\\") + 1);
            pathTemp = pathPhoto.Replace(pathTemp, "");
            pathPhoto = Path.Combine(pathPhoto, filename);
            string path = filepath;
            WebClient myWebClient = new WebClient();
            //byte[] myDataBuffer = myWebClient.DownloadData(filepath);
            myWebClient.DownloadFile(filepath, pathPhoto);
            PdfReader pdfReader = new PdfReader(pathPhoto);
            int numberOfPages = pdfReader.NumberOfPages;
            //return Json(new { Status = "success", Message = message });
            return Json(new { Status = "success", FileName = filename, PageCount = numberOfPages, data = pathTemp });
        }
        /// <summary>
        /// For order all copies on staff preview screen/page.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> OrderCopiesAll(List<PHLInfo> model)
        {
            try
            {
                string pathPhoto = Path.Combine(this.Environment.WebRootPath, "Storage");
                pathPhoto = CreatePath(pathPhoto);
                Directory.CreateDirectory(pathPhoto);
                string pathTemp = pathPhoto.Substring(0, pathPhoto.LastIndexOf("\\") + 1);
                pathTemp = pathPhoto.Replace(pathTemp, "");
                List<PrevOrder> prevOrders = new();
                foreach (var item in model)
                {
                    if (item.FileInfo != null)
                    {
                        item.FileInfo = item.FileInfo.Contains(" ") ? item.FileInfo.Replace(" ", "_") : item.FileInfo;
                        string pathPhotoTemp = Path.Combine(pathPhoto, item.FileInfo);
                        var tuple = await GetUniqueFileName(pathPhotoTemp, item.FileInfo, pathPhoto);
                        pathPhotoTemp = tuple.Item1;
                        item.FileInfo = tuple.Item2;
                        WebClient myWebClient = new WebClient();
                        //byte[] myDataBuffer = myWebClient.DownloadData(filepath);
                        myWebClient.Headers.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

                        myWebClient.DownloadFile(item.PathInfo, pathPhotoTemp);
                        PdfReader pdfReader = new PdfReader(pathPhotoTemp);
                        int numberOfPages = pdfReader.NumberOfPages;
                        prevOrders.Add(new PrevOrder { NoP = numberOfPages, FileName = item.FileInfo });
                    }
                }
                //return Json(new { Status = "success", Message = message });
                return Json(new { Status = "success", DirData = prevOrders, data = pathTemp });
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        /// <summary>
        /// For checking the existence of a file on a path and rename the file on the same path to avoid duplicacy
        /// </summary>
        /// <param name="path">Fully qualified path</param>
        /// <param name="FileInfo">File Name</param>
        /// <param name="rootPath">root folder path</param>
        /// <returns></returns>
        public async Task<Tuple<string, string>> GetUniqueFileName(string path, string FileInfo, string rootPath)
        {
            if (System.IO.File.Exists(path))
            {
                FileInfo = FileInfo.Replace(".pdf", "_1.pdf");
                path = Path.Combine(rootPath, FileInfo);
                if (System.IO.File.Exists(path))
                {
                    return await GetUniqueFileName(path, FileInfo, rootPath);
                }

            }
            return new Tuple<string, string>(path, FileInfo);
        }
        /// <summary>
        /// Get copy center price for order copies.
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetCopyCenterPriceDetail()
        {
            HttpResponseDetail<dynamic> response = new();
            response = await _commonRepository.GetCopyCenterPriceDetail();
            return Json(response);
        }
        /// <summary>
        /// For view/download bid results on preview screen/page.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<IActionResult> ViewBidRes(int Id)
        {
            List<string> lstPdf = new List<string>();
            ProjectInformation model = await _projectRepository.GetProjectPreview(Id);           //foreach (var item in bidResult)
            if (!string.IsNullOrEmpty(model.ProjNumber) && model.ProjNumber != "0")
            {
                string rootFolder = model.ProjNumber.Substring(0, 2);
                rootFolder = rootFolder.Replace(rootFolder, "20" + rootFolder);
                string subRootFolder = model.ProjNumber.Substring(2, 2);
                List<string> BidResults = await GetListOfS3Content(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/", "Bid Results/");
                foreach (string add in BidResults)
                {
                    string toAdd = add;
                    if (toAdd.Contains(" "))
                        toAdd = toAdd.Replace(" ", "+");
                    lstPdf.Add(toAdd);
                }

            }

            return View(lstPdf);
            //View(model);
        }
        /// <summary>
        /// For count pdf/doc page.
        /// </summary>
        /// <param name="file"></param>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public JsonResult Countdoc(IFormFile[] file, string FilePath)
        {
            List<IFormFile> pdfFile = file.ToList();

            Dictionary<string, IFormFile> filesByName = new Dictionary<string, IFormFile>();

            // Iterate through each IFormFile
            foreach (var pdfitem in pdfFile)
            {
                string fileName = pdfitem.FileName;

                // Check if the file name is already in the dictionary
                if (filesByName.ContainsKey(fileName))
                {
                    // If yes, remove the old entry
                    filesByName.Remove(fileName);
                }

                // Add the current file to the dictionary
                filesByName.Add(fileName, pdfitem);
            }
            List<IFormFile> finalPdfFiles = new List<IFormFile>(filesByName.Values);
            pdfFile = finalPdfFiles;

            file = pdfFile.ToArray();

            List<CountDoc> countDoc = new();
            dynamic response;
            if (file == null || file.Length == 0)
            {
                // return BadRequest("Please select one or more files to upload.");
            }
            var pageCount = 0;
            foreach (var fs in file)
            {
                var Ext = "";
                if (fs != null && fs.Length > 0)
                {
                    Ext = Path.GetExtension(fs.FileName).ToLower();
                    if (Ext == ".docx")
                    {
                        using (var stream = fs.OpenReadStream())
                        {
                            CountDoc Cdoc = new();
                            var doc = WordprocessingDocument.Open(stream, false);
                            pageCount = int.Parse(doc.ExtendedFilePropertiesPart.Properties.Pages.Text);
                            Cdoc.PageNo = pageCount.ToString();
                            Cdoc.FileName = fs.FileName;
                            countDoc.Add(Cdoc);
                        }
                    }
                    else if (Ext == ".pdf")
                    {
                        CountDoc Cdoc = new();
                        string ImFilename = fs.FileName;
                        ImFilename = ImFilename.Contains(" ") ? ImFilename.Replace(" ", "_") : ImFilename;
                        string pathToCount = Path.Combine(FilePath, ImFilename);
                        PdfReader pdfReader = new PdfReader(pathToCount);
                        Cdoc.PageNo = pdfReader.NumberOfPages.ToString();
                        Cdoc.FileName = fs.FileName;
                        countDoc.Add(Cdoc);
                    }
                    else if (Ext == ".jpg" || Ext == ".tif")
                    {
                        CountDoc Cdoc = new();
                        Cdoc.PageNo = "1";
                        Cdoc.FileName = fs.FileName;
                        countDoc.Add(Cdoc);
                    }
                }
            }
            return Json(new { Status = "success", Data = countDoc });
        }
        /// <summary>
        /// For going to register new contractor screen/page.
        /// </summary>
        /// <param name="ActiveTab"></param>
        /// <returns></returns>
        public async Task<IActionResult> RegContractor(string ActiveTab)
        {
            if (!string.IsNullOrEmpty(ActiveTab))
            {
                Response.Cookies.Append("dtActiveText", ActiveTab, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    HttpOnly = false,
                    Secure = Convert.ToBoolean(false)
                });
            }
            List<SelectListItem> StateList = await _membershipRepository.GetStates();
            StateList.Insert(0, new SelectListItem { Value = "0", Text = "--Select State--" });
            ViewBag.States = StateList;
            return View();
        }
        /// <summary>
        /// For going to register a new architect screen/page.
        /// </summary>
        /// <param name="ActiveTab"></param>
        /// <returns></returns>
        public async Task<IActionResult> RegisterContact(string ActiveTab)
        {
            if (!string.IsNullOrEmpty(ActiveTab))
            {
                Response.Cookies.Append("dtActiveText", ActiveTab, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    HttpOnly = false,
                    Secure = Convert.ToBoolean(false)
                });
            }
            List<SelectListItem> StateList = await _membershipRepository.GetStates();
            StateList.Insert(0, new SelectListItem { Value = "0", Text = "--Select State--" });
            ViewBag.States = StateList;
            return View();
        }
        /// <summary>
        /// For going to contractorprofile screen/page.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ActiveTab"></param>
        /// <returns></returns>
        public async Task<IActionResult> ContractorProfile(int id, string ActiveTab)
        {
            MemberShipRegistration model = new();
            if (!string.IsNullOrEmpty(ActiveTab))
            {
                Response.Cookies.Append("dtActiveText", ActiveTab, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    HttpOnly = false,
                    Secure = Convert.ToBoolean(false)
                });
            }
            model = await _staffRepository.GetContractorProfileAsync(id);
            List<SelectListItem> StateList = await _membershipRepository.GetStates();
            StateList.Insert(0, new SelectListItem { Value = "0", Text = "--Select State--" });
            ViewBag.States = StateList;
            return View(model);
        }
        /// <summary>
        /// For update/edit contractor profile post method.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> UpdateContractorProfile(MemberShipRegistration model)
        {
            HttpResponseDetail<dynamic> response = new();
            response = await _staffRepository.EditContractorProfileAsync(model);
            return Json(response);
        }
        /// <summary>
        /// Add a new member on contractor profile screen/page(User Management Section).
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> AddNewUser(MemberShipRegistration model)
        {
            HttpResponseDetail<dynamic> response = new();
            response = await _staffRepository.AddNewUserAsync(model);
            return Json(response);
        }
        /// <summary>
        /// For going to architect profile screen/page.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="ActiveTab"></param>
        /// <returns></returns>
        public async Task<IActionResult> ArchitectProfile(int id, string ActiveTab)
        {
            MemberShipRegistration model = new();
            if (!string.IsNullOrEmpty(ActiveTab))
            {
                Response.Cookies.Append("dtActiveText", ActiveTab, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    HttpOnly = false,
                    Secure = Convert.ToBoolean(false)
                });
            }
            model = await _staffRepository.GetArchitectProfileAsync(id);
            List<SelectListItem> StateList = await _membershipRepository.GetStates();
            StateList.Insert(0, new SelectListItem { Value = "0", Text = "--Select State--" });
            ViewBag.States = StateList;
            return View(model);
        }
        /// <summary>
        /// For update/edit architect profile post method.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> UpdateArchitectProfile(MemberShipRegistration model)
        {
            HttpResponseDetail<dynamic> response = new();
            response = await _staffRepository.UpdateArchitectProfileAsync(model);
            return Json(response);
        }
        /// <summary>
        /// For update or save entity type post method.
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
        /// For delete entity type
        /// </summary>
        /// <param name="EntityID"></param>
        /// <returns></returns>
        public async Task<JsonResult> DeleteEntityType(int EntityID)
        {
            return Json(await _globalMasterRepository.DeleteEntityTypeAsync(EntityID));
        }
        /// <summary>
        /// forward a mail with particular order doc from staff's copycenter screen/page.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> SendForwardMail(EmailViewModel model)
        {
            DisplayLoginInfo logInfo = new();
            logInfo = _commonRepository.GetUserInfo(User.Identity.Name);
            if (logInfo != null)
            {
                model.AuthorizedBy = logInfo.Name;
            }
            return Json(await _emailServiceManager.SendForwardMailAsync(model));
        }
        /// <summary>
        /// For get copy center price list.
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetCopyCenterPriceList()
        {
            return Json(await _commonRepository.GetCopyCenterPriceListAsync());
        }
        /// <summary>
        /// Show modal popup of View Order field of staffAccount/copyCenter
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public async Task<JsonResult> ViewOrderDoc(int OrderId)
        {
            return Json(await _staffRepository.GetViewOrderDocAsync(OrderId));
        }
        /// <summary>
        /// For bidding project on staff dashboard screen/page(Past Projects tab).
        /// </summary>
        /// <param name="ProjId"></param>
        /// <returns></returns>
        public async Task<JsonResult> BidResult(int ProjId)
        {
            var name = User.Identity.Name;
            return Json(await _staffRepository.BidResultAsync(ProjId, name));
        }
        /// <summary>
        /// Get Annual Membership Packages card details.
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetPacificCardDetails()
        {
            HttpResponseDetail<dynamic> response = new();
            List<tblPaymentCardDetail> model = new();
            model = await _commonRepository.GetPacificCardDetailsAsync();
            response.data = model;
            return Json(response);
        }
        /// <summary>
        /// For delete user member from staff's memberprofile(User Management section).
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResult> DeleteUserManagement(int id)
        {
            return Json(await _staffRepository.DeleteUserManagementAsync(id));
        }
        /// <summary>
        /// For going to renewal payment screen/page.
        /// </summary>
        /// <param name="PkgText"></param>
        /// <param name="Cost"></param>
        /// <param name="Id"></param>
        /// <param name="Term"></param>
        /// <param name="MemType"></param>
        /// <param name="DiscountId"></param>
        /// <returns></returns>
        public async Task<IActionResult> RenewalPayment(string PkgText, string Cost, string Id, string Term, string MemType, int DiscountId)
        {
            ViewBag.PkgText = PkgText;
            ViewBag.Cost = Cost;
            ViewBag.Id = Id;
            ViewBag.Term = Term;
            ViewBag.MemType = MemType;
            ViewBag.DiscountId = DiscountId;
            return View();
        }
        /// <summary>
        /// Not use
        /// </summary>
        /// <param name="modelJson"></param>
        /// <returns></returns>
        public async Task<ActionResult> SaveRenewalPayment(string modelJson)
        {
            HttpResponseDetail<dynamic> response = new();
            MemberShipRegistration MemRegModal = JsonConvert.DeserializeObject<MemberShipRegistration>(modelJson);
            try
            {

                Response.Cookies.Delete("renewalpayment");
                Response.Cookies.Delete("PayFor");
                if (MemRegModal.PayStatus == "Done")
                {
                    response = await _staffRepository.SaveRenewalPaymentAsync(MemRegModal);
                    TempData["SuccessMessage"] = "Payment successfully done.";
                    return RedirectToAction("MemberProfile", new { id = MemRegModal.ID });
                }

                else if (MemRegModal.PayStatus == "Customer Issue")
                {
                    TempData["SuccessMessage"] = "Customer issue payment not done.";
                    return RedirectToAction("MemberProfile", new { id = MemRegModal.ID });
                }
                else if (MemRegModal.PayStatus == "Item Issue")
                {
                    TempData["SuccessMessage"] = "Item issue payment not done.";
                    return RedirectToAction("MemberProfile", new { id = MemRegModal.ID });
                }
                else if (MemRegModal.PayStatus == "Invoice Issue")
                {
                    TempData["SuccessMessage"] = "Invoice issue payment not done.";
                    return RedirectToAction("MemberProfile", new { id = MemRegModal.ID });
                }
                else
                {
                    TempData["SuccessMessage"] = "Something went wrong";
                    return RedirectToAction("MemberProfile", new { id = MemRegModal.ID });
                }

            }
            catch (Exception ex)
            {
                TempData["SuccessMessage"] = "Something went wrong";
                return RedirectToAction("MemberProfile", new { id = MemRegModal.ID });
            }
        }
        /// <summary>
        /// For save renewal payment post method or update plan.
        /// </summary>
        /// <param name="modelJson"></param>
        /// <returns></returns>
        public async Task<ActionResult> SaveRenewalPayment1()
        {
            HttpResponseDetail<dynamic> response = new();
            string modelJson = Request.Cookies["renewalpaymentModel"] != null ? Request.Cookies["renewalpaymentModel"].ToString() : "";
            MemberShipRegistration MemRegModal = JsonConvert.DeserializeObject<MemberShipRegistration>(modelJson);
            try
            {

                Response.Cookies.Delete("renewalpaymentModel");
                Response.Cookies.Delete("renewalpayment");
                Response.Cookies.Delete("PayFor");
                if (MemRegModal.PayStatus.Contains("Done"))
                {
                    response = await _staffRepository.SaveRenewalPaymentAsync(MemRegModal);
                    TempData["SuccessMessage"] = "Payment successfully done.";
                    return RedirectToAction("MemberProfile", new { id = MemRegModal.ID });
                }

                else if (MemRegModal.PayStatus.Contains("Customer Issue"))
                {
                    TempData["SuccessMessage"] = "Customer issue payment not done.";
                    return RedirectToAction("MemberProfile", new { id = MemRegModal.ID });
                }
                else if (MemRegModal.PayStatus.Contains("Item Issue"))
                {
                    response = await _staffRepository.SaveErrorRenewalPaymentAsync(MemRegModal);
                    TempData["SuccessMessage"] = "Item issue payment not done.";
                    return RedirectToAction("MemberProfile", new { id = MemRegModal.ID });
                }
                else if (MemRegModal.PayStatus.Contains("Invoice Issue"))
                {
                    response = await _staffRepository.SaveErrorRenewalPaymentAsync(MemRegModal);
                    TempData["SuccessMessage"] = "Invoice issue payment not done.";
                    return RedirectToAction("MemberProfile", new { id = MemRegModal.ID });
                }
                else
                {
                    response = await _staffRepository.SaveErrorRenewalPaymentAsync(MemRegModal);
                    TempData["SuccessMessage"] = "Something went wrong";
                    return RedirectToAction("MemberProfile", new { id = MemRegModal.ID });
                }

            }
            catch (Exception ex)
            {
                response = await _staffRepository.SaveErrorRenewalPaymentAsync(MemRegModal);
                TempData["SuccessMessage"] = "Something went wrong";
                return RedirectToAction("MemberProfile", new { id = MemRegModal.ID });
            }

        }
        /// <summary>
        /// For delete member from membermanagement screen/page(Member tab).
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResult> DeleteMember(int id)
        {
            return Json(await _staffRepository.DeleteMemberAsync(id));
        }
        /// <summary>
        /// For going to new register member
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> NewRegMember(MemberShipRegistration model)
        {
            List<SelectListItem> StateList = await _membershipRepository.GetStates();
            StateList.Insert(0, new SelectListItem { Value = "0", Text = "--Select State--" });
            ViewBag.States = StateList;
            return View(model);
        }
        /// <summary>
        /// For get Annual Membership Packages card pakages name list
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetPKGList()
        {
            return Json(await _staffRepository.GetPKGListAsync());
        }
        /// <summary>
        /// For get Annual Membership Packages card pakages details.
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetPkgDetails(string pkg, string term)
        {
            dynamic response;
            response = await _staffRepository.GetPkgDetailsAsync(pkg, term);
            return Json(response);
        }
        /// <summary>
        /// For going to save new register member post method.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> SaveNewRegMember(MemberShipRegistration model)
        {
            dynamic response;
            try
            {
                model.ASPUserId = Guid.NewGuid();
                //var data = _chargebeeAPI.AddUserSubcriptionByStaff(model);
                //if (data.ToString() != "success")
                //{
                //    return Json("Something Went Wrong..");
                //}
            }
            catch (Exception)
            {
                return Json("Something Went Wrong..");
            }
            var user = new IdentityUser { Id = model.ASPUserId.ToString(), UserName = model.ContactEmail, Email = model.ContactEmail };
            var result = await _userManager.CreateAsync(user, model.ContactPassword);
            if (result.Succeeded)
            {

                response = await _staffRepository.SaveNewRegMemberAsync(model);
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
                await _emailServiceManager.SendEmail(emailObj);
                return Json(response);
            }
            return Json("Something Went Wrong..");
        }
        /// <summary>
        /// For going to save/register contractor post method.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> RegisterContractor(MemberShipRegistration model)
        {
            HttpResponseDetail<dynamic> response = new();
            response = await _staffRepository.RegContractor(model);
            return Json(response);
        }
        /// <summary>
        /// For permanentaly delete inactive member from membermanagement screen/page(Other Contact Tab/DataBase).
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResult> DeleteInactiveMember(int id)
        {
            return Json(await _staffRepository.DeleteInactiveMemberAsync(id));
        }
        /// <summary>
        /// Inactive/delete contractor from staff's membermanagement screen/page.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResult> DeleteContractor(int id)
        {
            return Json(await _staffRepository.DeleteContractorAsync(id));
        }
        /// <summary>
        /// Inactive/delete architect from staff's membermanagement screen/page.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResult> DeleteArchitect(int id)
        {
            return Json(await _staffRepository.DeleteArchitectAsync(id));
        }
        public async Task<JsonResult> UpdateGracePeriod(MemberShipRegistration model)
        {
            return Json(await _staffRepository.UpdateGracePeriodAsync(model));
        }
    }

}

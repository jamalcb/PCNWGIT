using DocumentFormat.OpenXml.Packaging;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PCNW.Data.Repository;
using PCNW.Helpers;
using PCNW.Models;
using PCNW.Models.ContractModels;
using PCNW.Repository;
using PCNW.ViewModel;
using System.Diagnostics;
using System.Net.Http.Headers;

namespace PCNW.Controllers
{
    [DisableRequestSizeLimit]
    public partial class HomeController : BaseController
    {
        private readonly IManageExsingUserRepository manageExsingUserRepository;
        private readonly ILogger<HomeController> _logger;
        private readonly ICommonRepository _commonRepository;
        private readonly IWebHostEnvironment Environment;
        private readonly IConfiguration _Configuration;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IImplementRepository _implementRepository;
        private readonly IGlobalMasterRepository _globalMasterRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly ApplicationDbContext _dbContext;
        private readonly IConfiguration _configuration;
        //private readonly ApplicationDbContext _applicationDbContext;
        private readonly string _fileUploadPath;

        public HomeController(IConfiguration configuration,IManageExsingUserRepository _manageExsingUserRepository, ILogger<HomeController> logger, ICommonRepository commonRepository, IWebHostEnvironment _Environment, IConfiguration Configuration, IMembershipRepository membershipRepository, IStaffRepository IStaffRepository, UserManager<IdentityUser> userManager, IImplementRepository IImplementRepository, IGlobalMasterRepository globalMasterRepository, IProjectRepository projectRepository, ApplicationDbContext dbContext)
        {
            manageExsingUserRepository = _manageExsingUserRepository;
            _logger = logger;
            _commonRepository = commonRepository;
            _Configuration = Configuration;
            //applicationDbContext = applicationDbContext;
            Environment = _Environment;
            _membershipRepository = membershipRepository;
            _staffRepository = IStaffRepository;
            _userManager = userManager;
            _implementRepository = IImplementRepository;
            _globalMasterRepository = globalMasterRepository;
            _projectRepository = projectRepository;
            _configuration = configuration;
           _dbContext = dbContext;
            _fileUploadPath = _configuration.GetSection("AppSettings")["FileUploadPath"]; ;
        }
        /// <summary>
        /// For going to home's index screen/page
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Index()
        {
            if (!string.IsNullOrEmpty(User.Identity.Name))
            {
                var user = await _userManager.FindByEmailAsync(User.Identity.Name);
                if (await _userManager.IsInRoleAsync(user, "Member"))
                {
                    DisplayLoginInfo res = _commonRepository.GetUserInfo(User.Identity.Name);
                    TempData["InActive"] = null;
                    if (res.InActive == true)
                    {
                        TempData["InActive"] = "Yes";
                    }
                }
            }
            if (TempData["Message"] != null)
            {
                ViewBag.SuccessMessage = TempData["Message"];
            }
            return View();
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <returns></returns>
        private async Task<dynamic> ManageExistingUser()
        {
            var resonse = await manageExsingUserRepository.ManageExistingUserAsync();
            return resonse;
        }
        /// <summary>
        /// For going to privacy screen/page.
        /// </summary>
        /// <returns></returns>
        public IActionResult Privacy()
        {
            return View();
        }
        /// <summary>
        /// For going to careers screen/page.
        /// </summary>
        /// <returns></returns>
        public IActionResult Careers()
        {

            return View();
        }
        /// <summary>
        /// For going to faq screen/page.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Faq()
        {
            IEnumerable<TblFAQ> model = await _membershipRepository.GetFaqAsync();
            foreach (var faq in model)
            {
                if (faq.Answer.Contains("&gt;"))
                    faq.Answer = faq.Answer.Replace("&gt;", ">");
                faq.Answer = faq.Answer.Contains("&lt;") ? faq.Answer.Replace("&lt;", "<") : faq.Answer;
                faq.Answer = faq.Answer.Contains("&quot;") ? faq.Answer.Replace("&quot;", "\"") : faq.Answer;
                faq.Answer = faq.Answer.Contains("&#039;") ? faq.Answer.Replace("&#039;", "\'") : faq.Answer;
                faq.Answer = faq.Answer.Contains("&amp;") ? faq.Answer.Replace("&amp;", "&") : faq.Answer;
                HtmlString str = new HtmlString(faq.Answer);
                faq.AnswerRaw = str;

            }
            return View(model);
        }
        /// <summary>
        /// For going to advertise screen/page.
        /// </summary>
        /// <returns></returns>
        public IActionResult Advertise()
        {
            return View();
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <returns></returns>
        public IActionResult FreeMemberNavigation()
        {
            MemberShipRegistration model = new();
            model.isFromFree = true;
            return RedirectToAction("Register", "Member", model);
        }
        /// <summary>
        /// For going to error screen/page.
        /// </summary>
        /// <returns></returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        /// <summary>
        /// For going to sendprojectfile screen/page.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public async Task<IActionResult> SendProjectFiles(string msg = "N")
        {
            MemberProjectInfo model = new();
            DisplayLoginInfo logInfo = new();
            logInfo = _commonRepository.GetUserInfo(User.Identity.Name);
            if (logInfo != null)
            {
                model.ContactMember = logInfo.Company;
                model.ContactName = logInfo.Name;
                model.ContactPhone = logInfo.Phone;
                model.memberId = logInfo.Id;
                model.ContactEmail = logInfo.Email;
            }
            model.ErrorMsg = msg;
            List<string> objList = new List<string>();
            objList.Add("New Construction");
            objList.Add("Remodel");
            objList.Add("Addition");
            model.ProjScopeList = objList;
            return View(model);
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <param name="T1"></param>
        /// <param name="T2"></param>
        /// <param name="T3"></param>
        /// <param name="T4"></param>
        /// <returns></returns>
        public async Task<IActionResult> UpdateBidStatus(string T1, string T2, string T3, string T4)
        {
            string appName = _Configuration.GetSection("AppSettings")["SecurityKey"];
            string DecProjId = EncryptDecryption.DecryptString(appName, T1);
            string DecEmail = EncryptDecryption.DecryptString(appName, T2);
            string DecStatus = EncryptDecryption.DecryptString(appName, T3);
            string DecTime = EncryptDecryption.DecryptString(appName, T4);
            if (DateTime.Now > Convert.ToDateTime(DecTime))
            {

                return View("Expired");
            }
            HttpResponseDetail<dynamic> response = await _commonRepository.ConfirmBiddingProj(DecProjId, DecEmail, DecStatus);
            if (response.success == true)
            {
                ViewBag.Message = response.data;
                return View();
            }
            else
            {
                return View("Error");
            }
        }
        /// <summary>
        /// For going to processerror screen/page.
        /// </summary>
        /// <returns></returns>
        public IActionResult ProcessError()
        {
            return View();
        }
        /// <summary>
        /// For going to expired screen/page(No use).
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Expired()
        {
            return View();
        }
        /// <summary>
        /// For going to copycenter screen/page(Non member copycenter page).
        /// </summary>
        /// <param name="LoadValChk"></param>
        /// <returns></returns>
        public async Task<ActionResult> CopyCenter(string LoadValChk = "NoValue")
        {
            var email = User.Identity.Name;
            if (email != null)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    if (await _userManager.IsInRoleAsync(user, "Staff"))
                    {
                        return RedirectToAction("CopyCenter", "StaffAccount");
                    }
                    if (await _userManager.IsInRoleAsync(user, "Member"))
                    {
                        DisplayLoginInfo info = _commonRepository.GetUserInfo(email);
                        if (info.InActive == false)
                            return RedirectToAction("CopyCenter", "Member");
                    }
                }
            }
            List<SelectListItem> StateList = await _membershipRepository.GetStates();
            StateList.Insert(0, new SelectListItem { Value = "0", Text = "--Select State--" });
            ViewBag.States = StateList;
            ViewBag.LoadValChk = LoadValChk;
            return View();
        }
        /// <summary>
        /// For going to copycenter post method.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> SaveCopyCenterInfo()
        {
            string modelJson = Request.Cookies["GetCopyModel"] != null ? Request.Cookies["GetCopyModel"] : "";
            string pathChK = Request.Cookies["pathChk"] != null ? Request.Cookies["pathChk"] : "";
            Response.Cookies.Delete("GetCopyModel");
            Response.Cookies.Delete("pathChk");
            HttpResponseDetail<dynamic> response = new();
            OrderTables CopyModel = JsonConvert.DeserializeObject<OrderTables>(modelJson);
            response = await _staffRepository.SaveCopyCenterInfoAsync(CopyModel);
            string Oid = response.data.OrderId.ToString();
            string pathPhoto = Path.Combine(this.Environment.WebRootPath, "Storage");
            string ToReplace = Path.Combine(pathPhoto, pathChK);
            string Replaced = Path.Combine(pathPhoto, Oid);
            Directory.Move(ToReplace, Replaced);
            return RedirectToAction("CopyCenter", new { LoadValChk = "success" });
        }
        /// <summary>
        /// For going to upload pdf/file post method.
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
        /// For Get Pacific Card Details
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
        /// For getspecial message on home index screen/page.
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetSpecialMsgList()
        {
            return Json(await _commonRepository.GetSpecialMsgAsync());
        }
        public async Task<JsonResult> GetSpecialMsgMain()
        {
            return Json(await _commonRepository.GetSpecialMsgMainAsync());
        }
        /// <summary>
        /// Get data for show/hide freetrial tab in payment card.
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetTabData()
        {
            HttpResponseDetail<dynamic> httpResponse = await _globalMasterRepository.GetTabData();
            return Json(httpResponse);
            //return View();
        }
        /// <summary>
        /// For going to legalstuff screen/page
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> LegalStuff()
        {
            return View();
        }
        /// <summary>
        /// For check county post method.
        /// </summary>
        /// <param name="City"></param>
        /// <param name="State"></param>
        /// <returns></returns>
        public async Task<JsonResult> CheckCounty(string City, string State)
        {
            return Json(await _projectRepository.CheckCountyAsync(City, State));
        }
        /// <summary>
        /// For count pdf/doc page post method.
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
            //for (var i = 0; i < pdfFile.Count; i++)
            //{
            //    int k = 1;
            //    for (var j = i + 1; j <= pdfFile.Count - 1; j++)
            //    {
            //        if (pdfFile[i].FileName == pdfFile[j].FileName)
            //        {
            //            string fileNamenew = k + "_" + pdfFile[j].FileName;
            //            k++;
            //            pdfFile[j] = new FormFile(pdfFile[j].OpenReadStream(), 0, pdfFile[j].Length, pdfFile[j].Name, fileNamenew)
            //            {
            //                Headers = pdfFile[j].Headers,
            //                ContentType = pdfFile[j].ContentType
            //            };
            //        }
            //    }
            //}

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
        /// To get projnumber(Member's PostProjectHere and SendProjectFiles page).
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult GetProjectCode()
        {
            HttpResponseDetail<dynamic> response = new();
            var result = _projectRepository.GetProjectCodeAsync();
            response.data = result;
            response.success = true;
            return Json(response);
        }
        /// <summary>
        /// Check unique email post method.
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
        /// For upload pdf/file post method.
        /// </summary>
        /// <param name="pdfFile"></param>
        /// <param name="name"></param>
        /// <param name="email"></param>
        /// <param name="member"></param>
        /// <param name="title"></param>
        /// <param name="phone"></param>
        /// <param name="projNum"></param>
        /// <returns></returns>
        public async Task<JsonResult> UploadPdf(List<IFormFile> pdfFile, string name, string email, string member, string title, string phone, string projNum)
        {
            HttpResponseDetail<dynamic> httpResponse = new();
            MemberProjectInfo model = new();
            model.ProjNumber = projNum;
            model.Title = title;
            model.ContactName = name;
            if (pdfFile.Count > 0)
            {
                string username = "Contact.txt";
                string pathPhoto = Path.Combine(this.Environment.WebRootPath, "PostProject");
                pathPhoto = Path.Combine(pathPhoto, model.ProjNumber);
                pathPhoto = Path.Combine(pathPhoto, "Uploads");
                using (FileStream stream1 = new FileStream(Path.Combine(pathPhoto, username), FileMode.Create))
                {
                    stream1.Close();
                    System.IO.File.WriteAllText(Path.Combine(pathPhoto, username), "Project Name : " + title + "\n" + "Name : " + name + "\n" + "Contact Email : " + email + "\n" + "Contact Member : " + member + "\n" + "Contact Phone : " + phone);
                }
                string rootfolder = model.ProjNumber.Substring(0, 2);
                rootfolder = rootfolder.Replace(rootfolder, "20" + rootfolder);
                string subRootfolder = model.ProjNumber.Substring(2, 2);
                await CreateFolder(rootfolder + "/" + subRootfolder + "/" + model.ProjNumber + "/Uploads");
                await CreateFolder(rootfolder + "/" + subRootfolder + "/" + model.ProjNumber + "/Addenda");
                await CreateFolder(rootfolder + "/" + subRootfolder + "/" + model.ProjNumber + "/Bid Results");
                await CreateFolder(rootfolder + "/" + subRootfolder + "/" + model.ProjNumber + "/PHL");
                await CreateFolder(rootfolder + "/" + subRootfolder + "/" + model.ProjNumber + "/Plans");
                await CreateFolder(rootfolder + "/" + subRootfolder + "/" + model.ProjNumber + "/Specs");
                string[] fileArray = Directory.GetFiles(pathPhoto);

                httpResponse = await _commonRepository.SendProjectFilesAsync(model);
                httpResponse.data = pathPhoto;
                httpResponse.statusMessage = model.ProjNumber;
            }

            return Json(httpResponse);
        }
        /// <summary>
        /// Demo file upload for progress file/pdf upload status.
        /// </summary>
        /// <param name="files"></param>
        /// <param name="ProjNum"></param>
        /// <returns></returns>
        [HttpPost]
        [RequestSizeLimit(500 * 1024 * 1024)]
        [RequestFormLimits(MultipartBodyLengthLimit = 500 * 1024 * 1024)]
        public async Task<JsonResult> DemoFileUpload(IList<IFormFile> files, string ProjNum, string firstfile = null)
        {
            HttpResponseDetail<dynamic> httpResponse = new();
            string pathToEx = "";
            Startup.Progress = 0;
            Startup.FileCount = 0;
            long totalBytes = files.Sum(f => f.Length);
            long totalReadBytes = 0;
            try
            {
                foreach (IFormFile source in files)
                {
                    string filename = ContentDispositionHeaderValue.Parse(source.ContentDisposition).FileName.ToString().Trim('"');

                    filename = this.EnsureCorrectFilename(filename);
                    string path = Environment.WebRootPath + "\\PostProject\\" + ProjNum;

                    if (Directory.Exists(path))
                    {
                        if (!String.IsNullOrEmpty(firstfile) && firstfile == "0")
                        {
                            Directory.Delete(path, true);
                            Directory.CreateDirectory(path);
                        }
                    }
                    else
                    {
                        Directory.CreateDirectory(path);
                    }
                    using (FileStream stream = new FileStream(Path.Combine(path, filename), FileMode.Create))
                    {
                        source.CopyTo(stream);
                    }
                    ////string pathPhoto = this.GetPathAndFilename(filename, ProjNum);
                    //byte[] buffer = new byte[16 * 1024];
                    //using (FileStream output = System.IO.File.Create(this.GetPathAndFilename(filename, ProjNum)))
                    //{
                    //    using (Stream input = source.OpenReadStream())
                    //    {

                    //        int readBytes;

                    //        while ((readBytes = input.Read(buffer, 0, buffer.Length)) > 0)
                    //        {
                    //            await output.WriteAsync(buffer, 0, readBytes);
                    //            totalReadBytes += readBytes;
                    //            Startup.Progress = (int)((float)totalReadBytes / (float)totalBytes * 100.0);
                    //            await Task.Delay(1000); // It is only to make the process slower
                    //        }
                    //    }
                    //}
                    Startup.FileCount = Startup.FileCount + 1;
                }
                httpResponse.success = true;
                httpResponse.data = Environment.WebRootPath + "\\PostProject\\" + ProjNum;
            }
            catch (Exception ex)
            {
                httpResponse.success = false;
                httpResponse.statusCode = "404";
                _logger.LogWarning(ex.Message);
            }

            return Json(httpResponse);
        }
        /// <summary>
        /// For send pdf/file to aws s3
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> SendToS3(MemberProjectInfo model)
        {
            HttpResponseDetail<dynamic> httpResponse = await _commonRepository.SendProjectFilesAsync(model);
            if (!httpResponse.success)
            {
                httpResponse.statusMessage = "Problem Caused";
                return Json(httpResponse);
            }
            try
            {
                string year = "20" + httpResponse.data.ProjNumber.Substring(0, 2);
                string month = httpResponse.data.ProjNumber.Substring(2, 2);
                string projNumber = httpResponse.data.ProjNumber;

                // Define the base path for saving locally
                string basePath = _fileUploadPath;

                // Create the main project directory structure
                string projectPath = Path.Combine(basePath, year, month, projNumber);

                // Create necessary directories locally
                LocalCreateFolder(Path.Combine(projectPath, "Uploads"));
                LocalCreateFolder(Path.Combine(projectPath, "Addenda"));
                LocalCreateFolder(Path.Combine(projectPath, "Bid Results"));
                LocalCreateFolder(Path.Combine(projectPath, "PHL"));
                LocalCreateFolder(Path.Combine(projectPath, "Plans"));
                LocalCreateFolder(Path.Combine(projectPath, "Specs"));

                // Create and write to the Contact.txt file
                string username = "Contact.txt";
                string contactFilePath = Path.Combine(model.LocalPath, username);
                using (FileStream stream1 = new FileStream(contactFilePath, FileMode.Create))
                {
                    using (StreamWriter writer = new StreamWriter(stream1))
                    {
                        writer.WriteLine("Project Name : " + model.Title);
                        writer.WriteLine("Name : " + model.ContactName);
                        writer.WriteLine("Contact Email : " + model.ContactEmail);
                        writer.WriteLine("Contact Phone : " + model.ContactPhone);
                    }
                }

                // Save files to the Uploads directory
                string[] fileArray = Directory.GetFiles(model.LocalPath);
                foreach (string s in fileArray)
                {
                    SaveToLocalPath(s, Path.Combine(projectPath, "Uploads"));
                }

                // Clean up the local path
                if (Directory.Exists(model.LocalPath))
                {
                    Directory.Delete(model.LocalPath, true);
                }

                return Json(new { Status = "success" });
            }
            catch (Exception ex)
            {
                // Return the exception message as a JSON response
                return Json(new { Status = "error", statusmessage = ex.Message });
            }
        }
        private void LocalCreateFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public void SaveToLocalPath(string sourceFilePath, string destinationFolder)
        {
            try
            {
                // Ensure the destination folder exists
                if (!Directory.Exists(destinationFolder))
                {
                    Directory.CreateDirectory(destinationFolder);
                }

                // Get the file name from the source file path
                string fileName = Path.GetFileName(sourceFilePath);

                // Define the destination path
                string destFilePath = Path.Combine(destinationFolder, fileName);

                // Copy the file using FileStream
                using (FileStream sourceStream = new FileStream(sourceFilePath, FileMode.Open, FileAccess.Read))
                {
                    using (FileStream destStream = new FileStream(destFilePath, FileMode.Create, FileAccess.Write))
                    {
                        sourceStream.CopyTo(destStream);
                    }
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions
                Console.WriteLine(ex.Message);
            }
        }
        #region for upload pdf/file progress status
        /// <summary>
        /// For pdf progress status post method.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public JsonResult Progress()
        {
            return Json(new { progress = Startup.Progress.ToString(), Count = Startup.FileCount.ToString() });
        }

        private string EnsureCorrectFilename(string filename)
        {
            if (filename.Contains("\\"))
                filename = filename.Substring(filename.LastIndexOf("\\") + 1);

            return filename;
        }

        private string GetPathAndFilename(string filename, string ProjNum)
        {
            string path = Environment.WebRootPath + "\\PostProject\\" + ProjNum + "\\";

            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            return path + filename;
        }
        #endregion
        /// <summary>
        /// For going to timepicker screen/page.
        /// </summary>
        /// <returns></returns>
        public IActionResult TimePicker()
        {
            return View();
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
        /// Get delivery dropdown list for copycenter screen/page.
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetDeliveryList()
        {
            return Json(await _commonRepository.GetDeliveryListAsync());
        }
        public async Task<ActionResult> CopyCenter1(string LoadValChk = "NoValue")
        {
            var email = User.Identity.Name;
            if (email != null)
            {
                var user = await _userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    if (await _userManager.IsInRoleAsync(user, "Staff"))
                    {
                        return RedirectToAction("CopyCenter", "StaffAccount");
                    }
                    if (await _userManager.IsInRoleAsync(user, "Member"))
                    {
                        return RedirectToAction("CopyCenter", "Member");
                    }
                }
            }
            List<SelectListItem> StateList = await _membershipRepository.GetStates();
            StateList.Insert(0, new SelectListItem { Value = "0", Text = "--Select State--" });
            ViewBag.States = StateList;
            ViewBag.LoadValChk = LoadValChk;
            return View();
        }
    }
}
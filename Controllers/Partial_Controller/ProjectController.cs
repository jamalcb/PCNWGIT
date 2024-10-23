using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PCNW.Data.Repository;
using PCNW.Helpers;
using PCNW.Models;
using PCNW.Models.ContractModels;
using PCNW.Models.ProcessContracts;
using PCNW.Services;
using PCNW.ViewModel;
using System.Collections;
using System.Net;

namespace PCNW.Controllers
{
    [Authorize(Roles = "Staff,Admin,Member")]
    public class ProjectController : BaseController
    {
        private readonly IProjectRepository _projectRepository;
        private readonly ILogger<ProjectController> _logger;
        private readonly ApplicationDbContext _applicationDbContext;
        private readonly ApplicationDbContext _dbContext;
        private readonly ICommonRepository _commonRepository;
        private readonly IEmailServiceManager _emailServiceManager;
        private readonly IWebHostEnvironment Environment;
        private readonly IGlobalRepository _globalRepository;
        private readonly IMembershipRepository _membershipRepository;
        private readonly IEntityRepository _entityRepository;
        private readonly IConfiguration _configuration;
        private readonly string _fileUploadPath;
        public ProjectController(IConfiguration configuration, IProjectRepository projectRepository, ILogger<ProjectController> logger, ApplicationDbContext applicationDbContext, ApplicationDbContext dbContex, ICommonRepository commonRepository, IEmailServiceManager emailServiceManager, IWebHostEnvironment _Environment, IGlobalRepository globalRepository, IMembershipRepository membershipRepository, IEntityRepository entityRepository)
        {
            _applicationDbContext = applicationDbContext;
            _projectRepository = projectRepository;
            //_awsAccessKey = awsAccessKey;
            _logger = logger;
            _dbContext = dbContex;
            _commonRepository = commonRepository;
            _emailServiceManager = emailServiceManager;
            Environment = _Environment;
            _globalRepository = globalRepository;
            _membershipRepository = membershipRepository;
            _entityRepository = entityRepository;
           _configuration = configuration;
            _fileUploadPath = _configuration.GetSection("AppSettings")["FileUploadPath"]; 
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <returns></returns>
        public IActionResult Index()
        {
            return View();
        }
        /// <summary>
        /// Save project and remain on the same project or in another word go to the edit mode for the same project
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveProjectInfoEdit(ProjectInformation model)
        {
            int Id = 0;
            try
            {
                model.Contact = User.Identity.Name;
                Id = await _projectRepository.SaveProjectInfoAsync(model);
                //string rootFolder = model.ProjNumber.Substring(0, 2);
                //rootFolder = rootFolder.Replace(rootFolder, "20" + rootFolder);
                //string subRootFolder = model.ProjNumber.Substring(2, 2);
                //await CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Addenda");
                //await CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Bid Results");
                //await CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/PHL");
                //await CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Plans");
                //await CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Specs");
                string year = "20" + model.ProjNumber.Substring(0, 2);
                string month = model.ProjNumber.Substring(2, 2);
                string projNumber = model.ProjNumber;

                // Define the base path for saving locally
                string basePath = _fileUploadPath;

                // Create the main project directory structure
                string projectPath = Path.Combine(basePath, year, month, projNumber);

                // Create necessary directories locally
                LocalCreateFolder(Path.Combine(projectPath, "Addenda"));
                LocalCreateFolder(Path.Combine(projectPath, "Bid Results"));
                LocalCreateFolder(Path.Combine(projectPath, "PHL"));
                LocalCreateFolder(Path.Combine(projectPath, "Plans"));
                LocalCreateFolder(Path.Combine(projectPath, "Specs"));
            }
            catch (Exception Ex) { }
            ProjectInformation model1 = new();
            List<string> objList = new List<string>();
            objList.Add("New Construction");
            objList.Add("Remodel");
            objList.Add("Addition");
            model1.ProjScopeList = objList;
            string strModel = JsonConvert.SerializeObject(model);
            if (Id == 0)
            {
                return RedirectToAction("EditProjectInfo", new { id = Id, modelInfo = strModel });
            }
            else
            {
                return RedirectToAction("EditProjectInfo", new { id = Id });
            }
        }
        /// <summary>
        /// Save Project then go back to dashboard
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveProjectInfoNew(ProjectInformation model)
        {
            int Id = 0;
            try
            {
                model.Contact = User.Identity.Name;
                Id = await _projectRepository.SaveProjectInfoAsync(model);
                //string rootFolder = model.ProjNumber.Substring(0, 2);
                //rootFolder = rootFolder.Replace(rootFolder, "20" + rootFolder);
                //string subRootFolder = model.ProjNumber.Substring(2, 2);
                ////bool test = await DoesFolderExist(rootFolder + "/" + subRootFolder + "/");
                ////if(!test)
                //await CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Addenda");
                //await CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Bid Results");
                //await CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/PHL");
                //await CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Plans");
                //await CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Specs");

                string year = "20" + model.ProjNumber.Substring(0, 2);
                string month = model.ProjNumber.Substring(2, 2);
                string projNumber = model.ProjNumber;

                // Define the base path for saving locally
                string basePath = _fileUploadPath;

                // Create the main project directory structure
                string projectPath = Path.Combine(basePath, year, month, projNumber);

                // Create necessary directories locally
                LocalCreateFolder(Path.Combine(projectPath, "Addenda"));
                LocalCreateFolder(Path.Combine(projectPath, "Bid Results"));
                LocalCreateFolder(Path.Combine(projectPath, "PHL"));
                LocalCreateFolder(Path.Combine(projectPath, "Plans"));
                LocalCreateFolder(Path.Combine(projectPath, "Specs"));
            }
            catch (Exception Ex) { }
            ProjectInformation model1 = new();
            List<string> objList = new List<string>();
            objList.Add("New Construction");
            objList.Add("Remodel");
            objList.Add("Addition");
            model1.ProjScopeList = objList;
            string strModel = JsonConvert.SerializeObject(model);
            if (Id == 0)
            {
                return RedirectToAction("EditProjectInfo", new { id = Id, modelInfo = strModel });
            }
            else
            {
                return RedirectToAction("EditProjectInfo", "Project");
            }
        }
        private void LocalCreateFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        /// <summary>
        /// Save project and go back to dashboard
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveProjectInfoClose(ProjectInformation model)
        {
            int Id = 0;
            try
            {
                model.Contact = User.Identity.Name;
                Id = await _projectRepository.SaveProjectInfoAsync(model);
                //string rootFolder = model.ProjNumber.Substring(0, 2);
                //rootFolder = rootFolder.Replace(rootFolder, "20" + rootFolder);
                //string subRootFolder = model.ProjNumber.Substring(2, 2);
                ////bool test = await DoesFolderExist(rootFolder + "/" + subRootFolder + "/" + sample);
                ////if(!test)
                //await CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Addenda");
                //await CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Bid Results");
                //await CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/PHL");
                //await CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Plans");
                //await CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Specs");
                string year = "20" + model.ProjNumber.Substring(0, 2);
                string month = model.ProjNumber.Substring(2, 2);
                string projNumber = model.ProjNumber;

                // Define the base path for saving locally
                string basePath = _fileUploadPath;

                // Create the main project directory structure
                string projectPath = Path.Combine(basePath, year, month, projNumber);

                // Create necessary directories locally
                LocalCreateFolder(Path.Combine(projectPath, "Addenda"));
                LocalCreateFolder(Path.Combine(projectPath, "Bid Results"));
                LocalCreateFolder(Path.Combine(projectPath, "PHL"));
                LocalCreateFolder(Path.Combine(projectPath, "Plans"));
                LocalCreateFolder(Path.Combine(projectPath, "Specs"));
            }
            catch (Exception Ex) { }
            ProjectInformation model1 = new();
            List<string> objList = new List<string>();
            objList.Add("New Construction");
            objList.Add("Remodel");
            objList.Add("Addition");
            model1.ProjScopeList = objList;
            string strModel = JsonConvert.SerializeObject(model);
            if (Id == 0)
            {
                return RedirectToAction("EditProjectInfo", new { id = Id, modelInfo = strModel });
            }
            else
            {
                return RedirectToAction("Dashboard", "StaffAccount");
            }
        }
        [HttpPost]
        public async Task<IActionResult> SaveProjectInfoCopy(ProjectInformation model)
        {
            int Id = 0;
            try
            {
                model.Contact = User.Identity.Name;
                Id = await _projectRepository.SaveProjectInfoAsync(model);
                //string rootFolder = model.ProjNumber.Substring(0, 2);
                //rootFolder = rootFolder.Replace(rootFolder, "20" + rootFolder);
                //string subRootFolder = model.ProjNumber.Substring(2, 2);
                ////bool test = await DoesFolderExist(rootFolder + "/" + subRootFolder + "/" + sample);
                ////if(!test)
                //await CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Addenda");
                //await CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Bid Results");
                //await CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/PHL");
                //await CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Plans");
                //await CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Specs");

                string year = "20" + model.ProjNumber.Substring(0, 2);
                string month = model.ProjNumber.Substring(2, 2);
                string projNumber = model.ProjNumber;

                // Define the base path for saving locally
                string basePath = _fileUploadPath;

                // Create the main project directory structure
                string projectPath = Path.Combine(basePath, year, month, projNumber);

                // Create necessary directories locally
                LocalCreateFolder(Path.Combine(projectPath, "Addenda"));
                LocalCreateFolder(Path.Combine(projectPath, "Bid Results"));
                LocalCreateFolder(Path.Combine(projectPath, "PHL"));
                LocalCreateFolder(Path.Combine(projectPath, "Plans"));
                LocalCreateFolder(Path.Combine(projectPath, "Specs"));

            }
            catch (Exception Ex) { }
            ProjectInformation model1 = new();
            List<string> objList = new List<string>();
            objList.Add("New Construction");
            objList.Add("Remodel");
            objList.Add("Addition");
            model.ProjScopeList = objList;
            string strModel = JsonConvert.SerializeObject(model);
            if (Id == 0)
            {
                return RedirectToAction("EditProjectInfo", new { id = Id, modelInfo = strModel });
            }
            else
            {
                return RedirectToAction("EditProjectInfo", new { id = Id, chk = "Copy" });
            }
        }
        /// <summary>
        /// Update Project and remain on same window
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateProjectInfoEdit(ProjectInformation model)
        {
            HttpResponseDetail<dynamic> httpResponse = new();
            string strModel = JsonConvert.SerializeObject(model);
            string user = User.Identity.Name;
            DisplayLoginInfo info = new();
            if (user != null)
            {
                info = await _commonRepository.GetUserInfoAsync(user);
                try
                {
                    httpResponse = await _projectRepository.UpdateProjectInfoAsync(model);

                }
                catch (Exception Ex)
                {
                    _logger.LogWarning(Ex.Message);
                }
                if (httpResponse.data != null)
                {
                    TempData["SuccessMessage"] = "Information for Project number: " + model.ProjNumber + " updated";
                    if (httpResponse.success == true)
                    {
                        httpResponse.data.REMOTE_ADDR = GetIPAddressOfClient();
                        httpResponse.data.AuthorizedBy = info.Name;
                        EmailViewModel emailObj = new();
                        await _emailServiceManager.ChangeBidDateToTracking(emailObj, httpResponse.data);
                        emailObj.EmailTos = httpResponse.data.EmailsTo;
                        var response = await _emailServiceManager.SendEmail(emailObj);
                    }
                    if (model.ProjId > 0 && model.Publish)
                    {
                        var message = await getchangesmessage(model.ProjId, model.Title);
                        if (message != null)
                        {
                            EmailViewModel emailObj = new();
                            emailObj.strMessage = message;
                            var data = await _emailServiceManager.Updateproject(emailObj, model);
                            if (data.EmailTos.Count > 0)
                            {
                                var response = await _emailServiceManager.SendEmail(data);
                            }
                        }
                    }
                    return RedirectToAction("EditProjectInfo", new { id = model.ProjId });

                }
                else
                {
                    TempData["ErrorMessage"] = "Information for Project number: " + model.ProjNumber + " not updated";
                    return RedirectToAction("EditProjectInfo", new { id = model.ProjId });
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Information for Project number: " + model.ProjNumber + " not updated";
                return RedirectToAction("EditProjectInfo", new
                {
                    id = model.ProjId,
                    modelInfo = strModel
                });
            }
        }

        public async Task<string?> getchangesmessage(int ProjId, string Title)
        {
            IEnumerable<TblCounty> lstCounty = await _globalRepository.GetCounties();
            ProjectInformation projectmodel = null;
            if (ProjId > 0)
            {
                projectmodel = await _projectRepository.GetProjectdetail(ProjId, lstCounty);
            }
            if (HttpContext.Session.GetString("ProjectInfo") != null && projectmodel != null)
            {
                var data = JsonConvert.DeserializeObject<ProjectInformation>(HttpContext.Session.GetString("ProjectInfo"));

                var updatedProperties = new List<string>();

                foreach (var property in typeof(ProjectInformation).GetProperties())
                {

                    var propertyName = property.Name;
                    var currentValue = property.GetValue(projectmodel);
                    var previousValue = property.GetValue(data);
                    if (new[] { "Title", "Entities", "preBidInfos", "BidDt", "Undecided", "AddendaNote", "BRNote", "BidResultsFiles", "EstCostDetails", "SubApprov", "BidBond", "PrevailingWage", "AddendaInformation", "Story" }.Contains(propertyName))
                    {
                        if (propertyName == "Entities" || propertyName == "preBidInfos" || propertyName == "pInfo" || propertyName == "phlInfo" || propertyName == "EstCostDetails" ||
                        propertyName == "BidDateTimes" || propertyName == "AddendaInformation" || propertyName == "AddendaS3Files" || propertyName == "AddendaDIRInfos" ||
                        propertyName == "BidResultsFiles" || propertyName == "PHLFiles" || propertyName == "PlansFiles" || propertyName == "SpecsFiles")
                        {
                            // Check for null values to avoid NullReferenceException
                            if (currentValue != null && previousValue != null)
                            {
                                var currentList = (IEnumerable<object>)currentValue;
                                var previousList = (IEnumerable<object>)previousValue;

                                // Check if counts are different
                                if (currentList.Count() != previousList.Count())
                                {
                                    updatedProperties.Add(propertyName);
                                }
                                else
                                {
                                    // Iterate through each item in the lists and compare properties
                                    var areListsDifferent = false;
                                    var currentEnumerator = currentList.GetEnumerator();
                                    var previousEnumerator = previousList.GetEnumerator();

                                    while (currentEnumerator.MoveNext() && previousEnumerator.MoveNext())
                                    {
                                        var currentElement = currentEnumerator.Current;
                                        var previousElement = previousEnumerator.Current;

                                        // Compare properties of current and previous elements
                                        foreach (var innerProperty in currentElement.GetType().GetProperties())
                                        {
                                            var innerPropertyName = innerProperty.Name;
                                            var currentInnerValue = innerProperty.GetValue(currentElement);
                                            var previousInnerValue = innerProperty.GetValue(previousElement);

                                            // Compare inner properties
                                            if (currentInnerValue != null && previousInnerValue != null && !currentInnerValue.Equals(previousInnerValue))
                                            {
                                                areListsDifferent = true;
                                                break; // Exit inner loop if a difference is found
                                            }
                                        }

                                        // Exit outer loop if a difference is found
                                        if (areListsDifferent)
                                        {
                                            break;
                                        }
                                    }

                                    // If lists are different, add the propertyName to updatedProperties
                                    if (areListsDifferent)
                                    {
                                        updatedProperties.Add(propertyName);
                                    }
                                }
                            }
                        }

                        else
                        {
                            // Explicitly exclude certain list properties if they are empty
                            if ((propertyName == "Entities" || propertyName == "preBidInfos" || propertyName == "pInfo" || propertyName == "phlInfo" || propertyName == "EstCostDetails" ||
                            propertyName == "BidDateTimes" || propertyName == "AddendaInformation" || propertyName == "AddendaS3Files" || propertyName == "AddendaDIRInfos" ||
                            propertyName == "BidResultsFiles" || propertyName == "PHLFiles" || propertyName == "PlansFiles" || propertyName == "SpecsFiles") &&
                            (currentValue as IEnumerable)?.Cast<object>().Any() == false && (previousValue as IEnumerable)?.Cast<object>().Any() == false)
                            {
                                continue;
                            }

                            if (currentValue is IEnumerable && previousValue is IEnumerable)
                            {
                                var currentList = ((IEnumerable)currentValue).Cast<object>().ToList();
                                var previousList = ((IEnumerable)previousValue).Cast<object>().ToList();

                                // If the counts are different, lists are considered different
                                if (currentList.Count != previousList.Count)
                                {
                                    updatedProperties.Add(propertyName);
                                    continue;
                                }

                                // Compare elements individually
                                for (int i = 0; i < currentList.Count; i++)
                                {
                                    var currentElement = currentList[i];
                                    var previousElement = previousList[i];

                                    if (!currentElement.Equals(previousElement))
                                    {
                                        updatedProperties.Add(propertyName);
                                        break; // Move to the next property
                                    }
                                }
                            }
                            else if (currentValue != null && previousValue != null && !currentValue.Equals(previousValue))
                            {
                                updatedProperties.Add(propertyName);
                            }
                            else if ((currentValue != null && previousValue == null) || (currentValue == null && previousValue != null))
                            {
                                updatedProperties.Add(propertyName);
                            }
                        }
                    }
                }

                // Check if any properties have been updated
                if (updatedProperties.Any())
                {
                    if (updatedProperties.Contains("BidDt") && updatedProperties.Contains("Undecided"))
                    {
                        updatedProperties.Remove("Undecided");
                    }
                    // Mapping of property names to their descriptions
                    var propertyDescriptions = new Dictionary<string, string>
                            {
                                { "BidDt", "Bid Date/Time" },
                                { "Undecided", "Bid Date/Time" },
                                { "preBidInfos", "PreBid Date/Time" },
                                { "Title", "Project Title" },
                                { "Story", "Project Story" },
                                { "EstCostDetails", "Estimated Costs" },
                                { "Entities", "Project Team" },
                                { "BRNote", "Bid Result" },
                                { "AddendaNote", "Addenda" },
                                { "BidBond", "Bid Bond" },
                                { "PrevailingWage", "Prevailing Wage" },
                                { "SubApprov", "Substitution Approval" }
                            };

                    // Generate the message indicating the updated properties
                    var updatedPropertiesString = string.Join(", ", updatedProperties
                        .Select(prop => propertyDescriptions.ContainsKey(prop) ? $"{propertyDescriptions[prop]}" : prop));

                    return $"The {updatedPropertiesString} has been updated for the Project: ''{Title}''";
                }

            }
            return null;
        }
        /// <summary>
        /// Update project then go back to dashboard
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateProjectInfoClose(ProjectInformation model)
        {
            HttpResponseDetail<dynamic> httpResponse = new();
            string strModel = JsonConvert.SerializeObject(model);
            string user = User.Identity.Name;
            DisplayLoginInfo info = new();
            if (user != null)
            {
                info = await _commonRepository.GetUserInfoAsync(user);
                try
                {
                    httpResponse = await _projectRepository.UpdateProjectInfoAsync(model);
                }
                catch (Exception Ex)
                {
                    _logger.LogWarning(Ex.Message);
                }
                if (httpResponse.data != null)
                {
                    TempData["SuccessMessage"] = "Information for Project number: " + model.ProjNumber + " updated";
                    if (httpResponse.success == true)
                    {
                        httpResponse.data.REMOTE_ADDR = GetIPAddressOfClient();
                        httpResponse.data.AuthorizedBy = info.Name;
                        EmailViewModel emailObj = new();
                        await _emailServiceManager.ChangeBidDateToTracking(emailObj, httpResponse.data);
                        emailObj.EmailTos = httpResponse.data.EmailsTo;
                        var response = await _emailServiceManager.SendEmail(emailObj);
                    }
                    if (model.ProjId > 0 && model.Publish)
                    {
                        var message = await getchangesmessage(model.ProjId, model.Title);
                        if (message != null)
                        {
                            EmailViewModel emailObj = new();
                            emailObj.strMessage = message;
                            var data = await _emailServiceManager.Updateproject(emailObj, model);
                            if (data.EmailTos.Count > 0)
                            {
                                var response = await _emailServiceManager.SendEmail(data);
                            }
                        }
                    }
                    return RedirectToAction("Dashboard", "StaffAccount");

                }
                else
                {
                    TempData["ErrorMessage"] = "Information for Project number: " + model.ProjNumber + " not updated";
                    return RedirectToAction("EditProjectInfo", new { id = model.ProjId, modelInfo = strModel });
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Information for Project number: " + model.ProjNumber + " not updated";
                return RedirectToAction("EditProjectInfo", new { id = model.ProjId, modelInfo = strModel });
            }
        }
        /// <summary>
        /// For updating the project then open a new project mode
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateProjectInfoNew(ProjectInformation model)
        {
            HttpResponseDetail<dynamic> httpResponse = new();
            string strModel = JsonConvert.SerializeObject(model);
            string user = User.Identity.Name;
            DisplayLoginInfo info = new();
            if (user != null)
            {
                info = await _commonRepository.GetUserInfoAsync(user);
                try
                {
                    httpResponse = await _projectRepository.UpdateProjectInfoAsync(model);
                }
                catch (Exception Ex)
                {
                    _logger.LogWarning(Ex.Message);
                }
                if (httpResponse.data != null)
                {
                    TempData["SuccessMessage"] = "Information for Project number: " + model.ProjNumber + " updated";
                    if (httpResponse.success == true)
                    {
                        httpResponse.data.REMOTE_ADDR = GetIPAddressOfClient();
                        httpResponse.data.AuthorizedBy = info.Name;
                        EmailViewModel emailObj = new();
                        await _emailServiceManager.ChangeBidDateToTracking(emailObj, httpResponse.data);
                        emailObj.EmailTos = httpResponse.data.EmailsTo;
                        var response = await _emailServiceManager.SendEmail(emailObj);
                    }
                    if (model.ProjId > 0 && model.Publish)
                    {
                        var message = await getchangesmessage(model.ProjId, model.Title);
                        if (message != null)
                        {
                            EmailViewModel emailObj = new();
                            emailObj.strMessage = message;
                            var data = await _emailServiceManager.Updateproject(emailObj, model);
                            if (data.EmailTos.Count > 0)
                            {
                                var response = await _emailServiceManager.SendEmail(data);
                            }
                        }
                    }
                    return RedirectToAction("EditProjectInfo", new { id = 0 });

                }
                else
                {
                    TempData["ErrorMessage"] = "Information for Project number: " + model.ProjNumber + " not updated";
                    return RedirectToAction("EditProjectInfo", new { id = model.ProjId, modelInfo = strModel });
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Information for Project number: " + model.ProjNumber + " not updated";
                return RedirectToAction("EditProjectInfo", new { id = model.ProjId, modelInfo = strModel });
            }
        }
        /// <summary>
        /// Update the project then go to new project window
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> UpdateProjectInfoCopy(ProjectInformation model)
        {
            HttpResponseDetail<dynamic> httpResponse = new();
            string strModel = JsonConvert.SerializeObject(model);
            string user = User.Identity.Name;
            DisplayLoginInfo info = new();
            if (user != null)
            {
                info = await _commonRepository.GetUserInfoAsync(user);
                try
                {
                    httpResponse = await _projectRepository.UpdateProjectInfoAsync(model);
                }
                catch (Exception Ex)
                {
                    _logger.LogWarning(Ex.Message);
                }
                if (httpResponse.data != null)
                {
                    TempData["SuccessMessage"] = "Information for Project number: " + model.ProjNumber + " updated";
                    if (httpResponse.success == true)
                    {
                        httpResponse.data.REMOTE_ADDR = GetIPAddressOfClient();
                        httpResponse.data.AuthorizedBy = info.Name;
                        EmailViewModel emailObj = new();
                        await _emailServiceManager.ChangeBidDateToTracking(emailObj, httpResponse.data);
                        emailObj.EmailTos = httpResponse.data.EmailsTo;
                        var response = await _emailServiceManager.SendEmail(emailObj);
                    }
                    if (model.ProjId > 0 && model.Publish)
                    {
                        var message = await getchangesmessage(model.ProjId, model.Title);
                        if (message != null)
                        {
                            EmailViewModel emailObj = new();
                            emailObj.strMessage = message;
                            var data = await _emailServiceManager.Updateproject(emailObj, model);
                            if (data.EmailTos.Count > 0)
                            {
                                var response = await _emailServiceManager.SendEmail(data);
                            }
                        }
                    }
                    return RedirectToAction("EditProjectInfo", new { id = model.ProjId, chk = "Copy", modelInfo = strModel });

                }
                else
                {
                    TempData["ErrorMessage"] = "Information for Project number: " + model.ProjNumber + " not updated";
                    return RedirectToAction("EditProjectInfo", new { id = model.ProjId, modelInfo = strModel });
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Information for Project number: " + model.ProjNumber + " not updated";
                return RedirectToAction("EditProjectInfo", new { id = model.ProjId, modelInfo = strModel });
            }
        }
        /// <summary>
        /// Autocomplete for Project type field
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public async Task<JsonResult> GetProjectType(string prefix)
        {
            HttpResponseDetail<dynamic> response = new();
            if (string.IsNullOrEmpty(prefix))
            {
                var result = (from member in _applicationDbContext.TblProjType
                              where member.IsActive == true
                              select new
                              {
                                  label = member.ProjType,
                                  val = member.ProjTypeId
                              }).ToList();
                return Json(result);
            }
            else
            {
                prefix = !string.IsNullOrEmpty(prefix) || !string.IsNullOrWhiteSpace(prefix) ? prefix.ToLower() : prefix;
                var result = await _applicationDbContext.TblProjType.Where(x => x.ProjType.ToLower().Contains(prefix) && x.IsActive == true).Select(x => new
                {
                    label = x.ProjType,
                    val = x.ProjTypeId
                }).ToListAsync();
                result = result.Where(x => x.label.ToLower().StartsWith(prefix)).ToList();
                return Json(result);
            }
        }
        /// <summary>
        /// Autocomplete for Entity Type
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public async Task<JsonResult> GetEntityType(string prefix)
        {
            var result = await _applicationDbContext.TblEntityType.Where(x => x.IsActive == true).Select(x => new
            {
                label = x.EntityType,
                val = x.EntityID
            }).OrderBy(x => x.label).ToListAsync();
            prefix = !string.IsNullOrEmpty(prefix) || !string.IsNullOrWhiteSpace(prefix) ? prefix.ToLower() : prefix;
            if (!string.IsNullOrEmpty(prefix) || !string.IsNullOrWhiteSpace(prefix))
            {
                result = await _applicationDbContext.TblEntityType.Where(x => x.EntityType.ToLower().Contains(prefix) && x.IsActive == true).Select(x => new
                {
                    label = x.EntityType,
                    val = x.EntityID
                }).ToListAsync();
                result = result.Where(x => x.label.ToLower().StartsWith(prefix)).ToList();
            }

            //var result = (from member in _applicationDbContext.TblEntityType
            //              where (!string.IsNullOrEmpty(member.EntityType))
            //              && (member.EntityType.StartsWith(prefix) && member.EntityType.Contains(prefix)) && member.IsActive==true
            //              select new
            //              {
            //                  label = member.EntityType,
            //                  val = member.EntityID
            //              }).Distinct().OrderBy(t => t.label)
            //.ToList();
            return Json(result);
        }
        /// <summary>
        /// Auto complete for PHL Company
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public async Task<JsonResult> GetCompanyName(string prefix)
        {
            prefix = !string.IsNullOrEmpty(prefix) || !string.IsNullOrWhiteSpace(prefix) ? prefix.ToLower() : prefix;
            var result = await _applicationDbContext.TblContractors.Where(x => x.Name.ToLower().Contains(prefix)).Select(x => new
            {
                label = x.Name,
                val = x.Id + ":" + x.Uid
            }).ToListAsync();
            result = result.Where(x => x.label.ToLower().StartsWith(prefix)).Take(20).ToList();
            return Json(result);
        }
        /// <summary>
        /// Autocomplete for Project subtype field
        /// </summary>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public async Task<JsonResult> GetProjectSubType(string prefix, int ProjTypeId)
        {
            HttpResponseDetail<dynamic> response = new();
            if (string.IsNullOrEmpty(prefix))
            {
                var result = await _applicationDbContext.TblProjSubType.Where(x => x.ProjTypeID == ProjTypeId).Select(x => new
                {
                    label = x.ProjSubType,
                    val = x.ProjSubTypeID
                }).ToListAsync();
                return Json(result);
            }
            else
            {
                prefix = !string.IsNullOrEmpty(prefix) || !string.IsNullOrWhiteSpace(prefix) ? prefix.ToLower() : prefix;
                var result = await _applicationDbContext.TblProjSubType.Where(x => x.ProjTypeID == ProjTypeId && x.ProjSubType.ToLower().Contains(prefix)).Select(x => new
                {
                    label = x.ProjSubType,
                    val = x.ProjSubTypeID
                }).ToListAsync();
                result = result.Where(x => x.label.ToLower().StartsWith(prefix)).ToList();
                return Json(result);
            }
        }
        /// <summary>
        /// No Use
        /// </summary>
        /// <param name="term"></param>
        /// <returns></returns>
        public JsonResult autofill(string term)
        {
            HttpResponseDetail<dynamic> response = new();
            var result = (from member in _applicationDbContext.TblProjType
                          where member.ProjType.StartsWith(term) && member.IsActive == true
                          select new
                          {
                              label = member.ProjType,
                              val = member.ProjTypeId
                          }).ToList();
            return Json(result);
        }
        /// <summary>
        /// No Use
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> ProjectInformation()
        {
            ProjectInformation model = new();
            List<string> objList = new List<string>();
            objList.Add("New Construction");
            objList.Add("Remodel");
            objList.Add("Addition");
            model.ProjScopeList = objList;
            model.AddendaS3 = new List<string>();
            List<SelectListItem> PHLType = await _projectRepository.GetPhlType();
            ViewBag.PHLType = PHLType;
            return View(model);
        }
        /// <summary>
        /// To get Project Number
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
        /// To open editproject info screen
        /// </summary>
        /// <param name="id"></param>
        /// <param name="search">To check the value of Pending project search of dashboard</param>
        /// <param name="ActiveTab">To check the value of Active tab</param>
        /// <param name="ActiveSearch">To check the value of Active project search of dashboard</param>
        /// <param name="PastSearch">To check the value of past project search of dashboard</param>
        /// <param name="modelInfo">For creating model when an error occures</param>
        /// <param name="chk">Used for checking whether its coming back from copy window</param>
        /// <returns></returns>
        [Authorize(Roles = "Staff")]
        public async Task<IActionResult> EditProjectInfo(int id, string search, string ActiveTab, string ActiveSearch, string PastSearch, string modelInfo, string chk)
        {
            if (id == 0)
            {
                ProjectInformation model = new();
                if (string.IsNullOrEmpty(modelInfo))
                {
                    model = new();
                }
                else
                {
                    model = JsonConvert.DeserializeObject<ProjectInformation>(modelInfo);
                    model.ProjNumber = null;
                    if (model.CallBack)
                    {
                        TempData["ErrorMessage"] = "Project is not saved. Please try again later";
                        ViewBag.ErrorMessage = TempData["ErrorMessage"] != null ? TempData["ErrorMessage"].ToString() : null;
                    }
                }
                List<string> objList = new List<string>();
                objList.Add("New Construction");
                objList.Add("Remodel");
                objList.Add("Addition");
                model.ProjScopeList = objList;
                model.AddendaS3 = new List<string>();
                List<SelectListItem> PHLType = await _projectRepository.GetPhlType();
                List<SelectListItem> BidOption = await _projectRepository.GetBidOption();
                BidOption.Insert(0, new SelectListItem { Value = "0", Text = "--No Selection--" });
                ViewBag.BidOption = BidOption;
                ViewBag.PHLType = PHLType;
                return View(model);
            }
            else
            {
                if (!string.IsNullOrEmpty(search))
                {
                    Response.Cookies.Append("dtSearchText", search, new Microsoft.AspNetCore.Http.CookieOptions
                    {
                        HttpOnly = false,
                        Secure = Convert.ToBoolean(false)
                    });
                }
                if (!string.IsNullOrEmpty(ActiveTab))
                {
                    Response.Cookies.Append("dtActiveText", ActiveTab, new Microsoft.AspNetCore.Http.CookieOptions
                    {
                        HttpOnly = false,
                        Secure = Convert.ToBoolean(false)
                    });
                }
                if (!string.IsNullOrEmpty(ActiveSearch))
                {
                    Response.Cookies.Append("dtActiveSearchText", ActiveSearch, new Microsoft.AspNetCore.Http.CookieOptions
                    {
                        HttpOnly = false,
                        Secure = Convert.ToBoolean(false)
                    });
                }
                if (!string.IsNullOrEmpty(PastSearch))
                {
                    Response.Cookies.Append("dtPastSearchText", PastSearch, new Microsoft.AspNetCore.Http.CookieOptions
                    {
                        HttpOnly = false,
                        Secure = Convert.ToBoolean(false)
                    });
                }
                IEnumerable<TblCounty> lstCounty = await _globalRepository.GetCounties();
                ProjectInformation model = new();
                if (string.IsNullOrEmpty(modelInfo) || id > 0)
                {
                    model = await _projectRepository.GetProjectdetail(id, lstCounty);
                }
                else
                {
                    model = JsonConvert.DeserializeObject<ProjectInformation>(modelInfo);
                }
                List<string> objList = new List<string>();
                objList.Add("New Construction");
                objList.Add("Remodel");
                objList.Add("Addition");
                model.ProjScopeList = objList;
                if (TempData.ContainsKey("ErrorMessage") && TempData["ErrorMessage"] != null)
                {
                    ViewBag.ErrorMessage = TempData["ErrorMessage"].ToString();
                }

                if (TempData.ContainsKey("SuccessMessage") && TempData["SuccessMessage"] != null)
                {
                    ViewBag.SuccessMessage = TempData["SuccessMessage"].ToString();
                }
                if (chk == "Copy")
                {
                    model.ProjId = 0;
                    model.ProjNumber = "";
                }
                if (!string.IsNullOrEmpty(model.ProjNumber) && model.ProjNumber != "0")
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
                }
                List<SelectListItem> PHLType = await _projectRepository.GetPhlType();
                ViewBag.PHLType = PHLType;
                List<SelectListItem> BidOption = await _projectRepository.GetBidOption();
                BidOption.Insert(0, new SelectListItem { Value = "0", Text = "--No Selection--" });
                ViewBag.BidOption = BidOption;
                if (model != null)
                {
                    HttpContext.Session.SetString("ProjectInfo", JsonConvert.SerializeObject(model));
                }
                return View(model);
            }

        }
        /// <summary>
        /// Download copycenter folder zip from project preview page
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task<FileResult> DownLoadZip(string filePath, string file)
        {
            string zipFile = file + ".zip";
            MemoryStream stream = await DownloadS3FolderAsZip(filePath);
            return File(stream, "application/zip", zipFile);
        }
        /// <summary>
        /// Download All copycenter folder zip from project preview page
        /// </summary>
        /// <param name="Zip"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public async Task<FileResult> DownLoadAllZip(string zip, string type)
        {
            // Define paths
            string pathRoot = Path.Combine(this.Environment.WebRootPath, "Preview");
            string pathPhoto = CreatePath(pathRoot);
            Directory.CreateDirectory(pathPhoto);

            // Create ZIP file name
            string zipFile = string.IsNullOrEmpty(type) ? $"{zip}.zip" : $"{zip}_{type}.zip";
            string source =(!string.IsNullOrEmpty(type))? Path.Combine(_fileUploadPath, "20" + zip.Substring(0, 2), zip.Substring(2, 2), zip,type): Path.Combine(_fileUploadPath, "20" + zip.Substring(0, 2), zip.Substring(2, 2), zip);

            // Download local files into a ZIP byte array
            byte[] zipBytes = await DownloadDirectoryAsync(pathPhoto, pathRoot, zip, type,source);

            // Delete temporary directory
            Directory.Delete(pathPhoto, true);

            // Return the ZIP file as a FileResult
            return File(zipBytes, "application/zip", zipFile);
        }


        /// <summary>
        /// Download pdf file from project preview page
        /// </summary>
        /// <param name="Zip"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public ActionResult DownloadFile(string file, string filePath)
        {
            try
            {
                WebClient myWebClient = new WebClient();
                byte[] myDataBuffer = myWebClient.DownloadData(filePath);

                // Set the response headers to suggest a download
                Response.ContentType = "application/octet-stream";
                Response.Headers.Add("Content-Disposition", $"attachment; filename=\"{file}\"");

                // Write the file to the response stream
                return File(myDataBuffer, "application/octet-stream");
            }
            catch (Exception ex)
            {
                // Handle exceptions appropriately
                // For example, you can return an error message or redirect to an error page.
                return Content("Error downloading file: " + ex.Message);
            }


        }

        //public FileResult DownLoadFile(string file, string filePath)
        //{
        //    //Build the File Path.
        //    string path = filePath;
        //    WebClient myWebClient = new WebClient();
        //    //Read the File data into Byte Array.
        //    byte[] myDataBuffer = myWebClient.DownloadData(filePath);
        //    //byte[] bytes = System.IO.File.ReadAllBytes(path);
        //    //myWebClient.DownloadFile("https://contractor-aws.s3.amazonaws.com/2023/01/23010008/Plans/103120000000007373.pdf", pathPhoto);

        //    //Send the File to Download.
        //    return File(myDataBuffer, "application/octet-stream", file);
        //}
        /// <summary>
        /// No Use
        /// </summary>
        /// <param name="uniqueDate"></param>
        /// <param name="projectNumber"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> UniqueDate(string uniqueDate, string projectNumber)
        {
            HttpResponseDetail<dynamic> response = new();
            var tbl = await _projectRepository.UniqueDate(uniqueDate, projectNumber);
            if (tbl == null)
            {
                response.success = true;
                response.statusMessage = "Date already exists";
            }
            else
            {

            }
            return Json(response);
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <param name="ProjId"></param>
        /// <param name="PHLNote"></param>
        /// <returns></returns>
        public async Task<JsonResult> AddPhlNote(int ProjId, string PHLNote)
        {
            HttpResponseDetail<dynamic> response = await _projectRepository.AddPhlNote(ProjId, PHLNote);

            return Json(response);
        }
        /// <summary>
        /// No Use
        /// </summary>
        /// <param name="ProjId"></param>
        /// <param name="BrNote"></param>
        /// <returns></returns>
        public async Task<JsonResult> AddBrNote(int ProjId, string BrNote)
        {
            HttpResponseDetail<dynamic> response = await _projectRepository.AddBrNote(ProjId, BrNote);

            return Json(response);
        }
        /// <summary>
        /// Show card of phl details in project preview page
        /// </summary>
        /// <param name="Id"></param>
        /// <param name="ConId"></param>
        /// <returns></returns>
        public async Task<JsonResult> ShowCard(int Id, int ConId)
        {
            HttpResponseDetail<dynamic> response = await _projectRepository.ShowCard(Id, ConId);
            return Json(response);
        }
        /// <summary>
        /// Check county when auto populate value from zip
        /// </summary>
        /// <param name="City"></param>
        /// <param name="State"></param>
        /// <returns></returns>
        public async Task<JsonResult> CheckCounty(string City, string State)
        {
            return Json(await _projectRepository.CheckCountyAsync(City, State));
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetProjectNumber()
        {
            return Json(await _projectRepository.GetProjectNumberAsync());
        }
        /// <summary>
        /// Get PhlType
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetPHLType()
        {
            return Json(await _projectRepository.GetPhlType());
        }
        /// <summary>
        /// Getting counties for multiple county popup
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetCounties()
        {
            IEnumerable<TblCounty> model = await _globalRepository.GetCounties();
            List<TblCounty> WAValues = model.Where(x => x.State == "WA").Select(x => new TblCounty
            {
                County = x.County,
                State = x.State,
                CountyId = x.CountyId
            }).OrderBy(x => x.County).ToList();
            List<TblCounty> ORValues = model.Where(x => x.State == "OR").Select(x => new TblCounty
            {
                County = x.County,
                State = x.State,
                CountyId = x.CountyId
            }).OrderBy(x => x.County).ToList();
            List<TblCounty> OtherVal = model.Where(x => x.State != "OR" && x.State != "WA").Select(x => new TblCounty
            {
                County = x.County,
                State = x.State,
                CountyId = x.CountyId
            }).OrderBy(x => x.County).ToList();
            return Json(new { Status = "success", ORData = ORValues, WAData = WAValues, OData = OtherVal });
        }
        public IActionResult PreviewPdf(string filePath)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                if (System.IO.File.Exists(filePath))
                {
                    byte[] fileBytes = System.IO.File.ReadAllBytes(filePath);
                    return File(fileBytes, "application/pdf");
                }
            }

            return NotFound();
        }
        public IActionResult DocPreview(string filePath ,int? pageNo)
        {
            if (!string.IsNullOrEmpty(filePath))
            {
                if (System.IO.File.Exists(filePath))
                {
                    string pageParam = pageNo.HasValue ? $"#page={pageNo.Value}" : "";

                    ViewBag.PdfUrl = Url.Action("PreviewPdf", "Project", new { filePath = filePath }) + pageParam;

                    return View();
                }
            }

            return NotFound();
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> Preview(int id)
        {
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
                string rootFolder = model.ProjNumber.Substring(0, 2);
                rootFolder = rootFolder.Replace(rootFolder, "20" + rootFolder);
                string subRootFolder = model.ProjNumber.Substring(2, 2);
                List<AddendaFileInfo> Addenda = await GetListOfS3ContentAddenda(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/", "Addenda/");
                List<AddendaFileInfo> AddendaS3List = new();
                foreach (AddendaFileInfo add in Addenda)
                {
                    AddendaFileInfo fi = new();
                    string[] AdFile = add.PathInfo.Split(@"/");
                    if (AdFile.Length == 6)
                    {
                        if (string.IsNullOrEmpty(AdFile[5]))
                        {
                            if (add.Size > 0)
                                fi.IsFile = true;
                            else
                                fi.IsFile = false;
                            fi.FileInfo = AdFile[4];
                            fi.PathInfo = add.PathInfo;
                            AddendaS3List.Add(fi);
                        }
                    }
                    if (AdFile.Length == 5)
                    {
                        if (add.Size > 0)
                            fi.IsFile = true;
                        else
                            fi.IsFile = false;
                        fi.FileInfo = AdFile[4];
                        fi.PathInfo = add.PathInfo;
                        AddendaS3List.Add(fi);
                    }
                    //string toReplace = rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/" + "Addenda/";
                    ////string FolderPath = add.Substring(0, add.LastIndexOf('/'));
                    //string replaced = add.Replace(toReplace, "");
                    //int chkIndex = replaced.IndexOf("/");
                    ////int length = replaced.Length -chkIndex;
                    //// toReplace = replaced.Substring(chkIndex, length);
                    ////replaced = replaced.Replace(toReplace, "");
                    //if (chkIndex > -1)
                    //{
                    //    if (replaced.Length - replaced.IndexOf("/") > 1)
                    //    {
                    //        replaced = replaced.Substring(replaced.IndexOf("/"), (replaced.Length - replaced.IndexOf("/")));
                    //        replaced = replaced.Replace("/", "");
                    //    }
                    //    else
                    //    {
                    //        replaced = String.Empty;
                    //    }
                    //}
                    ////replaced = replaced.Substring(0, (replaced.IndexOf("/")-1));
                    ////replaced = replaced.Replace("/", "");
                    //if (!string.IsNullOrEmpty(replaced))
                    ////AddendaS3List.Add(replaced);
                    //{
                    //    AddendaFileInfo fi = new();
                    //    fi.FileInfo = replaced;
                    //    fi.PathInfo = add;
                    //    AddendaS3List.Add(fi);
                    //}
                }
                model.AddendaS3Files = AddendaS3List;
                List<string> PHL = await GetListOfS3Content(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/", "PHL/");
                List<PHLInfo> PHLS3List = new();
                foreach (string add in PHL)
                {
                    string toReplace = rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/" + "PHL/";
                    //string FolderPath = add.Substring(0, add.LastIndexOf('/'));
                    string replaced = add.Replace(toReplace, "");
                    int chkIndex = replaced.IndexOf("/");
                    //int length = replaced.Length -chkIndex;
                    // toReplace = replaced.Substring(chkIndex, length);
                    //replaced = replaced.Replace(toReplace, "");
                    if (chkIndex > -1)
                    {
                        if (replaced.Length - replaced.IndexOf("/") > 1)
                        {
                            replaced = replaced.Substring(replaced.IndexOf("/"), (replaced.Length - replaced.IndexOf("/")));
                            replaced = replaced.Replace("/", "");
                        }
                        else
                        {
                            replaced = String.Empty;
                        }
                    }
                    //replaced = replaced.Substring(0, (replaced.IndexOf("/")-1));
                    //replaced = replaced.Replace("/", "");
                    if (!string.IsNullOrEmpty(replaced))
                    //PHLS3List.Add(replaced);
                    {
                        PHLInfo fi = new();
                        fi.FileInfo = replaced;
                        fi.PathInfo = add;
                        PHLS3List.Add(fi);
                    }
                }
                model.PHLFiles = PHLS3List;
                List<string> Plans = await GetListOfS3Content(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/", "Plans/");
                List<PlansInfo> PlansS3List = new();
                foreach (string add in Plans)
                {
                    string toReplace = rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/" + "Plans/";
                    //string FolderPath = add.Substring(0, add.LastIndexOf('/'));
                    string replaced = add.Replace(toReplace, "");
                    int chkIndex = replaced.IndexOf("/");
                    //int length = replaced.Length -chkIndex;
                    // toReplace = replaced.Substring(chkIndex, length);
                    //replaced = replaced.Replace(toReplace, "");
                    if (chkIndex > -1)
                    {
                        if (replaced.Length - replaced.IndexOf("/") > 1)
                        {
                            replaced = replaced.Substring(replaced.IndexOf("/"), (replaced.Length - replaced.IndexOf("/")));
                            replaced = replaced.Replace("/", "");
                        }
                        else
                        {
                            replaced = String.Empty;
                        }
                    }
                    //replaced = replaced.Substring(0, (replaced.IndexOf("/")-1));
                    //replaced = replaced.Replace("/", "");
                    if (!string.IsNullOrEmpty(replaced))
                    //PlansS3List.Add(replaced);
                    {
                        PlansInfo fi = new();
                        fi.FileInfo = replaced;
                        fi.PathInfo = add;
                        PlansS3List.Add(fi);
                    }
                }
                model.PlansFiles = PlansS3List;
                List<string> Specs = await GetListOfS3Content(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/", "Specs/");
                List<SpecsInfo> SpecsS3List = new();
                foreach (string add in Specs)
                {
                    string toReplace = rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/" + "Specs/";
                    //string FolderPath = add.Substring(0, add.LastIndexOf('/'));
                    string replaced = add.Replace(toReplace, "");
                    int chkIndex = replaced.IndexOf("/");
                    //int length = replaced.Length -chkIndex;
                    // toReplace = replaced.Substring(chkIndex, length);
                    //replaced = replaced.Replace(toReplace, "");
                    if (chkIndex > -1)
                    {
                        if (replaced.Length - replaced.IndexOf("/") > 1)
                        {
                            replaced = replaced.Substring(replaced.IndexOf("/"), (replaced.Length - replaced.IndexOf("/")));
                            replaced = replaced.Replace("/", "");
                        }
                        else
                        {
                            replaced = String.Empty;
                        }
                    }
                    //replaced = replaced.Substring(0, (replaced.IndexOf("/")-1));
                    //replaced = replaced.Replace("/", "");
                    if (!string.IsNullOrEmpty(replaced))
                    //SpecsS3List.Add(replaced);
                    {
                        SpecsInfo fi = new();
                        fi.FileInfo = replaced;
                        fi.PathInfo = add;
                        SpecsS3List.Add(fi);
                    }
                }
                model.SpecsFiles = SpecsS3List;
                List<string> BidResults = await GetListOfS3Content(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/", "Bid Results/");
                List<BidResultsInfo> BidResultsS3List = new();
                foreach (string add in BidResults)
                {
                    string toReplace = rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/" + "Bid Results/";
                    string replaced = add.Replace(toReplace, "");
                    int chkIndex = replaced.IndexOf("/");
                    //int length = replaced.Length -chkIndex;
                    // toReplace = replaced.Substring(chkIndex, length);
                    //replaced = replaced.Replace(toReplace, "");
                    if (chkIndex > -1)
                    {
                        if (replaced.Length - replaced.IndexOf("/") > 1)
                        {
                            replaced = replaced.Substring(replaced.IndexOf("/"), (replaced.Length - replaced.IndexOf("/")));
                            replaced = replaced.Replace("/", "");
                        }
                        else
                        {
                            replaced = String.Empty;
                        }
                    }
                    //replaced = replaced.Substring(0, (replaced.IndexOf("/")-1));
                    //replaced = replaced.Replace("/", "");
                    if (!string.IsNullOrEmpty(replaced))
                    //SpecsS3List.Add(replaced);
                    {
                        BidResultsInfo fi = new();
                        fi.FileInfo = replaced;
                        fi.PathInfo = add;
                        BidResultsS3List.Add(fi);
                    }

                }
                model.BidResultsFiles = BidResultsS3List;


            }
            return View(model);
        }
        /// <summary>
        /// Ordering single pdf from project preview (No use)
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
        /// Ordering all copy for particular section
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> OrderCopiesAll(List<PHLInfo> model)
        {
            string pathPhoto = Path.Combine(this.Environment.WebRootPath, "Storage");
            pathPhoto = CreatePath(pathPhoto);
            Directory.CreateDirectory(pathPhoto);
            string pathTemp = pathPhoto.Substring(0, pathPhoto.LastIndexOf("\\") + 1);
            pathTemp = pathPhoto.Replace(pathTemp, "");
            List<PrevOrder> prevOrders = new();
            foreach (var item in model)
            {
                item.FileInfo = item.FileInfo.Contains(" ") ? item.FileInfo.Replace(" ", "_") : item.FileInfo;
                string pathPhotoTemp = Path.Combine(pathPhoto, item.FileInfo);
                var tuple = await GetUniqueFileName(pathPhotoTemp, item.FileInfo, pathPhoto);
                pathPhotoTemp = tuple.Item1;
                item.FileInfo = tuple.Item2;
                WebClient myWebClient = new WebClient();
                //byte[] myDataBuffer = myWebClient.DownloadData(filepath);
                myWebClient.DownloadFile(item.PathInfo, pathPhotoTemp);
                PdfReader pdfReader = new PdfReader(pathPhotoTemp);
                int numberOfPages = pdfReader.NumberOfPages;
                prevOrders.Add(new PrevOrder { NoP = numberOfPages, FileName = item.FileInfo });
            }
            //return Json(new { Status = "success", Message = message });
            return Json(new { Status = "success", DirData = prevOrders, data = pathTemp });
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
        public async Task<JsonResult> GetCopyCenterPriceDetail()
        {
            HttpResponseDetail<dynamic> response = new();
            response = await _commonRepository.GetCopyCenterPriceDetail();
            return Json(response);
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <param name="State"></param>
        /// <returns></returns>
        public async Task<JsonResult> GetExCounties(string State)
        {

            List<string> response = new();
            response = await _projectRepository.GetExCounties(State);
            return Json(new { response = response });
        }
        /// <summary>
        /// For adding new new member from entity name field
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> RegNonMember(MemberShipRegistration model)
        {
            HttpResponseDetail<dynamic> response = new();
            response = await _projectRepository.RegNonMember(model);
            return Json(response);
        }
        /// <summary>
        /// Check for unique mail when adding entity name from edit project info
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
        /// Get Contact for COntractor under phl info
        /// </summary>
        /// <param name="id"></param>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public async Task<JsonResult> GetContactName(int id, string prefix)
        {
            if (string.IsNullOrEmpty(prefix))
            {
                var result = (from contact in _applicationDbContext.TblContacts
                              where contact.Id == id && contact.CompType == 2
                              select new
                              {
                                  label = contact.Contact,
                                  val = contact.ConId
                              }).Take(15).ToList();
                return Json(result);
            }
            else
            {
                prefix = !string.IsNullOrEmpty(prefix) || !string.IsNullOrWhiteSpace(prefix) ? prefix.ToLower() : prefix;
                var result = await _applicationDbContext.TblContacts.Where(x => x.Id == id && x.CompType == 2 && x.Contact.ToLower().Contains(prefix)).Select(x => new
                {
                    label = x.Contact,
                    val = x.ConId
                }).ToListAsync();
                result = result.Where(x => x.label.ToLower().StartsWith(prefix)).ToList();
                return Json(result);
            }
        }
        /// <summary>
        /// Check for uniqueemail when adding an contractor editprojectInfo in phlinfo section
        /// </summary>
        /// <param name="uniqueName">email</param>
        /// <param name="CompType">2</param>
        /// <returns></returns> 
        public async Task<JsonResult> ConUniqueEmail(string uniqueName, int CompType)
        {
            HttpResponseDetail<dynamic> response = new();
            if (!string.IsNullOrEmpty(uniqueName))
            {
                if (CompType == 2)
                {
                    var tblmember = await _dbContext.TblContractors.Where(m => m.Email == uniqueName).FirstOrDefaultAsync();
                    //.Where(m => (m.Email1 == uniqueName || m.Email2 ==uniqueName ) && m.compType==2).FirstOrDefaultAsync();
                    if (tblmember == null)
                    {
                        var tblContact = await _dbContext.TblContacts.Where(m => m.Email == uniqueName && m.CompType == CompType).FirstOrDefaultAsync();
                        if (tblContact == null)
                        {
                        }
                        else
                        {
                            response.success = true;
                            response.statusMessage = "Email already exists";
                        }
                    }
                    else
                    {
                        response.success = true;
                        response.statusMessage = "Email already exists";
                    }
                }
                else if (CompType == 3)
                {
                    var tblmember = await _dbContext.TblArchOwners.Where(m => m.Email == uniqueName).FirstOrDefaultAsync();
                    //.Where(m => (m.Email1 == uniqueName || m.Email2 ==uniqueName ) && m.compType==2).FirstOrDefaultAsync();
                    if (tblmember == null)
                    {
                        var tblContact = await _dbContext.TblContacts.Where(m => m.Email == uniqueName && m.CompType == CompType).FirstOrDefaultAsync();
                        if (tblContact == null)
                        {
                        }
                        else
                        {
                            response.success = true;
                            response.statusMessage = "Email already exists";
                        }
                    }
                    else
                    {
                        response.success = true;
                        response.statusMessage = "Email already exists";
                    }
                }
            }
            return Json(response);
        }
        /// <summary>
        /// Saving new archict
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> SaveNewContact(MemberShipRegistration model)
        {
            HttpResponseDetail<dynamic> response = new();
            response = await _projectRepository.SaveNewContact(model);
            return Json(response);
        }
        public async Task<JsonResult> GetEntityName(string prefix)
        {
            prefix = !string.IsNullOrEmpty(prefix) || !string.IsNullOrWhiteSpace(prefix) ? prefix.ToLower() : prefix;
            var result = _entityRepository.GetEntities().Where(x => x.Company.ToLower().Contains(prefix) && x.Inactive == false).Take(100).Select(x => new
            {
                label = x.Company,
                val = x.Id,
                CompType = ((bool)x.IsMember) ? 1 : ((bool)(x.IsArchitect) ? 3 : (bool)(x.IsContractor) ? 2 : 0)
            }).OrderBy(x => x.label).ToList();
            //var ArchResult = await _applicationDbContext.TblArchOwners.Where(x => x.Name.ToLower().Contains(prefix)).Take(100).Select(x => new
            //{
            //    label = x.Name,
            //    val = x.Id,
            //    CompType = 3
            //}).OrderBy(x => x.label).ToListAsync();
            //result.AddRange(ArchResult);
            //var ContResult = await _applicationDbContext.TblContractors.Where(x => x.Name.ToLower().Contains(prefix)).Take(100).Select(x => new
            //{
            //    label = x.Name,
            //    val = x.Id,
            //    CompType = 2
            //}).OrderBy(x => x.label).ToListAsync();
            //result.AddRange(ContResult);
            result = result.DistinctBy(x => x.label).ToList();
            result = result.Where(x => x.label.ToLower().StartsWith(prefix)).OrderBy(x => x.label).ToList();
            return Json(result);
        }
        /// <summary>
        /// Save Entity Type from editprojectinfo page
        /// </summary>
        /// <param name="EntityType"></param>
        /// <returns></returns>
        public async Task<JsonResult> SaveEntType(string EntityType)
        {
            HttpResponseDetail<dynamic> response = new();
            response = await _projectRepository.SaveEntityTypeAsync(EntityType);
            return Json(response);
        }
        /// <summary>
        /// Register contact for contractor from editprojectinfo popup
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> RegPhlCon(MemberShipRegistration model)
        {
            HttpResponseDetail<dynamic> response = new();
            response = await _projectRepository.RegPhlCon(model);
            return Json(response);
        }
        /// <summary>
        /// Get PhlType
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetBidOption()
        {
            return Json(await _projectRepository.GetBidOption());
        }
        /// <summary>
        /// Get Contact Detail From ConId
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetContactDetail(int ConId)
        {
            string PhoneNum = "";
            string Email = "";
            if (ConId != 0)
            {
                TblContact contact = await _dbContext.TblContacts.FirstOrDefaultAsync(x => x.ConId == ConId);
                Email = string.IsNullOrEmpty(contact.Email) ? "" : contact.Email;
                PhoneNum = string.IsNullOrEmpty(contact.Phone) ? "" : contact.Phone;
            }
            return Json(new { Status = "success", Email = Email, PhoneNum = PhoneNum, Flag = 'Y' });
        }
    }
}
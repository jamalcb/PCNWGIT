using Amazon;
using Amazon.S3;
using Amazon.S3.Model;
using Amazon.S3.Transfer;
using DocumentFormat.OpenXml.Packaging;
using iTextSharp.text.pdf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
using SolrNet;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using UglyToad.PdfPig;

namespace PCNW.Controllers
{
    [DisableRequestSizeLimit]
    [Authorize(Roles = "Member")]
    public partial class MemberController : BaseController
    {
        private readonly IMembershipRepository _membershipRepository;
        private readonly IStaffRepository _staffRepository;
        private readonly ILogger<MemberController> _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _dbContext;
        private readonly IEmailServiceManager _emailServiceManager;
        private readonly ICommonRepository _commonRepository;
        private readonly IProjectRepository _projectRepository;
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IImplementRepository _implementRepository;
        private readonly IConfiguration _configuration;
        private readonly IEntityRepository _entityRepository;
        private readonly string _fileUploadPath;
        private readonly ISolrOperations<SolrDocument> _solr;

        public MemberController(ISolrOperations<SolrDocument> solr, IMembershipRepository membershipRepository, IWebHostEnvironment _Environment, ILogger<MemberController> logger, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ApplicationDbContext dbContext, IEmailServiceManager emailServiceManager, ICommonRepository commonRepository, IProjectRepository projectRepository, IStaffRepository IStaffRepository, RoleManager<IdentityRole> roleManager, IConfiguration configuration, IImplementRepository implementRepository, IEntityRepository entityRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
            _emailServiceManager = emailServiceManager;
            _membershipRepository = membershipRepository;
            _logger = logger;
            _commonRepository = commonRepository;
            _projectRepository = projectRepository;
            _webHostEnvironment = _Environment;
            _staffRepository = IStaffRepository;
            _roleManager = roleManager;
            _configuration = configuration;
            _implementRepository = implementRepository;
            _entityRepository = entityRepository;
            _fileUploadPath = _configuration.GetSection("AppSettings")["FileUploadPath"];
            _solr = solr;
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
        /// For going to member's dashboard screen/page.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            string usern = User.Identity.Name;
            DisplayLoginInfo info = _commonRepository.GetUserInfo(User.Identity.Name);
            if (info.InActive == true)
            {
                TempData["InActive"] = "Yes";
                return View("AccessDenied");
            }
            MemberDashboard model = await _membershipRepository.GetMemberDashboardProjects(info)
;
            return View(model);
        }

        /// <summary>
        /// For going to preview screen/page.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="search"></param>
        /// <param name="futuresearch"></param>
        /// <param name="previoussearch"></param>
        /// <param name="LoadValChk"></param>
        /// <returns></returns>
        public async Task<IActionResult> Preview(int id, string search, string futuresearch, string previoussearch, string LoadValChk = "NoValue",string solr=null)
        {
            DisplayLoginInfo info = _commonRepository.GetUserInfo(User.Identity.Name);
            if (info.InActive == true)
            {
                TempData["InActive"] = "Yes";
                return View("AccessDenied");
            }

            if (!string.IsNullOrEmpty(search))
            {
                Response.Cookies.Append("dtActiveSearchText", search, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    HttpOnly = false,
                    Secure = Convert.ToBoolean(false)
                });
            }
            if (!string.IsNullOrEmpty(futuresearch))
            {
                Response.Cookies.Append("dtFutureSearchText", futuresearch, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    HttpOnly = false,
                    Secure = Convert.ToBoolean(false)
                });
            }
            if (!string.IsNullOrEmpty(previoussearch))
            {
                Response.Cookies.Append("dtPreviousSearchText", previoussearch, new Microsoft.AspNetCore.Http.CookieOptions
                {
                    HttpOnly = false,
                    Secure = Convert.ToBoolean(false)
                });
            }
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
                        var fi = new PlansInfo()
                        {
                            FileInfo = Path.GetFileName(file),
                            PathInfo = file
                        };
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
                        var fi = new SpecsInfo
                        {
                            FileInfo = Path.GetFileName(file),
                            PathInfo = file
                        };
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
                        var fi = new BidResultsInfo
                        {
                            FileInfo = Path.GetFileName(file),
                            PathInfo = file
                        };
                        BidResultsS3List.Add(fi);
                    }
                }
                model.BidResultsFiles = BidResultsS3List;
            }

            if (!string.IsNullOrEmpty(solr))
            {
                ViewBag.Query = solr;
                var solrQuery = CreateSolrQuery(solr);

                if (solrQuery != null)
                {
                    ///var results = _solr.Query(new SolrQuery($"content:{solr}"));

                    var results = _solr.Query(solrQuery);
                    var projIdList = results.Where(doc => doc.Content != null && int.TryParse(doc.projectId.FirstOrDefault(), out _))
                        .Select(doc => int.Parse(doc.projectId.FirstOrDefault()))
                        .Distinct().ToList();

                    if (projIdList.Contains(id) && model.ProjNumber != null)
                    {
                        var filteredList = results
                            .Where(doc => int.TryParse(doc.projectId.FirstOrDefault(), out int parsedId) &&
                            parsedId == id)
                            .ToList();

                        var filteredResults = new SolrQueryResults<SolrDocument>();
                        filteredResults.AddRange(filteredList);
                        results = filteredResults;
                        var formattedResults = new Dictionary<string, Dictionary<int, List<SearchResultViewModel>>>();

                        string year = string.Concat("20", model.ProjNumber.AsSpan(0, 2));
                        string month = model.ProjNumber.Substring(2, 2);
                        string projNumber = model.ProjNumber;

                        string basePath = _fileUploadPath;

                        foreach (var doc in results)
                        {
                            if (doc.Content != null)
                            {
                                var sourceType = doc.SourceType?.FirstOrDefault();
                                string folderType = sourceType;

                                // Set the correct file path based on folder type
                                string projectPath = Path.Combine(basePath, year, month, projNumber, folderType);
                                if (Directory.Exists(projectPath)) 
                                    {
                                        var filePath = Path.Combine(projectPath, doc.Filename.FirstOrDefault());
                                        var fileExtension = Path.GetExtension(filePath).ToLower();
                                        var pageContents = new Dictionary<int, List<string>>();

                                        switch (fileExtension)
                                        {
                                            case ".pdf":
                                                using (var pdf = UglyToad.PdfPig.PdfDocument.Open(filePath))
                                                {
                                                    int pageIndex = 1;
                                                    foreach (var page in pdf.GetPages())
                                                    {
                                                        var text = page.Text;
                                                        if (!string.IsNullOrWhiteSpace(text))
                                                        {
                                                            if (!pageContents.ContainsKey(pageIndex))
                                                            {
                                                                pageContents[pageIndex] = new List<string>();
                                                            }
                                                            pageContents[pageIndex].Add(text);
                                                        }
                                                        pageIndex++;
                                                    }
                                                }
                                                break;

                                            case ".docx":
                                                using (var wordDocument = WordprocessingDocument.Open(filePath, false))
                                                {
                                                    var text = ExtractTextFromDocx(wordDocument);
                                                    if (!string.IsNullOrWhiteSpace(text))
                                                    {
                                                        // Word documents typically don't have pages like PDFs, so we treat the entire content as one page
                                                        pageContents[1] = new List<string> { text };
                                                    }
                                                }
                                                break;

                                            case ".txt":
                                                var textContent = await System.IO.File.ReadAllTextAsync(filePath);
                                                if (!string.IsNullOrWhiteSpace(textContent))
                                                {
                                                    // Treat the entire text file as a single "page"
                                                    pageContents[1] = new List<string> { textContent };
                                                }
                                                break;

                                            default:
                                                _logger.LogWarning("Unsupported file type: {filePath}", filePath);
                                                continue;
                                        }

                                        var filename = doc.Filename.FirstOrDefault(); // Assuming Filename is a single string

                                        // Initialize the dictionary for the current filename if it does not exist
                                        if (!formattedResults.ContainsKey(filename))
                                        {
                                            formattedResults[filename] = new Dictionary<int, List<SearchResultViewModel>>();
                                        }

                                        foreach (var page in pageContents)
                                        {
                                            var pageNumber = page.Key;
                                            var contentLines = page.Value;

                                            if (!formattedResults[filename].ContainsKey(pageNumber))
                                            {
                                                formattedResults[filename][pageNumber] = new List<SearchResultViewModel>();
                                            }

                                            foreach (var line in contentLines)
                                            {
                                                solr = solr.Trim('*');
                                                // Split and process terms
                                                var pattern = "\"([^\"]*)\"|\\S+";
                                                var terms = Regex.Matches(solr, pattern)
                                                    .Cast<Match>()
                                                    .Select(m => m.Groups[1].Success ? $"\"{m.Groups[1].Value}\"" : m.Value.Trim())
                                                    .Where(t => !string.IsNullOrEmpty(t))
                                                    .ToList();

                                                var matches = Regex.Matches(solr, "\"([^\"]+)\"")
                                                    .Cast<Match>()
                                                    .Select(m => m.Groups[1].Success ? m.Groups[1].Value : m.Value.Trim())
                                                    .Where(t => !string.IsNullOrEmpty(t))
                                                    .ToList();

                                                var query= solr.Split(new[] { ',', '&' }, StringSplitOptions.RemoveEmptyEntries)
                                                    .Select(q => q.Trim())
                                                    .ToList();

                                                terms.AddRange(query);
                                                terms = terms.Distinct().ToList();

                                                bool containsAnyexactTerm = terms.Any(term => line.Contains(term, StringComparison.OrdinalIgnoreCase));
                                                bool containsAnyTerm = matches.Any(term => line.Contains(term, StringComparison.OrdinalIgnoreCase));

                                                if (containsAnyTerm || containsAnyexactTerm) 
                                                {
                                                    var sentences = ExtractSentences(line, terms);
                                                    foreach (var sentence in sentences)
                                                    {
                                                        if (!string.IsNullOrEmpty(sentence))
                                                        {
                                                            formattedResults[filename][pageNumber].Add(new SearchResultViewModel
                                                            {
                                                                Id = doc.Id,
                                                                Filename = filename,
                                                                Content = new List<string> { sentence },
                                                                PageNumber = pageNumber,
                                                                filepath = filePath
                                                            });
                                                        }
                                                    }
                                                }
                                            }


                                        }
                                    }
                                    else
                                    {
                                        _logger.LogWarning("Project path not found: {projectPath}", projectPath);
                                    }
                            }
                        }

                        model.SolrSearchResult = formattedResults;
                    }
                }
            }


            return View(model);
        }
        private SolrQuery CreateSolrQuery(string userInput)
        {
            if (string.IsNullOrEmpty(userInput))
            {
                return null;
            }

            string query = "";

            var terms = userInput.Split(new[] { ',', '&' }, StringSplitOptions.RemoveEmptyEntries)
                                 .Select(term => term.Trim())
                                 .Where(term => !string.IsNullOrEmpty(term))
                                 .ToList();

            bool isAndSearch = userInput.Contains("&");
            bool isOrSearch = userInput.Contains(",");

            List<string> queryParts = new List<string>();

            foreach (var term in terms)
            {
                if (term.StartsWith("\"") && term.EndsWith("\""))
                {
                    queryParts.Add($"content:{term}");
                }
                else if (term.StartsWith("*") || term.EndsWith("*"))
                {
                    queryParts.Add($"content:{term}");
                }
                else
                {
                    queryParts.Add($"content:*{term}*");
                }
            }

            if (isAndSearch && isOrSearch)
            {
                //throw new InvalidOperationException("Cannot mix commas (OR) and ampersands (AND) in the query.");
            }
            else if (isAndSearch)
            {
                query = string.Join(" AND ", queryParts);
            }
            else
            {
                query = string.Join(" OR ", queryParts);
            }

            return new SolrQuery(query);
        }


        public static string ExtractTextFromDocx(WordprocessingDocument wordDocument)
        {
            if (wordDocument.MainDocumentPart == null)
            {
                return string.Empty;
            }

            var document = wordDocument.MainDocumentPart.Document;
            if (document == null || document.Body == null)
            {
                return string.Empty;
            }

            return document.Body.InnerText;
        }
        private IEnumerable<string> ExtractSentences(string text, List<string> queries)
        {
            var result = new List<string>();
            var addedRanges = new List<(int start, int end)>(); 

            foreach (var query in queries)
            {
                var terms = query.Split(new[] { ',', '&' }, StringSplitOptions.RemoveEmptyEntries)
                                 .Select(q => q.Trim())
                                 .ToList();

                bool anyTermMatched = false;

                foreach (var term in terms)
                {
                    bool isExactMatch = term.StartsWith("\"") && term.EndsWith("\"");
                    string searchTerm = isExactMatch ? term.Trim('"') : term;

                    if (isExactMatch)
                    {
                        var pattern = $@"(?<=\s|^|[.,!?;:])\b{Regex.Escape(searchTerm)}\b(?=\s|$|[.,!?;:])"; 
                        int startIndex = 0;

                        while (true)
                        {
                            var match = Regex.Match(text.Substring(startIndex), pattern, RegexOptions.IgnoreCase);

                            if (!match.Success)
                            {
                                break;
                            }

                            int matchStartIndex = startIndex + match.Index;
                            int matchEndIndex = matchStartIndex + match.Length;

                            var snippetStart = Math.Max(0, matchStartIndex - 60);
                            var snippetEnd = Math.Min(text.Length, matchEndIndex + 60);

                            bool keywordInRange = addedRanges.Any(r => matchStartIndex >= r.start && matchStartIndex <= r.end);

                            if (!keywordInRange)
                            {
                                addedRanges.Add((snippetStart, snippetEnd));
                                var snippet = text.Substring(snippetStart, snippetEnd - snippetStart).Trim();

                                //if (snippet.Length > 130)
                                //{
                                //    snippet = snippet.Substring(0, 130);
                                //}

                                result.Add(snippet);
                            }

                            startIndex += match.Index + match.Length; 
                        }
                    }
                    else
                    {
                        var index = text.IndexOf(searchTerm, StringComparison.OrdinalIgnoreCase);
                        while (index != -1)
                        {
                            var start = Math.Max(0, index - 60);
                            var end = Math.Min(text.Length, index + searchTerm.Length + 60);

                            bool keywordInRange = addedRanges.Any(r => index >= r.start && index <= r.end);

                            if (!keywordInRange)
                            {
                                addedRanges.Add((start, end));
                                var snippet = text.Substring(start, end - start).Trim();

                                //if (snippet.Length > 130)
                                //{
                                //    snippet = snippet.Substring(0, 130);
                                //}

                                result.Add(snippet);
                            }

                            index = text.IndexOf(searchTerm, index + searchTerm.Length, StringComparison.OrdinalIgnoreCase);
                        }
                    }

                    anyTermMatched = anyTermMatched || addedRanges.Any(r => r.start != r.end); 
                }

            }

            return result;
        }

        /// <summary>
        /// Check unique email  for member site.
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

        #region Member Registertion

        /// <summary>
        /// For going to member screen/page.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isFromFree"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<IActionResult> Register(int id, bool isFromFree = false)
        {
            MemberShipRegistration model = new();
            try
            {
                if (id != null)
                {
                    var response = await _membershipRepository.GetTrialMemberAsync(id);
                    if (response != null)
                    {
                        model.ID = response.ID;
                        model.ConID = response.ConID;
                        model.Company = response.Company;
                        model.ContactEmail = response.Email;
                        var myString = response.FirstName;
                        var lastSpaceIndex = myString.LastIndexOf(" ");
                        if (lastSpaceIndex > 0)
                        {
                            model.FirstName = myString.Substring(0, lastSpaceIndex);
                            model.LastName = myString.Substring(lastSpaceIndex + 1);
                        }
                        else
                        {
                            model.FirstName = response.FirstName;
                            model.LastName = "";
                        }
                        model.MailAddress = response.MailAddress;
                        model.MailCity = response.MailCity;
                        var state = response.MailState;
                        model.MailZip = response.MailZip;
                        model.hdnpreStep = 0;
                        model.hdnnextStep = 2;
                        model.hdncurrentStep = 1;
                        model.Term = "";
                        model.CheckRadio = "";
                        var stateid = await _commonRepository.CheckStateAsync(state);
                        model.MailStateId = stateid.data[0].StateId.ToString();
                    }
                }
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
            }
            catch (Exception)
            {
                throw;
            }
            return View(model);
        }

        /// <summary>
        /// For going to Register pst method.
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
        [AllowAnonymous]
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
        [AllowAnonymous]
        public async Task<IActionResult> GoToStep2(MemberShipRegistration model, List<string> MinorityStatusList, string Div, string MailState, string BillState)
        {
            // Store blank values
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
        [AllowAnonymous]
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
        [AllowAnonymous]
        public async Task<IActionResult> GoToStep4(MemberShipRegistration model)
        {
            dynamic row;
            // Register the member information
            if (model.ID > 0)
            {
                await _membershipRepository.UpdateTrialMembershipAsync(model);
            }
            //var user = new IdentityUser { UserName = model.ContactEmail, Email = model.ContactEmail };
            ////var result = await _userManager.CreateAsync(user, model.hdnPass);

            //    #region AssignRole
            //    IdentityRole role = await _roleManager.FindByNameAsync("Member");
            //    if (role != null)
            //    {
            //        IdentityResult? res = null;
            //        var AssignUser = await _userManager.FindByEmailAsync(model.ContactEmail);
            //        res = await _userManager.AddToRoleAsync(AssignUser, role.Name);
            //    }

            //    #endregion
            //    model.REMOTE_ADDR = GetIPAddressOfClient();
            //    EmailViewModel emailObj = new();
            //    await _emailServiceManager.GetEmailForRegistration(emailObj, model);
            //    List<string> emails = new();
            //    emails.Add(model.ContactEmail == null ? "codingbrains36@gmail.com" : model.ContactEmail);
            //    emailObj.EmailTos = emails;
            //    var response = _emailServiceManager.SendEmail(emailObj);
            //DeleteInCompleteSignUp(model.ToDelete);

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
        }

        #endregion Member Registertion

        /// <summary>
        /// For going to member profile screen/page.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> MemberProfile(int id)
        {
            MemberShipRegistration model = new();
            DisplayLoginInfo logInfo = new();
            DisplayLoginInfo info = _commonRepository.GetUserInfo(User.Identity.Name);
            if (info.InActive == true)
            {
                TempData["InActive"] = "Yes";
            }
            int ConId = 0;
            string UserName = "";
            bool IsMember = false;
            if (id <= 0)
            {
                logInfo = _commonRepository.GetUserInfo(User.Identity.Name);
                if (logInfo != null)
                {
                    id = logInfo.Id;
                    ConId = logInfo.ConId;
                    UserName = logInfo.Name;
                }

                IsMember = true;
            }
            string successMessage = TempData["SuccessMessage"] as string;
            if (!string.IsNullOrEmpty(successMessage))
            {
                ViewBag.SuccessMessage = successMessage;
            }
            model = await _membershipRepository.GetMemberProfileAsync(id, ConId, UserName);

            if (!(bool)model.MainContact)
            {
                TempData["Message"] = "You do not have access for Member Profile Page.";
                return RedirectToAction("Index", "Home");
            }

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
        /// For autorenewalon post method from member's memberprofile screen/page.
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
        /// For autorenewaloff post method from Member's memberprofile screen/page.
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
        /// For activemember post method from MEmber's memberprofile screen/page.
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
        /// For inactive member post method from Member's memberprofile screen/page.
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
        /// For memberprofile post method or update memberprofile from member side.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> EditProfile(MemberShipRegistration model)
        {
            await _membershipRepository.EditMemberProfile(model);
            return RedirectToAction("MemberProfile");
        }

        /// <summary>
        /// Add a new user for particular member/company on member's User Management section.
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
                IdentityRole role = await _roleManager.FindByNameAsync("Member");
                _entityRepository.UpdateContactUserId(model.ContactEmail, user);
                if (role != null)
                {
                    IdentityResult? res = null;
                    var AssignUser = await _userManager.FindByEmailAsync(model.ContactEmail);
                    res = await _userManager.AddToRoleAsync(AssignUser, role.Name);
                }
            }
            return Json(response);
        }
        string GetCookieValue(string cookieName)
        {
            return Request.Cookies[cookieName]?.ToString() ?? "";
        }
        void DeleteCookieIfExists(string cookieName)
        {
            if (Request.Cookies[cookieName] != null)
            {
                Response.Cookies.Delete(cookieName);
            }
        }
        /// <summary>
        /// For going to find project here screen/page.
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> FindProjectHere(int status = 0)
        {
            DisplayLoginInfo info = _commonRepository.GetUserInfo(User.Identity.Name);
            if (info.InActive == true)
            {
                TempData["InActive"] = "Yes";
                return View("AccessDenied");
            }
            ViewBag.ActiveSearchText = GetCookieValue("dtActiveSearchText");
            ViewBag.FutureSearchText = GetCookieValue("dtFutureSearchText");
            ViewBag.PreviousSearchText = GetCookieValue("dtPreviousSearchText");
            DeleteCookieIfExists("dtActiveSearchText");
            DeleteCookieIfExists("dtFutureSearchText");
            DeleteCookieIfExists("dtPreviousSearchText");
            List<SelectListItem> StateList = await _implementRepository.GetDistinctState();
            StateList.Insert(0, new SelectListItem { Value = "0", Text = "--Select State--" });
            ViewBag.States = StateList;
            string usern = User.Identity.Name;
            FindProjectModel model = new FindProjectModel();
            //string appName = _Configuration.GetSection("AppSettings")["SecurityKey"];
            var modelString = HttpContext.Session.GetString("SearchViewData");
            //var appliedfilters = HttpContext.Session.GetString("AppliedFilters");
            //if (appliedfilters != null)
            //{
            //    return RedirectToAction("FindProjectHereSearch", new { model =  JsonConvert.DeserializeObject<SearchViewModel>(appliedfilters) });
            //}
            if (status == 0 || modelString == null)
            {
                HttpContext.Session.Remove("AppliedFilters");
                model = await _membershipRepository.GetDashboardProjectsAsync(info);
            }
            else if (status == 1)
            {
                model = JsonConvert.DeserializeObject<FindProjectModel>(modelString);
            }
            return View(model);
        }

        /// <summary>
        /// For going to search project post method on member findprojecthere screen/page.
        /// </summary>
        /// <param name="model"></param>
        /// <param name="checkvalue"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> FindProjectHereSearch(SearchViewModel model, int checkvalue = 0)
        {
            var properties = typeof(SearchViewModel).GetProperties();
            foreach (var property in properties)
            {
                if (property.PropertyType == typeof(List<string>))
                {
                    var list = (List<string>?)property.GetValue(model);
                    if (list != null && list.All(val => val == null))
                    {
                        property.SetValue(model, new List<string>());
                    }
                }
            }
            HttpContext.Session.Remove("AppliedFilters");
            FindProjectModel modal = new();
            string usern = User.Identity.Name;
            DisplayLoginInfo info = _commonRepository.GetUserInfo(usern);
            List<SelectListItem> StateList = await _implementRepository.GetDistinctState();
            StateList.Insert(0, new SelectListItem { Value = "0", Text = "--Select State--" });
            ViewBag.States = StateList;
            if (!string.IsNullOrEmpty(model.City) && ((model.StateList != null && model.StateList.Count > 0) || (!string.IsNullOrEmpty(model.State) && model.State != "0")) && model.Distance != null && model.Distance > 0)
            {
                HttpResponseDetail<dynamic> response = new();
                var stateId = model.State;
                model.State = _membershipRepository.GetSelectedStateText(model.State);
                string username = User.Identity.Name;
                model.Distance = model.Distance * 1609.344;

                response = await _membershipRepository.RadiusSearchAsync(model, username);
                FindProjectModel res = await _membershipRepository.GetSortByStateFindProjectsAsync(response.data, model, 1, info);
                modal = res;
                model.Distance = model.Distance / 1609.344;
                model.State = stateId;
            }
            else if (!string.IsNullOrEmpty(model.City) || (!string.IsNullOrEmpty(model.State) && model.State != "0"))
            {
                FindProjectModel res = await _membershipRepository.GetSortByStateFindProjectsAsync(new List<ProjectInformation>(), model, 2, info);
                if (!string.IsNullOrEmpty(model.City) && (!string.IsNullOrEmpty(model.State) && model.State != "0"))
                {
                    var stateName = _membershipRepository.GetSelectedStateText(model.State);
                    modal.ActiveProjs = res.ActiveProjs.FindAll(m => m.LocCity != null && m.LocState != null && m.LocCity.ToLower() == model.City.ToLower() && m.LocState.ToLower() == stateName.ToLower());
                    modal.PrevProjs = res.PrevProjs.FindAll(m => m.LocCity != null && m.LocState != null && m.LocCity.ToLower() == model.City.ToLower() && m.LocState.ToLower() == stateName.ToLower());
                    modal.FutureProjs = res.FutureProjs.FindAll(m => m.LocCity != null && m.LocState != null && m.LocCity.ToLower() == model.City.ToLower() && m.LocState.ToLower() == stateName.ToLower());
                }
                else
                {
                    if (!string.IsNullOrEmpty(model.City))
                    {
                        modal.ActiveProjs = res.ActiveProjs.FindAll(m => m.LocCity != null && m.LocCity.ToLower() == model.City.ToLower());
                        modal.PrevProjs = res.PrevProjs.FindAll(m => m.LocCity != null && m.LocCity.ToLower() == model.City.ToLower());
                        modal.FutureProjs = res.FutureProjs.FindAll(m => m.LocCity != null && m.LocCity.ToLower() == model.City.ToLower());
                    }
                    else if (!string.IsNullOrEmpty(model.State) && model.State != "0")
                    {
                        var stateName = _membershipRepository.GetSelectedStateText(model.State);
                        modal.ActiveProjs = res.ActiveProjs.FindAll(m => m.LocState != null && m.LocState.ToLower() == stateName.ToLower());
                        modal.PrevProjs = res.PrevProjs.FindAll(m => m.LocState != null && m.LocState.ToLower() == stateName.ToLower());
                        modal.FutureProjs = res.FutureProjs.FindAll(m => m.LocState != null && m.LocState.ToLower() == stateName.ToLower());
                    }
                }
            }
            else
            {
                FindProjectModel res = await _membershipRepository.GetSortByStateFindProjectsAsync(new List<ProjectInformation>(), model, 2, info);
                modal = res;
            }
            string modelString = JsonConvert.SerializeObject(modal);
            if (modal != null)
            {
                HttpContext.Session.SetString("SearchViewData", modelString);
                HttpContext.Session.SetString("AppliedFilters", JsonConvert.SerializeObject(model));             

                if (checkvalue == 1)
                {
                    return Json("Success");
                }
                return RedirectToAction("FindProjectHere", new { status = 1 });
            }
            else
            {
                return RedirectToAction("FindProjectHere", new { status = 0 });
            }
        }

        public async Task<string> AppliedFilters()
        {
            if (HttpContext.Session.GetString("AppliedFilters") != null)
            {
                return JsonConvert.SerializeObject(HttpContext.Session.GetString("AppliedFilters"));
            }
            return null;
        }

        /// <summary>
        /// For add project to member dashboard from find project here screen/page.
        /// </summary>
        /// <param name="Change"></param>
        /// <param name="ProjId"></param>
        /// <returns></returns>
        public async Task<IActionResult> AddDashboard(bool Change, string ProjId)
        {
            string usern = User.Identity.Name;
            await _membershipRepository.AddToDashboard(Change, ProjId, usern);
            //IEnumerable<OrderTables> model = await _membershipRepository.GetDashboardProjectsAsync();
            return RedirectToAction("CopyCenter");
            //View(model);
        }

        /// <summary>
        /// Get package card details price.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<JsonResult> FindPriceDetail()
        {
            IEnumerable<PaymentInfo> model = await _membershipRepository.FindPriceDetail();
            return Json(model);
        }

        /// <summary>
        /// No use
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> FindProjectHereDesign()
        {
            return View();
        }

        /// <summary>
        /// For going to post project here screen/page.
        /// </summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        [Authorize]
        public async Task<IActionResult> PostProjectHere(string msg = "N")
        {
            MemberProjectInfo model = new();
            DisplayLoginInfo logInfo = new();
            logInfo = _commonRepository.GetUserInfo(User.Identity.Name);
            if (logInfo.InActive == true)
            {
                TempData["InActive"] = "Yes";
                return View("AccessDenied");
            }
            if (logInfo != null)
            {
                model.ContactMember = logInfo.Company;
                model.ContactName = logInfo.Name;
                model.ContactPhone = logInfo.Phone;
                model.memberId = logInfo.Id;
                model.ContactEmail = logInfo.Email;
                model.ErrorMsg = msg;
            }
            List<string> objList = new List<string>();
            objList.Add("New Construction");
            objList.Add("Remodel");
            objList.Add("Addition");
            model.ProjScopeList = objList;
            List<SelectListItem> PHLType = await _projectRepository.GetPhlType();
            ViewBag.PHLType = PHLType;
            List<SelectListItem> BidOption = await _projectRepository.GetBidOption();
            BidOption.Insert(0, new SelectListItem { Value = "0", Text = "--No Selection--" });
            ViewBag.BidOption = BidOption;
            return View(model);
        }

        /// <summary>
        /// For get user dtails.
        /// </summary>
        /// <param name="Email"></param>
        /// <returns></returns>
        public JsonResult GetUserInfo(string Email)
        {
            DisplayLoginInfo model = _commonRepository.GetUserInfo(Email);
            return Json(model);
        }

        /// <summary>
        /// For upload pdf/file post method.
        /// </summary>
        /// <param name="pdfFile"></param>
        /// <param name="projNum"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UploadPdf(List<IFormFile> pdfFile, string projNum)
        {
            string wwwPath = this._webHostEnvironment.WebRootPath;
            string contentPath = this._webHostEnvironment.ContentRootPath;
            string pathPhoto = Path.Combine(this._webHostEnvironment.WebRootPath, projNum);
            pathPhoto = Path.Combine(pathPhoto, "Uploads");
            if (!Directory.Exists(pathPhoto))
            {
                Directory.CreateDirectory(pathPhoto);
            }
            foreach (IFormFile formFile in pdfFile)
            {
                string fileName = Path.GetFileName(formFile.FileName);
                fileName = fileName.Contains(" ") ? fileName.Replace(" ", "_") : fileName;
                using (FileStream stream = new FileStream(Path.Combine(pathPhoto, fileName), FileMode.Create))
                {
                    formFile.CopyTo(stream);
                }
                string message = string.Format("{0}", fileName);
            }
            //return Json(new { Status = "success", Message = message });
            return Json(new { Status = "success" });
        }

        /// <summary>
        /// For upload pdf/file on aws s3
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        public async Task UploadFileToS3(IFormFile file)
        {
            using (var client = new AmazonS3Client("AKIAQD4LHUTEXBKECZF4", "+HcEbEXF9nP9etVuxqEHMArFsmj6r5FWHQ5/iQ0S", RegionEndpoint.USEast1))
            {
                await using (var newMemoryStream = new MemoryStream())
                {
                    try
                    {
                        file.CopyTo(newMemoryStream);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }

                    var uploadRequest = new TransferUtilityUploadRequest
                    {
                        InputStream = newMemoryStream,
                        Key = file.FileName,
                        BucketName = "contractor-aws",
                        CannedACL = S3CannedACL.PublicRead
                    };

                    var fileTransferUtility = new TransferUtility(client);
                    await fileTransferUtility.UploadAsync(uploadRequest);
                }
            }
        }

        /// <summary>
        /// For save a project or postprojecthere post method.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<IActionResult> SaveMemberProject(MemberProjectInfo model)
        {
            HttpResponseDetail<dynamic> httpResponse = new();
            string user = User.Identity.Name;
            DisplayLoginInfo info = new();
            httpResponse = await _membershipRepository.SaveProjectInfoAsync(model);
            model.REMOTE_ADDR = GetIPAddressOfClient();
            model.AuthorizedBy = user;
            EmailViewModel emailObj = new();
            var emailsubject = await _emailServiceManager.UploadPostProject(emailObj, model);
            emailObj.EmailTos = model.EmailsTo;
            emailObj.strMessage = emailsubject;
            var httpresponse = _emailServiceManager.SendEmail(emailObj);

            if (httpResponse != null)
            {
                // Extract year, month, and project number from the project number string
                string year = "20" + model.ProjNumber.Substring(0, 2);
                string month = model.ProjNumber.Substring(2, 2);
                string projNumber = model.ProjNumber;

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

                // Define the path for photo uploads
                string pathPhoto = Path.Combine(this._webHostEnvironment.WebRootPath, model.ProjNumber, "Uploads");

                if (Directory.Exists(pathPhoto))
                {
                    string[] fileArray = Directory.GetFiles(pathPhoto);
                    foreach (string s in fileArray)
                    {
                        SaveToLocalPath(s, Path.Combine(projectPath, "Uploads"));
                    }
                    Directory.Delete(pathPhoto, true);
                }

                return RedirectToAction("postprojecthere", new { msg = "OK" });
            }
            return RedirectToAction("postprojecthere", new { msg = "Y" });
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
        public void UploadToS3(string filePath, string subFolder)
        {
            try
            {
                TransferUtility fileTransferUtility = new
                    TransferUtility(new AmazonS3Client("AKIAQD4LHUTEXBKECZF4", "+HcEbEXF9nP9etVuxqEHMArFsmj6r5FWHQ5/iQ0S", RegionEndpoint.USEast1));

                string bucketName = "contractor-aws";

                //if (_bucketSubdirectory == "" || _bucketSubdirectory == null)
                //{
                //    bucketName = _bucketName; //no subdirectory just bucket name
                //}
                //else
                //{   // subdirectory and bucket name
                bucketName = bucketName + @"/" + subFolder;
                //}

                // 1. Upload a file, file name is used as the object key name.
                fileTransferUtility.Upload(filePath, bucketName);
                //Console.WriteLine("Upload 1 completed");
            }
            catch (AmazonS3Exception s3Exception)
            {
                //Console.WriteLine(s3Exception.Message,
                //s3Exception.InnerException);
            }
        }

        //This was all for uploading files in Amazon S3 bucket.I worked on.netcore 2.0 and also, don't forget to add necessary
        public async Task CreateFolder(string folderName)
        {
            var folderKey = folderName + "/"; //end the folder name with "/"

            var request = new PutObjectRequest();
            request.BucketName = "contractor-aws";
            //request.WithBucketName(bucketName);
            IAmazonS3 client =
            new AmazonS3Client("AKIAQD4LHUTEXBKECZF4", "+HcEbEXF9nP9etVuxqEHMArFsmj6r5FWHQ5/iQ0S",
            RegionEndpoint.USEast1);
            request.StorageClass = S3StorageClass.Standard;
            request.ServerSideEncryptionMethod = ServerSideEncryptionMethod.None;

            //request.CannedACL = S3CannedACL.BucketOwnerFullControl;
            request.Key = folderKey;
            request.ContentBody = string.Empty;
            //request.WithKey(folderKey);
            //request.WithContentBody(string.Empty);
            PutObjectResponse response = await client.PutObjectAsync(request);
            //S3Response response = m_S3Client.PutObject(request);
        }

        /// <summary>
        /// For savesearch post method from find project here screen/page.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> SaveSearch(SearchViewModel model)
        {
            string usern = User.Identity.Name;
            DisplayLoginInfo loginInfo = _commonRepository.GetUserInfo(usern);
            HttpResponseDetail<dynamic> response = new();
            if (model.Id > 0)
            {
                var status = await _membershipRepository.EditSaveSearch(model, loginInfo);
                response = status;
            }
            else
            {
                var status = await _membershipRepository.SaveSearch(model, loginInfo);
                if (status > 0)
                {
                    response.success = true;
                    response.statusMessage = "Search details saved successfully";
                }
                else
                {
                    response.success = false;
                    response.statusMessage = "Some problem occurred";
                }
            }

            return Json(response);
        }

        /// <summary>
        /// Get save search list on find project here screen/page.
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetSavedSearch()
        {
            string usern = User.Identity.Name;
            DisplayLoginInfo loginInfo = _commonRepository.GetUserInfo(usern);
            HttpResponseDetail<dynamic> response = new();
            List<SearchViewModel> svm = await _membershipRepository.GetSavedSearch(loginInfo);
            return Json(svm);
        }

        /// <summary>
        /// For gotosavesearch post method.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResult> GoToSavedSearch(int id)
        {
            SearchViewModel searchView = await _membershipRepository.GoToSavedSearch(id);
            List<ProjectInformation> model = new();
            DisplayLoginInfo loginInfo = _commonRepository.GetUserInfo(User.Identity.Name);
            FindProjectModel response = new();
            if (!string.IsNullOrEmpty(searchView.City) && ((searchView.StateList != null && searchView.StateList.Count > 0) || searchView.State != null) && searchView.Distance != null && searchView.Distance >= 0)
            {
                HttpResponseDetail<dynamic> Htresponse = new();
                int chkState = 0;
                int.TryParse(searchView.State, out chkState);
                if (chkState != 0)
                {
                    searchView.State = _membershipRepository.GetSelectedStateText(searchView.State);
                }
                string username = User.Identity.Name;
                searchView.Distance = searchView.Distance;

                Htresponse = await _membershipRepository.RadiusSearchAsync(searchView, User.Identity.Name);
                response = await _membershipRepository.GetSortByStateFindProjectsAsync(Htresponse.data, searchView, 1, loginInfo);
            }
            else
                response = await _membershipRepository.GetSortByStateFindProjectsAsync(model, searchView, 2, loginInfo);
            if (response != null)
            {
                if (response.ActiveProjs != null && response.ActiveProjs.Count > 0)
                {
                    foreach (var item in response.ActiveProjs)
                    {
                        string Location = "";
                        item.Publish = item.Publish != null ? (bool)item.Publish : false;
                        item.SpecsOnPlans = item.SpecsOnPlans != null ? (bool)item.SpecsOnPlans : false;
                        item.MemberTrack = item.MemberTrack != null ? (bool)item.MemberTrack : false;
                        var strbd = "";
                        var BiDateValue = DateTime.Now.Date;
                        DateTime dateTimeValue = DateTime.MinValue;
                        try
                        {
                            dateTimeValue = DateTime.ParseExact(item.strBidDt5, "HH:mm", CultureInfo.InvariantCulture);
                        }
                        catch (Exception ex)
                        {
                            // Handle the exception here, e.g., log an error or set a default value
                            dateTimeValue = DateTime.MinValue;
                        }
                        string BidTime = dateTimeValue == DateTime.MinValue ? "" : dateTimeValue.ToString("hh:mm tt");
                        if (item.strBidDt == "TBD" || item.BidDt == null)
                        {
                            strbd = "TBD";
                        }
                        else
                        {
                            string inputDate = item.BidDt >= BiDateValue ? Convert.ToDateTime(item.BidDt).ToString("MM/dd/yyyy") : "";
                            if (inputDate != "")
                            {
                                DateTime date = DateTime.ParseExact(inputDate, "MM/dd/yyyy", null);
                                string formattedDate = date.ToString("MMM dd yyyy");
                                strbd = formattedDate;
                                strbd = BidTime == "" ? strbd : strbd + " " + BidTime + " PT";
                            }
                        }
                        item.strBidDt5 = strbd;
                        string ConsBillAddress = "";
                        ConsBillAddress = string.IsNullOrEmpty(item.LocAddr1) ? "" : item.LocAddr1;
                        ConsBillAddress = string.IsNullOrEmpty(item.LocCity) ? ConsBillAddress : (string.IsNullOrEmpty(ConsBillAddress) ? item.LocCity : ConsBillAddress + ", " + item.LocCity);
                        ConsBillAddress = string.IsNullOrEmpty(item.LocState) ? ConsBillAddress : (string.IsNullOrEmpty(ConsBillAddress) ? item.LocState : ConsBillAddress + ", " + item.LocState);
                        ConsBillAddress = string.IsNullOrEmpty(item.LocAddr2) ? ConsBillAddress : (string.IsNullOrEmpty(ConsBillAddress) ? item.LocAddr2 : ConsBillAddress + ", " + item.LocAddr2);
                        ConsBillAddress = string.IsNullOrEmpty(item.LocZip) ? ConsBillAddress : (string.IsNullOrEmpty(ConsBillAddress) ? item.LocZip : ConsBillAddress + ", " + item.LocZip);
                        item.LocAddr1 = ConsBillAddress;
                    }
                }
                if (response.FutureProjs != null && response.FutureProjs.Count > 0)
                {
                    foreach (var item in response.FutureProjs)
                    {
                        string Location = "";
                        item.Publish = item.Publish != null ? (bool)item.Publish : false;
                        item.SpecsOnPlans = item.SpecsOnPlans != null ? (bool)item.SpecsOnPlans : false;
                        item.MemberTrack = item.MemberTrack != null ? (bool)item.MemberTrack : false;
                        var strbd = "";
                        var BiDateValue = DateTime.Now.Date;
                        DateTime dateTimeValue = DateTime.MinValue;
                        try
                        {
                            dateTimeValue = DateTime.ParseExact(item.strBidDt5, "HH:mm", CultureInfo.InvariantCulture);
                        }
                        catch (Exception ex)
                        {
                            // Handle the exception here, e.g., log an error or set a default value
                            dateTimeValue = DateTime.MinValue;
                        }
                        string BidTime = dateTimeValue == DateTime.MinValue ? "" : dateTimeValue.ToString("hh:mm tt");
                        if (item.strBidDt == "TBD" || item.BidDt == null)
                        {
                            strbd = "TBD";
                        }
                        else
                        {
                            string inputDate = item.BidDt >= BiDateValue ? Convert.ToDateTime(item.BidDt).ToString("MM/dd/yyyy") : "";
                            if (inputDate != "")
                            {
                                DateTime date = DateTime.ParseExact(inputDate, "MM/dd/yyyy", null);
                                string formattedDate = date.ToString("MMM dd yyyy");
                                strbd = formattedDate;
                                strbd = BidTime == "" ? strbd : strbd + " " + BidTime + " PT";
                            }
                        }
                        item.strBidDt5 = strbd;
                        string ConsBillAddress = "";
                        ConsBillAddress = string.IsNullOrEmpty(item.LocAddr1) ? "" : item.LocAddr1;
                        ConsBillAddress = string.IsNullOrEmpty(item.LocCity) ? ConsBillAddress : (string.IsNullOrEmpty(ConsBillAddress) ? item.LocCity : ConsBillAddress + ", " + item.LocCity);
                        ConsBillAddress = string.IsNullOrEmpty(item.LocState) ? ConsBillAddress : (string.IsNullOrEmpty(ConsBillAddress) ? item.LocState : ConsBillAddress + ", " + item.LocState);
                        ConsBillAddress = string.IsNullOrEmpty(item.LocAddr2) ? ConsBillAddress : (string.IsNullOrEmpty(ConsBillAddress) ? item.LocAddr2 : ConsBillAddress + ", " + item.LocAddr2);
                        ConsBillAddress = string.IsNullOrEmpty(item.LocZip) ? ConsBillAddress : (string.IsNullOrEmpty(ConsBillAddress) ? item.LocZip : ConsBillAddress + ", " + item.LocZip);
                        item.LocAddr1 = ConsBillAddress;
                    }
                }
                if (response.PrevProjs != null && response.PrevProjs.Count > 0)
                {
                    foreach (var item in response.PrevProjs)
                    {
                        string Location = "";
                        item.Publish = item.Publish != null ? (bool)item.Publish : false;
                        item.SpecsOnPlans = item.SpecsOnPlans != null ? (bool)item.SpecsOnPlans : false;
                        item.MemberTrack = item.MemberTrack != null ? (bool)item.MemberTrack : false;
                        var strbd = "";
                        var BiDateValue = DateTime.Now.Date;
                        DateTime dateTimeValue = DateTime.MinValue;
                        try
                        {
                            dateTimeValue = DateTime.ParseExact(item.strBidDt5, "HH:mm", CultureInfo.InvariantCulture);
                        }
                        catch (Exception ex)
                        {
                            // Handle the exception here, e.g., log an error or set a default value
                            dateTimeValue = DateTime.MinValue;
                        }
                        string PrevInputDate = item.BidDt != null ? Convert.ToDateTime(item.BidDt).ToString("MM/dd/yyyy") : "";
                        DateTime Prevdate = DateTime.ParseExact(PrevInputDate, "MM/dd/yyyy", null);
                        string formattedDate = Prevdate.ToString("MMM dd yyyy");
                        string BidTime = dateTimeValue == DateTime.MinValue ? "" : dateTimeValue.ToString("hh:mm tt");
                        item.strBidDt5 = formattedDate + BidTime;
                        string ConsBillAddress = "";
                        ConsBillAddress = string.IsNullOrEmpty(item.LocAddr1) ? "" : item.LocAddr1;
                        ConsBillAddress = string.IsNullOrEmpty(item.LocCity) ? ConsBillAddress : (string.IsNullOrEmpty(ConsBillAddress) ? item.LocCity : ConsBillAddress + ", " + item.LocCity);
                        ConsBillAddress = string.IsNullOrEmpty(item.LocState) ? ConsBillAddress : (string.IsNullOrEmpty(ConsBillAddress) ? item.LocState : ConsBillAddress + ", " + item.LocState);
                        ConsBillAddress = string.IsNullOrEmpty(item.LocAddr2) ? ConsBillAddress : (string.IsNullOrEmpty(ConsBillAddress) ? item.LocAddr2 : ConsBillAddress + ", " + item.LocAddr2);
                        ConsBillAddress = string.IsNullOrEmpty(item.LocZip) ? ConsBillAddress : (string.IsNullOrEmpty(ConsBillAddress) ? item.LocZip : ConsBillAddress + ", " + item.LocZip);
                        item.LocAddr1 = ConsBillAddress;
                    }
                }
            }
            return Json(response);
        }

        /// <summary>
        /// For save incomplete signup post method.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<JsonResult> SaveInCompleteSignUp(MemberShipRegistration model)
        {
            return Json(await _membershipRepository.SaveInCompleteSignUpAsync(model));
        }

        /// <summary>
        /// For delete incomplete signup post method when signup completed.
        /// </summary>
        /// <param name="ToDelete"></param>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task DeleteInCompleteSignUp(int ToDelete)
        {
            await _membershipRepository.DeleteInCompleteSignUp(ToDelete);
        }

        /// <summary>
        /// For going to copycenter screen/page on member site.
        /// </summary>
        /// <param name="LoadValChk"></param>
        /// <returns></returns>
        public async Task<ActionResult> CopyCenter(string LoadValChk = "NoValue")
        {
            DisplayLoginInfo info = _commonRepository.GetUserInfo(User.Identity.Name);
            if (info.InActive == true)
            {
                TempData["InActive"] = "Yes";
                return View("AccessDenied");
            }
            ViewBag.LoadValChk = LoadValChk;
            DisplayLoginInfo loginInfo = _commonRepository.GetUserInfo(User.Identity.Name);
            IEnumerable<OrderTables> model = await _membershipRepository.GetCopyCenterData(loginInfo.ConId);
            List<SelectListItem> StateList = await _membershipRepository.GetStates();
            StateList.Insert(0, new SelectListItem { Value = "0", Text = "--Select State--" });
            ViewBag.States = StateList;
            return View(model);
        }

        /// <summary>
        /// For get particular login user/member data.
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<JsonResult> AutoFill()
        {
            AutoFillData response = new();
            DisplayLoginInfo loginInfo = _commonRepository.GetUserInfo(User.Identity.Name);
            response = await _membershipRepository.AutoFill(loginInfo.ConId);
            return Json(response);
        }

        /// <summary>
        /// For upload pdf/file.
        /// </summary>
        /// <param name="pdfFile"></param>
        /// <returns></returns>
        public JsonResult UploadPdf(List<IFormFile> pdfFile)
        {
            string pathPhoto = Path.Combine(this._webHostEnvironment.WebRootPath, "Storage");
            pathPhoto = CreatePath(pathPhoto);
            string pathTemp = pathPhoto.Substring(0, pathPhoto.LastIndexOf("\\") + 1);
            pathTemp = pathPhoto.Replace(pathTemp, "");
            Directory.CreateDirectory(pathPhoto);
            foreach (IFormFile formFile in pdfFile)
            {
                string fileName = Path.GetFileName(formFile.FileName);
                using (FileStream stream = new FileStream(Path.Combine(pathPhoto, fileName), FileMode.Create))
                {
                    formFile.CopyTo(stream);
                }
                string message = string.Format("{0}", fileName);
            }
            //return Json(new { Status = "success", Message = message });
            return Json(new { Status = "success", Data = pathTemp, Flag = 'Y' });
        }

        /// <summary>
        /// For save copy center post method.
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
            string pathPhoto = Path.Combine(this._webHostEnvironment.WebRootPath, "Storage");
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
        /// For view copy order doc in a new tab
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
        /// Upload pdffile on copycenter page for order copy post method.
        /// </summary>
        /// <param name="pdfFile"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult UploadPrintPdf(List<IFormFile> pdfFile, string time)
        {
            string wwwPath = this._webHostEnvironment.WebRootPath;
            string contentPath = this._webHostEnvironment.ContentRootPath;
            string pathPhoto = Path.Combine(this._webHostEnvironment.WebRootPath, "Storage");
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
        /// For reorder copy on member's copycenter screen/page.
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
                string rootPath = _webHostEnvironment.WebRootPath;
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
        /// For going to memberdirectory screen/page.
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> MemberDirectory()
        {
            DisplayLoginInfo info = _commonRepository.GetUserInfo(User.Identity.Name);
            if (info.InActive == true)
            {
                TempData["InActive"] = "Yes";
                return View("AccessDenied");
            }
            IEnumerable<MemberShipRegistration> model = await _membershipRepository.GetMemberDirectoryAsync();
            return View(model);
        }

        /// <summary>
        /// For show a card with company details on memberdirectory screen/page.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<JsonResult> ShowCard(int Id)
        {
            HttpResponseDetail<dynamic> response = await _membershipRepository.ShowCard(Id);
            return Json(response);
        }

        /// <summary>
        /// From service
        /// </summary>
        /// <param name="T1"></param>
        /// <param name="T2"></param>
        /// <param name="T3"></param>
        /// <returns></returns>
        public async Task<ActionResult> ProjectUpdate(string T1, string T2, string T3)
        {
            string appName = _configuration.GetSection("AppSettings")["SecurityKey"];
            string DecEmail = EncryptDecryption.DecryptString(appName, T1);
            string DecProjs = EncryptDecryption.DecryptString(appName, T2);
            string DecTime = EncryptDecryption.DecryptString(appName, T3);
            DisplayLoginInfo logInfo = new();
            logInfo = await _commonRepository.GetUserInfoAsync(User.Identity.Name);
            int memberType = 0;
            memberType = _entityRepository.GetEntities().Where(x => x.Id == logInfo.Id)?.FirstOrDefault()?.MemberType == null ? 0 : Convert.ToInt32(_entityRepository.GetEntities().Where(x => x.Id == logInfo.Id)?.FirstOrDefault()?.MemberType);
            if (DecEmail != User.Identity.Name || memberType == 0 || memberType == 1)
            {
                return View("Unautorized");
            }
            else
            if (DateTime.Now > Convert.ToDateTime(DecTime))
            {
                return View("Expired");
            }
            else
                if (memberType == 7)
            {
                IEnumerable<ProjectInformation> response = await _membershipRepository.GetProjectUpdatePacificAsync(logInfo.ConId);
                return View(response);
            }
            else
            {
                List<int> Pids = new();
                string Proj = "";
                for (int i = 0; i < DecProjs.Length; i++)
                {
                    if (DecProjs[i].ToString() == "\\")
                    {
                        if (Proj.Contains(","))
                            Proj = Proj.Replace(",", "");
                        int x = 0;
                        bool a = int.TryParse(Proj, out x);
                        if (x != 0)
                        {
                            Pids.Add(x);
                        }
                        Proj = "";
                    }
                    else
                    {
                        Proj += DecProjs[i].ToString();
                    }
                }
                IEnumerable<ProjectInformation> response = await _membershipRepository.GetProjectUpdateAsync(Pids, logInfo.ConId);
                return View(response);
            }
        }

        /// <summary>
        /// No use
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Unautorized()
        {
            return View();
        }

        /// <summary>
        /// No use
        /// </summary>
        /// <returns></returns>
        public async Task<ActionResult> Expired()
        {
            return View();
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
        /// For save License from member's memberprofile screen/page.
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
        /// Add location from member memberprofile screen/page.
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
        /// For removefrom dashboard post method from member dashboard screen/page.
        /// </summary>
        /// <param name="ProjId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> RemoveFromDashboard(int ProjId, DisplayLoginInfo model)
        {
            HttpResponseDetail<dynamic> response = new();
            response = await _membershipRepository.RemoveFromDashboardProj(ProjId, model);
            //IEnumerable<OrderTables> model = await _membershipRepository.GetDashboardProjectsAsync();
            return Json(response);
            //View(model);
        }

        /// <summary>
        /// For bidding post method from member dashboard screen/page.
        /// </summary>
        /// <param name="ProjId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> AddBidding(string ProjId, DisplayLoginInfo model)
        {
            await _membershipRepository.AddBiddingProj(ProjId, model);
            HttpResponseDetail<dynamic> response = new();
            //IEnumerable<OrderTables> model = await _membershipRepository.GetDashboardProjectsAsync();
            return Json(response);
            //View(model);
        }

        /// <summary>
        /// For get plan holder list pdf/file on dashboard screen/page.
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
        /// Add to calendar post method on dashboard screen/page.
        /// </summary>
        /// <param name="ProjId"></param>
        /// <param name="BidDate"></param>
        /// <param name="Ischecked"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<JsonResult> AddToCalendar(int ProjId, DateTime BidDate, bool Ischecked, DisplayLoginInfo model)
        {
            HttpResponseDetail<dynamic> response = new();
            response = await _membershipRepository.AddToCalendar(ProjId, BidDate, model, Ischecked);
            return Json(response);
        }

        /// <summary>
        /// For onchange upload companylogo from member's memberprofile screen/page.
        /// </summary>
        /// <param name="pdfFile"></param>
        /// <returns></returns>
        public JsonResult UploadLogo(IFormFile pdfFile)
        {
            string pathPhoto = Path.Combine(this._webHostEnvironment.WebRootPath, "Profile");
            string fileName = Path.GetFileName(pdfFile.FileName);
            string extention = Path.GetExtension(fileName);
            using (FileStream stream = new FileStream(Path.Combine(pathPhoto, fileName), FileMode.Create))
                pdfFile.CopyTo(stream);
            string data = "\\Profile\\" + fileName;
            return Json(new { Status = "success", data = data, Flag = 'Y' });
        }

        /// <summary>
        /// For update directory post method in member directory screen/page.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> UpdateDirectory(MemberShipRegistration model)
        {
            return Json(await _commonRepository.UpdateDirectoryAsync(model));
        }

        /// <summary>
        /// For make a user to admin on member's memberprofile(User management section on check admin column check box post method).
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> AdminUserContact(MemberShipRegistration model)
        {
            return Json(await _commonRepository.AdminUserContactAsync(model));
        }

        /// <summary>
        /// for use user daily report checkbox on member's memberprofile(MemberUserDailyReport post method on User management section).
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> MemberUserDailyReport(MemberShipRegistration model)
        {
            return Json(await _commonRepository.MemberUserDailyReportAsync(model));
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
        /// For order copies from member's preview screen/page.
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="filepath"></param>
        /// <returns></returns>
        public JsonResult OrderCopies(string filename, string filepath)
        {
            string pathPhoto = Path.Combine(this._webHostEnvironment.WebRootPath, "Storage");
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
        /// For order all copies on member's preview screen/page.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public async Task<JsonResult> OrderCopiesAll(List<PHLInfo> model)
        {
            try
            {
                string pathPhoto = Path.Combine(this._webHostEnvironment.WebRootPath, "Storage");
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

        public async Task<IActionResult> RadiousSearch(SearchViewModel model)
        {
            HttpResponseDetail<dynamic> response = new();
            model.State = _membershipRepository.GetSelectedStateText(model.State);
            string username = User.Identity.Name;
            model.Distance = model.Distance * 1609.344;
            response = await _membershipRepository.RadiusSearchAsync(model, username);
            response.TempValue = 1;
            return RedirectToAction("FindProjectHere", "Member", new { info = response.data, i = response.TempValue });
        }

        /// <summary>
        /// For view/download bid results on preview screen/page.
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        public async Task<IActionResult> ViewBidRes(int Id)
        {
            List<string> lstPdf = new List<string>();
            ProjectInformation model = await _projectRepository.GetProjectPreview(Id);
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
        /// For get copy center price list.
        /// </summary>
        /// <returns></returns>
        public async Task<JsonResult> GetCopyCenterPriceList()
        {
            return Json(await _commonRepository.GetCopyCenterPriceListAsync());
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
                string rootFolder = model.ProjNumber.Substring(0, 2);
                rootFolder = rootFolder.Replace(rootFolder, "20" + rootFolder);
                string subRootFolder = model.ProjNumber.Substring(2, 2);
                CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Addenda");
                CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Bid Results");
                CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/PHL");
                CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Plans");
                CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Specs");
            }
            catch (Exception Ex) { }
            ProjectInformation model1 = new();
            List<string> objList = new List<string>();
            objList.Add("New Construction");
            objList.Add("Remodel");
            objList.Add("Addition");
            model1.ProjScopeList = objList;
            return RedirectToAction("EditProjectInfo", new { id = Id });
        }

        /// <summary>
        /// Save Project then go back to EditProjectInfo
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveProjectInfoNew(ProjectInformation model)
        {
            try
            {
                model.Contact = User.Identity.Name;
                await _projectRepository.SaveProjectInfoAsync(model);
                string rootFolder = model.ProjNumber.Substring(0, 2);
                rootFolder = rootFolder.Replace(rootFolder, "20" + rootFolder);
                string subRootFolder = model.ProjNumber.Substring(2, 2);
                //bool test = await DoesFolderExist(rootFolder + "/" + subRootFolder + "/");
                //if(!test)
                CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Addenda");
                CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Bid Results");
                CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/PHL");
                CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Plans");
                CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Specs");
            }
            catch (Exception Ex) { }
            ProjectInformation model1 = new();
            List<string> objList = new List<string>();
            objList.Add("New Construction");
            objList.Add("Remodel");
            objList.Add("Addition");
            model1.ProjScopeList = objList;
            return RedirectToAction("EditProjectInfo", "Project");
        }

        /// <summary>
        /// Save project and go back to FindProjectHere
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveProjectInfoClose(ProjectInformation model)
        {
            try
            {
                model.Contact = User.Identity.Name;
                await _projectRepository.SaveProjectInfoAsync(model);
                string rootFolder = model.ProjNumber.Substring(0, 2);
                rootFolder = rootFolder.Replace(rootFolder, "20" + rootFolder);
                string subRootFolder = model.ProjNumber.Substring(2, 2);
                //bool test = await DoesFolderExist(rootFolder + "/" + subRootFolder + "/" + sample);
                //if(!test)
                CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Addenda");
                CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Bid Results");
                CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/PHL");
                CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Plans");
                CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Specs");
            }
            catch (Exception Ex) { }
            ProjectInformation model1 = new();
            List<string> objList = new List<string>();
            objList.Add("New Construction");
            objList.Add("Remodel");
            objList.Add("Addition");
            model1.ProjScopeList = objList;

            return RedirectToAction("FindProjectHere", "Member");
        }

        /// <summary>
        /// Save project and go back to EditProjectInfo
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> SaveProjectInfoCopy(ProjectInformation model)
        {
            int Id = 0;
            try
            {
                model.Contact = User.Identity.Name;
                Id = await _projectRepository.SaveProjectInfoAsync(model);
                string rootFolder = model.ProjNumber.Substring(0, 2);
                rootFolder = rootFolder.Replace(rootFolder, "20" + rootFolder);
                string subRootFolder = model.ProjNumber.Substring(2, 2);
                //bool test = await DoesFolderExist(rootFolder + "/" + subRootFolder + "/" + sample);
                //if(!test)
                CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Addenda");
                CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Bid Results");
                CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/PHL");
                CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Plans");
                CreateFolder(rootFolder + "/" + subRootFolder + "/" + model.ProjNumber + "/Specs");
            }
            catch (Exception Ex) { }
            ProjectInformation model1 = new();
            List<string> objList = new List<string>();
            objList.Add("New Construction");
            objList.Add("Remodel");
            objList.Add("Addition");
            model.ProjScopeList = objList;
            return RedirectToAction("EditProjectInfo", new { id = Id, chk = "Copy" });
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
        /// For delete user member from member's memberprofile(User Management section) screen/page.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResult> DeleteUserManagement(int id)
        {
            return Json(await _staffRepository.DeleteUserManagementAsync(id));
        }

        /// <summary>
        /// For view order doc.
        /// </summary>
        /// <param name="OrderId"></param>
        /// <returns></returns>
        public async Task<JsonResult> ViewOrderDoc(int OrderId)
        {
            return Json(await _staffRepository.GetViewOrderDocAsync(OrderId));
        }

        /// <summary>
        /// For edit save search post method.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResult> EditSavedSearch(int id)
        {
            SearchViewModel searchView = await _membershipRepository.GoToSavedSearch(id);
            return Json(searchView);
        }

        /// <summary>
        /// For delete save search post method.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<JsonResult> DeleteSavedSearch(int id)
        {
            return Json(await _membershipRepository.DeleteSaveSearchAsync(id));
        }

        /// <summary>
        /// For going to accessdenied screen/page.
        /// </summary>
        /// <returns></returns>
        public IActionResult AccessDenied()
        {
            return View();
        }

        //logged in User state details
        public async Task<IActionResult> userMemberStates()
        {
            try
            {
                string usern = User.Identity.Name;
                DisplayLoginInfo info = _commonRepository.GetUserInfo(usern);

                if (info != null)
                {
                    var user = _entityRepository.GetEntities().FirstOrDefault(m => m.Id == info.Id);

                    if (user != null)
                    {
                        var counties = _dbContext.TblMemberTypeCounty
                            .Where(mtc => mtc.MemberType == user.MemberType)
                            .ToList();
                        var stateNames = (from county in _dbContext.TblCounty.ToList()
                                          join mtc in counties on county.CountyId equals mtc.CountyID
                                          select county.State)
                                          .Distinct()
                                          .ToList();

                        if (stateNames != null && stateNames.Any())
                        {
                            return Json(stateNames);
                        }
                        else
                        {
                            return NotFound("No state names found.");
                        }
                    }
                }

                return NotFound("User information not found.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }

        public async Task<IActionResult> GetFilterCount(string projType)
        {
            try
            {
                Dictionary<string, List<TypeCount>> resultDictionary = new Dictionary<string, List<TypeCount>>();
                string usern = User.Identity.Name;
                DisplayLoginInfo info = _commonRepository.GetUserInfo(usern);
                List<TblProject> model = new();
                List<int> ProjectTypeIds = new();
                List<int> ProjectSubTypeIds = new();

                if (info != null)
                {
                    if (HttpContext.Session.GetString("AppliedFilters") != null)
                    {
                        var appliedfilters = HttpContext.Session.GetString("AppliedFilters");
                        if (appliedfilters != null)
                        {
                            var data = JsonConvert.DeserializeObject<SearchViewModel>(appliedfilters);
                            model = await _membershipRepository.GetFilteredProjectsData(null);
                            if (data.ProjectTypeIds != null && data.ProjectTypeIds.Count > 0)
                            {
                                ProjectTypeIds = data.ProjectTypeIds.Select(id => Convert.ToInt32(id)).ToList();
                                //if (data.ProjectSubTypeIds != null && data.ProjectSubTypeIds.Count > 0)
                                if (false)
                                {
                                    ProjectSubTypeIds = data.ProjectSubTypeIds.Select(id => Convert.ToInt32(id)).ToList();
                                    var scopeCounts = model.Where(m => !string.IsNullOrEmpty(m.ProjScope) && (ProjectSubTypeIds.Any() && ProjectSubTypeIds.Contains((int)m.ProjSubTypeId)))
                                        .SelectMany(m => m.ProjScope.Split(','))
                                        .Where(scope => !string.IsNullOrWhiteSpace(scope))
                                        .GroupBy(scope => scope.Trim())
                                        .Select(g => new TypeCount
                                        {
                                            TypeId = g.Key.ToString(),
                                            Type = g.Key,
                                            ProjectCount = g.Count().ToString()
                                        }).ToList();

                                    resultDictionary["Scope"] = scopeCounts;
                                }
                                else
                                {
                                    //    var scopeCounts = model
                                    //        .Where(m => !string.IsNullOrEmpty(m.ProjScope) && (ProjectTypeIds.Any() && ProjectTypeIds.Contains((int)m.ProjTypeId)))
                                    //        .SelectMany(m => m.ProjScope.Split(','))
                                    //        .Where(scope => !string.IsNullOrWhiteSpace(scope))
                                    //        .GroupBy(scope => scope.Trim())
                                    //        .Select(g => new TypeCount
                                    //        {
                                    //            TypeId = g.Key.ToString(),
                                    //            Type = g.Key,
                                    //            ProjectCount = g.Count().ToString()
                                    //        })
                                    //.ToList();
                                    var scopeCounts = model.Where(m => !string.IsNullOrEmpty(m.ProjScope))
                                        .SelectMany(m => m.ProjScope.Split(','))
                                        .Where(scope => !string.IsNullOrWhiteSpace(scope))
                                        .GroupBy(scope => scope.Trim())
                                        .Select(g => new TypeCount
                                        {
                                            TypeId = g.Key.ToString(),
                                            Type = g.Key,
                                            ProjectCount = g.Count().ToString()
                                        })
                                        .ToList();

                                    resultDictionary["Scope"] = scopeCounts;
                                }
                            }
                            else
                            {
                                var scopeCounts = model.Where(m => !string.IsNullOrEmpty(m.ProjScope))
                                    .SelectMany(m => m.ProjScope.Split(','))
                                    .Where(scope => !string.IsNullOrWhiteSpace(scope))
                                    .GroupBy(scope => scope.Trim())
                                    .Select(g => new TypeCount
                                    {
                                        TypeId = g.Key.ToString(),
                                        Type = g.Key,
                                        ProjectCount = g.Count().ToString()
                                    })
                                    .ToList();

                                resultDictionary["Scope"] = scopeCounts;
                            }
                        }
                    }
                    else
                    {
                        model = await _membershipRepository.GetFilteredProjectsData(null);

                        var scopeCounts = model.Where(m => !string.IsNullOrEmpty(m.ProjScope))
                            .SelectMany(m => m.ProjScope.Split(','))
                            .Where(scope => !string.IsNullOrWhiteSpace(scope))
                            .GroupBy(scope => scope.Trim())
                            .Select(g => new TypeCount
                            {
                                TypeId = g.Key.ToString(),
                                Type = g.Key,
                                ProjectCount = g.Count().ToString()
                            })
                            .ToList();

                        resultDictionary["Scope"] = scopeCounts;
                    }
                    //var abc = await _membershipRepository.GetFilteredProjectsAsync(info);
                    if (model != null)
                    {
                        // Group projects by ProjTypeId and count them
                        var typeCounts = model.Where(m => m.ProjTypeId != null && m.ProjTypeId != 0)
                            .GroupBy(p => p.ProjTypeId)
                            .Select(g => new TypeCount
                            {
                                TypeId = g.Key.ToString(),
                                Type = _dbContext.TblProjType.FirstOrDefault(m => m.ProjTypeId == g.Key)?.ProjType ?? "Unknown",
                                ProjectCount = g.Count().ToString()
                            })
                            .ToList();
                        resultDictionary["Type"] = typeCounts;

                        var subtypeCounts = model.Where(m => m.ProjSubTypeId != null && m.ProjSubTypeId != 0 && ProjectTypeIds.Contains((int)m.ProjTypeId))
                         .GroupBy(p => p.ProjSubTypeId)
                         .Select(g => new TypeCount
                         {
                             TypeId = g.Key.ToString(),
                             Type = _dbContext.TblProjSubType.FirstOrDefault(m => m.ProjSubTypeID == g.Key)?.ProjSubType ?? "Unknown",
                             ProjectCount = g.Count().ToString()
                         })
                         .ToList();

                        resultDictionary["SubType"] = subtypeCounts;

                        List<string> rangeOptions = new List<string>
                        {
                            "0-100000",
                            "100001-500000",
                            "500001-1000000",
                            "1000001-5000000",
                            "5000001-10000000",
                            "10000001-50000000",
                            "50000000"
                        };

                        var estCount = GetProjectCountForOption(model, rangeOptions);
                        resultDictionary["estCost"] = estCount;

                        var wagesCounts = new List<TypeCount>
                        {
                            new TypeCount
                            {
                                TypeId="PrevailingWagesFlag",
                                Type = "Prevailing Wages",
                                ProjectCount = model.Count(m => m.PrevailingWage != null && m.PrevailingWage == true).ToString()
                            }
                        };

                        resultDictionary["Wages"] = wagesCounts;

                        var stateCounts = model.Where(m => !string.IsNullOrEmpty(m.LocState))
                            .GroupBy(p => p.LocState)
                            .Select(g => new TypeCount
                            {
                                TypeId = _dbContext.TblState.FirstOrDefault(m => m.State == g.Key)?.StateId.ToString() ?? string.Empty,
                                Type = g.Key,
                                ProjectCount = g.Count().ToString()
                            })
                            .ToList();

                        resultDictionary["State"] = stateCounts;

                        var jsonResult = new JsonResult(resultDictionary);
                        return jsonResult;
                    }
                }

                var emptyTypeCounts = new List<TypeCount>();
                var emptyJsonResult = new JsonResult(emptyTypeCounts);
                return emptyJsonResult;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return StatusCode(500, "Internal Server Error");
            }
        }

        public List<TypeCount> GetProjectCountForOption(List<TblProject> response, List<string> options)
        {
            List<TypeCount> typeCounts = new List<TypeCount>();
            List<TblProject> responseFilterObj = new();

            foreach (var option in options)
            {
                if (option != null && option != "0")
                {
                    responseFilterObj = new();
                    int minRange = 0;
                    int maxRange = 0;
                    if (option.Contains('-'))
                    {
                        string[] costArr = option.Split('-');
                        int.TryParse(costArr[0], out minRange);
                        int.TryParse(costArr[1], out maxRange);
                    }
                    else
                    {
                        minRange = int.Parse(option);
                        maxRange = int.MaxValue;
                    }
                    if (response != null && response.Count > 0)
                    {
                        foreach (var item in response)
                        {
                            int mnEst = 0;
                            int mxEst = 0;
                            bool Added = false;
                            if (!string.IsNullOrEmpty(item.EstCost) && item.EstCost.Contains('-'))
                            {
                                string[] costArr = item.EstCost.Split('-');
                                int.TryParse(costArr[0], out mnEst);
                                int.TryParse(costArr[1], out mxEst);
                                if (mnEst > 0 || mxEst > 0)
                                {
                                    if (!(mnEst >= maxRange) && !(mxEst <= minRange))
                                    {
                                        if (mnEst >= minRange || maxRange >= mxEst)
                                        {
                                            responseFilterObj.Add(item);
                                            Added = true;
                                        }
                                    }
                                }
                            }
                            else if (!string.IsNullOrEmpty(item.EstCost))
                            {
                                int.TryParse(item.EstCost, out mnEst);
                                if (mnEst > 0)
                                {
                                    if (mnEst >= minRange && maxRange >= mnEst)
                                    {
                                        responseFilterObj.Add(item);
                                        Added = true;
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(item.EstCost2) && item.EstCost2.Contains('-') && Added == false)
                            {
                                string[] costArr = item.EstCost2.Split('-');
                                int.TryParse(costArr[0], out mnEst);
                                int.TryParse(costArr[0], out mxEst);
                                if (mnEst > 0 || mxEst > 0)
                                {
                                    if (!(mnEst >= maxRange) && !(mxEst <= minRange))
                                    {
                                        if (mnEst >= minRange || maxRange >= mxEst)
                                        {
                                            responseFilterObj.Add(item);
                                            Added = true;
                                        }
                                    }
                                }
                            }
                            else if (!string.IsNullOrEmpty(item.EstCost2) && Added == false)
                            {
                                int.TryParse(item.EstCost2, out mnEst);
                                if (mnEst > 0)
                                {
                                    if (mnEst >= minRange && maxRange >= mnEst)
                                    {
                                        responseFilterObj.Add(item);
                                        Added = true;
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(item.EstCost3) && item.EstCost3.Contains('-') && Added == false)
                            {
                                string[] costArr = item.EstCost3.Split('-');
                                int.TryParse(costArr[0], out mnEst);
                                int.TryParse(costArr[0], out mxEst);
                                if (mnEst > 0 || mxEst > 0)
                                {
                                    if (!(mnEst >= maxRange) && !(mxEst <= minRange))
                                    {
                                        if (mnEst >= minRange || maxRange >= mxEst)
                                        {
                                            responseFilterObj.Add(item);
                                            Added = true;
                                        }
                                    }
                                }
                            }
                            else if (!string.IsNullOrEmpty(item.EstCost3) && Added == false)
                            {
                                int.TryParse(item.EstCost3, out mnEst);
                                if (mnEst > 0)
                                {
                                    if (mnEst >= minRange && maxRange >= mnEst)
                                    {
                                        responseFilterObj.Add(item);
                                        Added = true;
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(item.EstCost4) && item.EstCost4.Contains('-') && Added == false)
                            {
                                string[] costArr = item.EstCost4.Split('-');
                                int.TryParse(costArr[0], out mnEst);
                                int.TryParse(costArr[0], out mxEst);
                                if (mnEst > 0 || mxEst > 0)
                                {
                                    if (!(mnEst >= maxRange) && !(mxEst <= minRange))
                                    {
                                        if (mnEst >= minRange || maxRange >= mxEst)
                                        {
                                            responseFilterObj.Add(item);
                                            Added = true;
                                        }
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(item.EstCost4) && Added == false)
                            {
                                int.TryParse(item.EstCost4, out mnEst);
                                if (mnEst > 0)
                                {
                                    if (mnEst >= minRange && maxRange >= mnEst)
                                    {
                                        responseFilterObj.Add(item);
                                        Added = true;
                                    }
                                }
                            }
                            if (!string.IsNullOrEmpty(item.EstCost5) && item.EstCost5.Contains('-') && Added == false)
                            {
                                string[] costArr = item.EstCost5.Split('-');
                                int.TryParse(costArr[0], out mnEst);
                                int.TryParse(costArr[0], out mxEst);
                                if (mnEst > 0 || mxEst > 0)
                                {
                                    if (!(mnEst >= maxRange) && !(mxEst <= minRange))
                                    {
                                        if (mnEst >= minRange || maxRange >= mxEst)
                                        {
                                            responseFilterObj.Add(item);
                                            Added = true;
                                        }
                                    }
                                }
                            }
                            else if (!string.IsNullOrEmpty(item.EstCost5) && Added == false)
                            {
                                int.TryParse(item.EstCost5, out mnEst);
                                if (mnEst > 0)
                                {
                                    if (mnEst >= minRange && maxRange >= mnEst)
                                    {
                                        responseFilterObj.Add(item);
                                        Added = true;
                                    }
                                }
                            }
                        }
                    }
                    if (responseFilterObj.Count > 0)
                    {
                        typeCounts.Add(new TypeCount
                        {
                            TypeId = option,
                            Type = GetRangeDisplayText(option),
                            ProjectCount = responseFilterObj.Count.ToString()
                        });
                    }
                }
            }
            return typeCounts;
        }

        [HttpPost]
        public IActionResult IsSearchNameAlreadySaved(string searchName, int id)
        {
            bool isSaved = _dbContext.TblAutoSearch.Any(s => s.Name == searchName && s.Id != id);
            return Json(new { isSaved });
        }

        private string GetRangeDisplayText(string option)
        {
            switch (option)
            {
                case "0-100000":
                    return "Less than 100,000";

                case "100001-500000":
                    return "100,001-500,000";

                case "500001-1000000":
                    return "500,001-1,000,000";

                case "1000001-5000000":
                    return "1,000,001-5,000,000";

                case "5000001-10000000":
                    return "5,000,001-10,000,000";

                case "10000001-50000000":
                    return "Greater than 10,000,000";

                case "50000000":
                    return "Greater than 50,000,000";

                default:
                    return option; // Default to the original option if not matched
            }
        }

        public async Task<IActionResult> getMembershipdata(int id)
        {
            var data = await _dbContext.tblPaymentCardDetail.FirstOrDefaultAsync(m => m.Id == id);
            return Json(data);
        }

        public async Task<IActionResult> getpricingdata(string plan)
        {
            var data = await _dbContext.tblMemberShipSubPlans.FirstOrDefaultAsync(m => m.SubMemberShipPlanName.Replace(" ", "").ToLower() == plan);
            return Json(data);
        }
    }

    public class TypeCount
    {
        public string? TypeId { get; set; }
        public string? Type { get; set; }
        public string? ProjectCount { get; set; }
    }
}
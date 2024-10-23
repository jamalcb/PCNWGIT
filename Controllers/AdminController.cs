using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PCNW.Data.Repository;
using PCNW.Models.ContractModels;

namespace PCNW.Controllers
{
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ILogger<AdminController> _logger;
        private readonly ICommonRepository _commonRepository;

        public AdminController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, ILogger<AdminController> logger, ICommonRepository commonRepository)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _commonRepository = commonRepository;
        }
        /// <summary>
        /// For going to admin login screen/page
        /// </summary>
        /// <param name="ReturnUrl"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Login(string ReturnUrl)
        {
            LoginViewModel model = new();
            ViewData["returnedUrl"] = ReturnUrl;
            model.ReturnUrl = ReturnUrl;
            var userid = Request.Cookies["UserName"];

            if (!string.IsNullOrEmpty(userid))
            {
                model.Email = userid;

            }
            var pass = Request.Cookies["password"];
            if (!string.IsNullOrEmpty(pass))
            {
                model.getpassword = pass;
            }
            return View(model);
        }
        /// <summary>
        /// For going to admin login post method or verify the login.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Admin/Login")]
        public async Task<IActionResult> VerifyLogin(LoginViewModel model)
        {
            #region Check as Role
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                if (await _userManager.IsInRoleAsync(user, "Staff"))
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
                if (model.RememberMe == true)
                {
                    Response.Cookies.Append("UserName", model.Email, new Microsoft.AspNetCore.Http.CookieOptions
                    {
                        HttpOnly = false,
                        Secure = Convert.ToBoolean(false)
                    });
                }
                if (model.RememberMe == true)
                {
                    Response.Cookies.Append("password", model.Password, new Microsoft.AspNetCore.Http.CookieOptions
                    {
                        HttpOnly = false,
                        Secure = Convert.ToBoolean(false)
                    });
                }

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
                    return RedirectToAction("Dashboard", "Administration");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid Login Attempt");
                model.RedirectFromLogin = String.Format("You have enterd wrong username or password");
            }
            return View("Login", model);
        }
        /// <summary>
        ///  For going to admin login page and logout the session/page.
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
        /// For auto logout page when admin remain inactive in the screen or no activity and save activity record in TblLogActivity.
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
    }
}

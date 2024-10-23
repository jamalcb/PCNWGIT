using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PCNW.Data.Repository;
using PCNW.Helpers;
using PCNW.Models;
using PCNW.Models.ContractModels;
using PCNW.ViewModel;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;
using System.Web;

namespace PCNW.Controllers
{
    public class AccountController : BaseController
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ICommonRepository _commonRepository;
        private readonly ApplicationDbContext _dbContext;
        public AccountController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager, ICommonRepository commonRepository, ApplicationDbContext dbContext)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _commonRepository = commonRepository;
            _dbContext = dbContext;
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Register()
        {
            RegisterViewModel model = new RegisterViewModel();
            return View(model);
        }
        /// <summary>
        /// No use
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new IdentityUser { UserName = model.Email, Email = model.Email };
                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: true);
                    return RedirectToAction("Dashboard", "Member");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View(model);
        }
        /// <summary>
        /// For going to account/member login screen/page
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
        /// For going to account/member login post method or verify the login.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost("Account/Login")]
        public async Task<IActionResult> VerifyLogin(LoginViewModel model)
        {
            #region Check as Role
            if (ViewData["returnedUrl"] != null)
            {
                string logindata = ViewData["returnedUrl"].ToString();
            }
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                //if (await _userManager.IsInRoleAsync(user, "Admin"))
                //{
                //    ModelState.AddModelError("", "Invalid Login Attempt");
                //    model.RedirectFromLogin = String.Format("You have enterd wrong username or password");
                //    return View("Login", model);
                //}
                //if (await _userManager.IsInRoleAsync(user, "Administration"))
                //{
                //    ModelState.AddModelError("", "Invalid Login Attempt");
                //    model.RedirectFromLogin = String.Format("You have enterd wrong username or password");
                //    return View("Login", model);
                //}
                //if (await _userManager.IsInRoleAsync(user, "Staff"))
                //{
                //    ModelState.AddModelError("", "Invalid Login Attempt");
                //    model.RedirectFromLogin = String.Format("You have enterd wrong username or password");
                //    return View("Login", model);
                //}
                #endregion
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    var logInfo = _commonRepository.GetUserInfo(model.Email);
                    if (logInfo != null)
                    {
                        Response.Cookies.Append("loggedinname", logInfo.Name, new Microsoft.AspNetCore.Http.CookieOptions
                        {
                            HttpOnly = false,
                            Secure = Convert.ToBoolean(false)
                        });
                    }
                    if (await _userManager.IsInRoleAsync(user, "Member"))
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
                            Dictionary<string, object> keyValuePairs = new();
                            string ControllerName = string.Empty;
                            string ActionName = string.Empty;
                            string ParameterId = string.Empty;
                            string[] Arr = model.ReturnUrl.Replace("%2F", "/").Split('/');
                            if (Arr.Contains("?"))
                            {
                                string[] Arr1 = Arr[2].Split('?');
                                ControllerName = Arr[1];
                                ActionName = Arr1.Length > 1 ? Arr1[0] : Arr[2];

                                if (Arr1.Length > 1)
                                {
                                    string[] Arr2 = Arr1[1].Split('&');
                                    for (int i = 0; i < Arr2.Length; i++)
                                    {
                                        string[] routes = Arr2[i].Split("=");
                                        if (routes.Length > 1)
                                        {
                                            keyValuePairs.Add(routes[0], HttpUtility.UrlDecode(routes[1]));
                                        }
                                    }
                                }
                                await _commonRepository.GetLogActivityAsync(model);
                                if (keyValuePairs.Count > 0)
                                {
                                    return RedirectToAction(ActionName, ControllerName, keyValuePairs);
                                }
                                return Redirect(model.ReturnUrl);
                            }
                            else
                            {
                                TempData["Email"] = model.Email;
                                ControllerName = Arr[1];
                                ActionName = Arr[2];
                                await _commonRepository.GetLogActivityAsync(model);
                                return RedirectToAction(ActionName, ControllerName);
                            }
                        }
                        else
                        {
                            TempData["Email"] = model.Email;
                            await _commonRepository.GetLogActivityAsync(model);
                            DisplayLoginInfo res = _commonRepository.GetUserInfo(model.Email);
                            TempData["InActive"] = null;
                            if (res.InActive == true)
                            {
                                TempData["InActive"] = "Yes";
                                return RedirectToAction("MemberProfile", "Member");
                            }
                            else
                            {
                                return RedirectToAction("FindProjectHere", "Member");
                            }

                        }
                    }
                    else if (await _userManager.IsInRoleAsync(user, "Staff"))
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
                    else if (await _userManager.IsInRoleAsync(user, "Admin"))
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
                }
                else
                {
                    ModelState.AddModelError("", "Invalid Login Attempt");
                    model.RedirectFromLogin = String.Format("You have enterd wrong username or password");
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
        /// No Use
        /// </summary>
        /// <param name="Email"></param>
        /// <param name="Password"></param>
        /// <param name="RememberMe"></param>
        /// <returns></returns>
        [HttpPost("Account/VerifyLogin")]
        public async Task<JsonResult> JsonVerifyLogin(string Email, string Password, bool RememberMe)
        {
            LoginViewModel model = new();
            model.Email = Email;
            model.Password = Password;
            model.RememberMe = RememberMe;
            HttpResponseDetail<dynamic> response = new();
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
                    response.data = "/" + ControllerName + "/" + ActionName;
                }
                else
                    response.data = "/StaffAccount/Dashboard";

                response.success = true;
            }
            else
            {
                response.success = false;
                response.statusMessage = "Invalid Login Attempt. You have enterd wrong username or password";
            }
            return Json(response);
        }
        /// <summary>
        /// For going to user login page and logout the session/page.
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
            return RedirectToAction("Index", "Home");
        }
        /// <summary>
        /// For auto logout page when user remain inactive in the screen and save activity record in TblLogActivity.
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
        /// For going to forgetpassword screen/page.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Forgetpassword()
        {
            return View();
        }
        /// <summary>
        /// For going to forgetpassword post method and send a mail for this.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost("Account/Forgetpassword")]
        public async Task<IActionResult> Forgetpassword([Required] string email)
        {
            if (!ModelState.IsValid)
                return View(email);
            var User = await _userManager.FindByEmailAsync(email);
            if (User == null)
            {
                TempData["ErrorMessage"] = "This email is not valid";
                return View();
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(User);
            var link = Url.Action("Resetpassword", "Account", new { token, email = User.Email }, Request.Scheme);

            bool emailResponse = false;
            if (!string.IsNullOrEmpty(link))
            {
                EmailMessageSender emailHelper = new EmailMessageSender();
                emailResponse = emailHelper.SendEmailPasswordReset(User.Email, link);
            }

            if (emailResponse)
            {
                //return RedirectToAction("ForgotPasswordConfirmation");
                TempData["SuccessMessage"] = "The email has been sent. Please check your email to reset your password.";
                return View();
            }

            else
            {
                // log email failed 
            }
            return View(email);
        }
        /// <summary>
        /// For going to forgetpassword post method from memberprofile's user management section and send a mail for this.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost("Account/ProfileForgetpassword")]
        public async Task<IActionResult> ProfileForgetpassword([Required] string email)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Forgetpassword", "Account");
            var User = await _userManager.FindByEmailAsync(email);
            if (User == null)
            {
                TempData["ErrorMessage"] = "This email is not valid";
                return RedirectToAction("Forgetpassword", "Account");
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(User);
            var link = Url.Action("Resetpassword", "Account", new { token, email = User.Email }, Request.Scheme);

            bool emailResponse = false;
            if (!string.IsNullOrEmpty(link))
            {
                EmailMessageSender emailHelper = new EmailMessageSender();
                emailResponse = emailHelper.SendEmailPasswordReset(User.Email, link);
            }

            if (emailResponse)
            {
                //return RedirectToAction("ForgotPasswordConfirmation");
                TempData["SuccessMessage"] = "The email has been sent. Please check your email to reset your password.";
                return RedirectToAction("MemberProfile", "Member");
            }
            else
            {
                TempData["SuccessMessage"] = "Something went wrong";
                return RedirectToAction("MemberProfile", "Member");
            }
        }
        /// <summary>
        /// For going to forgetpassword post method from staff memberprofile's user management section and send a mail for this.
        /// </summary>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpPost("Account/StaffForgetpassword")]
        public async Task<IActionResult> StaffForgetpassword([Required] string email)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Forgetpassword", "Account");
            var User = await _userManager.FindByEmailAsync(email);
            if (User == null)
            {
                TempData["ErrorMessage"] = "This email is not valid";
                return RedirectToAction("Forgetpassword", "Account");
            }
            var token = await _userManager.GeneratePasswordResetTokenAsync(User);
            var link = Url.Action("Resetpassword", "Account", new { token, email = User.Email }, Request.Scheme);

            bool emailResponse = false;
            if (!string.IsNullOrEmpty(link))
            {
                EmailMessageSender emailHelper = new EmailMessageSender();
                emailResponse = emailHelper.SendEmailPasswordReset(User.Email, link);
            }

            if (emailResponse)
            {
                //return RedirectToAction("ForgotPasswordConfirmation");
                TempData["SuccessMessage"] = "The email has been sent. Please check your email to reset your password.";
                var maincontact = _dbContext.TblContacts.SingleOrDefault(m => m.Email == email);
                var GetId = _dbContext.TblContacts.FirstOrDefault(x => x.Id == maincontact.Id && x.MainContact == true);
                return RedirectToAction("MemberProfile", "StaffAccount", new { id = GetId.Id });
            }
            else
            {
                TempData["SuccessMessage"] = "Something went wrong";
                var maincontact = await _dbContext.TblContacts.SingleOrDefaultAsync(x => x.Email == email);
                var GetId = await _dbContext.TblContacts.FirstOrDefaultAsync(x => x.Id == maincontact.Id && x.MainContact == true);
                return RedirectToAction("MemberProfile", "StaffAccount", new { id = GetId.Id });
            }
        }
        /// <summary>
        /// For going to resetpassword screen/page.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        [HttpGet]
        public IActionResult Resetpassword(string token, string email)
        {
            var model = new Resetpassword { Token = token, Email = email };
            return View(model);
        }
        /// <summary>
        /// For going to resetpassword post method or verify/change password.
        /// </summary>
        /// <param name="resetpassword"></param>
        /// <returns></returns>
        [HttpPost("Account/Resetpassword")]
        public async Task<IActionResult> VerifyResetPassword(Resetpassword resetpassword)
        {
            if (!ModelState.IsValid)
                return View(resetpassword);

            var user = await _userManager.FindByEmailAsync(resetpassword.Email);
            if (user == null)
                RedirectToAction("Resetpassword");

            if (user != null)
            {
                var resetPassResult = await _userManager.ResetPasswordAsync(user, resetpassword.Token, resetpassword.Password);
                if (!resetPassResult.Succeeded)
                {
                    foreach (var error in resetPassResult.Errors)
                        ModelState.AddModelError(error.Code, error.Description);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var PasswordUpdate = await _commonRepository.PasswordUpdateAsync(resetpassword);
                    TempData["SuccessMessage"] = "Password updated successfully";
                }
            }
            return RedirectToAction("Resetpassword");
        }
    }
}

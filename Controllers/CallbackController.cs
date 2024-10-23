using Intuit.Ipp.OAuth2PlatformClient;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace PCNW.Controllers
{
    public class CallbackController : BaseController
    {
        public string clientid = null;
        public string clientsecret = null;
        public string redirectUrl = null;
        public string environment = null;

        private readonly ILogger<AppController> _logger;
        private readonly IConfiguration _configuration;
        //private readonly OAuth2Client auth2Client1;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly OAuth2Client auth2Client;

        public CallbackController(ILogger<AppController> logger, IConfiguration configuration, UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _logger = logger;

            _configuration = configuration;
            //auth2Client1 = auth2Client;
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            //QuickbookInfo quickbookInfo = _configuration.GetValue<QuickbookInfo>("QuickbookInfo");
            //clientid = quickbookInfo.clientid;
            //clientsecret = quickbookInfo.clientsecret;
            //         redirectUrl = quickbookInfo.redirectUrl;
            //environment = quickbookInfo.environment;
            var clientId = _configuration.GetSection("AppSettings")["ClientId"];
            var clientSecret = _configuration.GetSection("AppSettings")["ClientSecret"];
            var redirectUrl = _configuration.GetSection("AppSettings")["RedirectUrl"];



            auth2Client = new OAuth2Client(clientId, clientSecret, redirectUrl, "sandbox");
            //this.auth2Client = new OAuth2Client("ABKkJCWJShTwHpbT3JG6NQS79jkQV9iAVitco7WMSNGoWrSupF", "vWB9s6WleO0aUxteu6cpM7r0KRLS99PydrB8A5FI", "http://qb.contractorplancenter.com/Callback", "sandbox");
        }
        /// <summary>
        /// Code and realmid/company id recieved on Index page after redirect is complete from Authorization url
        /// </summary>
        public async Task<IActionResult> Index([FromQuery] string state, string code, string realmId, string error)
        {
            if (TempData["Model"] != null)
            {
                int toDel = Convert.ToInt32(TempData["Model"]);
                TempData["Model"] = toDel;
            }//Sync the state info and update if it is not the same
            //var state = HttpContext.Request.QueryString["state"];
            if (state.Equals(auth2Client.CSRFToken, StringComparison.Ordinal))
            {
                ViewBag.State = state + " (valid)";
            }
            else
            {
                ViewBag.State = state + " (invalid)";
            }

            //string code = Request.QueryString["code"] ?? "none";
            //string realmId = Request.QueryString["realmId"] ?? "none";
            // List<Claim> Claim =
            string A = await GetAuthTokensAsync(code, realmId);

            //ViewBag.Error = Request.QueryString["error"] ?? "none";
            ViewBag.Error = error;
            //TempData["Token"] = Claim.ToList();
            return RedirectToAction("ApiCallService", "App", new { Token = A });
        }

        /// <summary>
        /// Exchange Auth code with Auth Access and Refresh tokens and add them to Claim list
        /// </summary>
        private async Task<string> GetAuthTokensAsync(string code, string realmId)
        {
            // if (realmId != null)
            //{
            HttpContext.Session.SetString("realmId", Convert.ToString("4620816365247983250"));
            //}

            //Request.GetOwinContext().Authentication.SignOut("TempState");

            var tokenResponse = await auth2Client.GetBearerTokenAsync(code);

            var claims = new List<Claim>();

            if (HttpContext.Session.GetString("realmId") != null)
            {
                realmId = Convert.ToString(HttpContext.Session.GetString("realmId"));
                claims.Add(new Claim("realmId", realmId));
            }

            if (!string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
            {
                claims.Add(new Claim("access_token", tokenResponse.AccessToken));
                claims.Add(new Claim("access_token_expires_at", DateTime.Now.AddSeconds(tokenResponse.AccessTokenExpiresIn).ToString()));
            }

            if (!string.IsNullOrWhiteSpace(tokenResponse.RefreshToken))
            {
                claims.Add(new Claim("refresh_token", tokenResponse.RefreshToken));
                claims.Add(new Claim("refresh_token_expires_at", DateTime.Now.AddSeconds(tokenResponse.RefreshTokenExpiresIn).ToString()));
            }

            var id = new ClaimsIdentity(claims, "Cookies");
            //Request.GetOwinContext().Authentication.SignIn(id);
            //Request.GetOwinContext().Authentication.SignIn(id);
            //	var user = new IdentityUser {UserName="codingbrains33@gmail.com",  };
            //await _signInManager.SignInAsync(user, false);
            string A = claims[1].ToString();
            return A;
        }
        private async Task<List<Claim>> GetAuthTokens(string code, string realmId)
        {
            if (realmId != null)
            {
                HttpContext.Session.SetString("realmId", Convert.ToString(realmId));
            }

            //Request.GetOwinContext().Authentication.SignOut("TempState");

            var tokenResponse = await auth2Client.GetBearerTokenAsync(code);

            var claims = new List<Claim>();

            if (HttpContext.Session.GetString("realmId") != null)
            {
                realmId = Convert.ToString(HttpContext.Session.GetString("realmId"));
                claims.Add(new Claim("realmId", realmId));
            }

            if (!string.IsNullOrWhiteSpace(tokenResponse.AccessToken))
            {
                claims.Add(new Claim("access_token", tokenResponse.AccessToken));
                claims.Add(new Claim("access_token_expires_at", DateTime.Now.AddSeconds(tokenResponse.AccessTokenExpiresIn).ToString()));
            }

            if (!string.IsNullOrWhiteSpace(tokenResponse.RefreshToken))
            {
                claims.Add(new Claim("refresh_token", tokenResponse.RefreshToken));
                claims.Add(new Claim("refresh_token_expires_at", DateTime.Now.AddSeconds(tokenResponse.RefreshTokenExpiresIn).ToString()));
            }

            var id = new ClaimsIdentity(claims, "Cookies");
            //Request.GetOwinContext().Authentication.SignIn(id);
            //Request.GetOwinContext().Authentication.SignIn(id);
            var user = new IdentityUser { UserName = "codingbrains33@gmail.com", };
            await _signInManager.SignInAsync(user, false);
            return claims;
        }
    }
}
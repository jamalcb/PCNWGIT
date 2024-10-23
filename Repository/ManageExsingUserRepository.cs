using Microsoft.AspNetCore.Identity;
using PCNW.Data.ContractResponse;
using PCNW.ExtentionMethods;
using PCNW.Helpers;
using PCNW.Models;

namespace PCNW.Repository
{
    public class ManageExsingUserRepository : IManageExsingUserRepository
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly ApplicationDbContext dbContext;
        private readonly ILogger<ManageExsingUserRepository> logger;

        public ManageExsingUserRepository(UserManager<IdentityUser> _userManager, SignInManager<IdentityUser> _signInManager, ApplicationDbContext dbContext, ILogger<ManageExsingUserRepository> logger)
        {
            userManager = _userManager;
            signInManager = _signInManager;
            this.dbContext = dbContext;
            this.logger = logger;
        }
        public async Task<dynamic> ManageExistingUserAsync()
        {
            HttpResponseDetail<dynamic> response = new();
            List<dynamic> userinfo = new();
            try
            {
                var users = await GeneralMethods.DynamicListFromSqlAsync(dbContext, "sp_pcnw_GetUserInformation", new Dictionary<string, object>());
                if (users != null)
                {
                    QuickUserInformation quickUserInformation = new();
                    object UserName;
                    object Email;
                    int counter = 1;
                    foreach (var model in users)
                    {
                        UserName = ((object)model.UID ?? DBNull.Value);
                        Email = ((object)model.email ?? DBNull.Value);
                        if ((UserName != DBNull.Value) && (Email != DBNull.Value))
                        {
                            if ((UserName != null) && (Email != null))
                            {
                                //if ((!string.IsNullOrEmpty(model.UID)) && (!string.IsNullOrEmpty(model.email)))
                                if ((UserName != "") && (Email != ""))
                                {
                                    var user = new IdentityUser { UserName = model.UID, Email = model.email };
                                    var userExists = await userManager.FindByEmailAsync(user.Email);
                                    var userExistsUserName = await userManager.FindByNameAsync(user.UserName);
                                    if (userExists == null && userExistsUserName == null)
                                    {
                                        var result = await userManager.CreateAsync(user, model.Password);
                                        if (result.Succeeded)
                                        {
                                            quickUserInformation.username = model.Contact;
                                            quickUserInformation.email = model.email;
                                            quickUserInformation.password = model.Password;
                                            quickUserInformation.userid = model.UID;
                                            userinfo.Add(quickUserInformation);
                                        }
                                    }
                                }
                            }
                        }
                        counter++;
                    }
                }
                response.data = userinfo;
                response.success = true;
                response.statusCode = "200";
                response.statusMessage = "User manage successfully.";
            }
            catch (Exception ex)
            {
                response.data = userinfo;
                response.success = false;
                response.statusCode = GeneralMethods.GetErrorCode(ex);
                response.statusMessage = ex.Message;
                logger.LogInformation(ex.Message);
            }
            return response;
        }
    }
}

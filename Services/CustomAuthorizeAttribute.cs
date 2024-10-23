using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace PCNW.Services
{
    public class CustomAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly string _adminLoginUrl = "/Admin/Login";
        private readonly string _memberLoginUrl = "/Account/Login";
        private readonly string _staffLoginUrl = "/staffaccount/login";
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (!user.Identity.IsAuthenticated)
            {
                context.Result = new RedirectResult("/Account/Login");
            }
            else if (user.IsInRole("Admin"))
            {
                context.Result = new RedirectResult(_adminLoginUrl);
            }
            else if (user.IsInRole("Member"))
            {
                context.Result = new RedirectResult(_memberLoginUrl);
            }
            else if (user.IsInRole("Staff"))
            {
                context.Result = new RedirectResult(_staffLoginUrl);
            }
            else
            {
                context.Result = new RedirectResult("/Home/Index");
            }
        }
    }


}

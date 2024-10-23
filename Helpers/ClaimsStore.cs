using System.Security.Claims;

namespace PCNW.Helpers
{
    public static class ClaimsStore
    {
        public static List<Claim> AllClaims = new List<Claim>()
        {
            new Claim("View Role", "View Role"),
            new Claim("Craete Role", "Craete Role"),
            new Claim("Edit Role", "Edit Role"),
            new Claim("Delete Role", "Delete Role")
        };
    }
}
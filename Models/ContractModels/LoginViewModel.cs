using System.ComponentModel.DataAnnotations;

namespace PCNW.Models.ContractModels
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
        public string ReturnUrl { get; set; }
        public string getpassword { get; set; }

        public string? RedirectFromLogin { get; set; }

    }
}

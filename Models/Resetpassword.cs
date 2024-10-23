using System.ComponentModel.DataAnnotations;
namespace PCNW.Models
{
    public class Resetpassword
    {
        [Required]
        public string Password { get; set; }
        [Required]
        [Compare("Password", ErrorMessage = "The password and confirm password do not match.")]
        public string ConfirmPassword { get; set; }

        public string Email { get; set; }
        public string Token { get; set; }
    }
}

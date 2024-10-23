using Microsoft.Build.Framework;

namespace PCNW.Models.ContractModels
{
    public class CreateRoleViewModel
    {
        [Required]
        public string RoleName { get; set; }
    }
}

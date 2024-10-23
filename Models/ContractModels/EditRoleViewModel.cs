using System.ComponentModel.DataAnnotations;

namespace PCNW.Models.ContractModels
{
    public class EditRoleViewModel
    {
        public EditRoleViewModel()
        {
            Users = new List<string>();

        }
        public string Id { get; set; }
        [Required]
        [StringLength(150, ErrorMessage = "RoleName should not be blank")]
        public string RoleName { get; set; }
        public List<string> Users { get; set; }
    }
}

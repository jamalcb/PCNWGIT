using System.ComponentModel.DataAnnotations;

namespace PCNW.Models.ContractModels
{
    public class MemberSignUpViewModel
    {
        [Key]
        public int Id { get; set; }
        public string? Email { get; set; }
    }
}

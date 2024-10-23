using System.ComponentModel.DataAnnotations;

namespace PCNW.Models.ContractModels
{
    public class MembershipExpireViewModel
    {
        [Key]
        public int Id { get; set; }
        public string? Email { get; set; }
        public int? Timer { get; set; }
    }
}

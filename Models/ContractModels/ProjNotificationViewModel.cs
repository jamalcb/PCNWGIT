using System.ComponentModel.DataAnnotations;

namespace PCNW.Models.ContractModels
{
    public class ProjNotificationViewModel
    {
        [Key]
        public int Id { get; set; }
        public string? Email { get; set; }
    }
}

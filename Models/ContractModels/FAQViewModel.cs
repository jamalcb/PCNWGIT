using System.ComponentModel.DataAnnotations;

namespace PCNW.Models.ContractModels
{
    public class FAQViewModel
    {
        [Key]
        public int Id { get; set; }
        public string? Question { get; set; }
        public string? Answer { get; set; }
        public bool? IsActive { get; set; }
    }
}

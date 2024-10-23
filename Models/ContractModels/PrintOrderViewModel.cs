using System.ComponentModel.DataAnnotations;

namespace PCNW.Models.ContractModels
{
    public class PrintOrderViewModel
    {
        [Key]
        public int Id { get; set; }
        public string? Email { get; set; }
    }
}

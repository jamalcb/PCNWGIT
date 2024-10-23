using System.ComponentModel.DataAnnotations;

namespace PCNW.Models.ContractModels
{
    public class CopyCenterViewModel
    {
        [Key]
        public int Id { get; set; }
        public string? CopyCenter { get; set; }
    }
}

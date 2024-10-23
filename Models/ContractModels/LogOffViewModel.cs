using System.ComponentModel.DataAnnotations;

namespace PCNW.Models.ContractModels
{
    public class LogOffViewModel
    {
        [Key]
        public int Id { get; set; }
        public int? LogOff { get; set; }
    }
}

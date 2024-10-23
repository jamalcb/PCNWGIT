using System.ComponentModel.DataAnnotations;

namespace PCNW.Models.ContractModels
{
    public class CareerPostingViewModel
    {
        [Key]
        public int Id { get; set; }
        public string? PositionName { get; set; }
        public string? OpaningNo { get; set; }
        public string? Experience { get; set; }
        public string? Qualification { get; set; }
        public string? ContactPerson { get; set; }
        public string? ContactNumber { get; set; }
        public string? JobDescription { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}

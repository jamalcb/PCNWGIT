using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("CareerPostings")]

    public partial class TblCareerPostings
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

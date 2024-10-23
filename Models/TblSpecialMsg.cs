using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("SpecialMsg")]

    public partial class TblSpecialMsg
    {
        [Key]
        public int Id { get; set; }
        public string? Type { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? SpMessage { get; set; }
        public bool? IsActive { get; set; }
    }
}

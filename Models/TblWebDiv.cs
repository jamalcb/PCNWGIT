using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("WebDiv")]
    public partial class TblWebDiv
    {
        [Key]
        public int WebDivId { get; set; }
        public int? DivNo { get; set; }
        public string? DivDesc { get; set; }
        public string? DivName { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsDeleted { get; set; }
        public int? Priority { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("County")]

    public partial class TblCounty
    {
        [Key]
        public int CountyId { get; set; }
        public string? County { get; set; }
        public string? State { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("PHLType")]

    public partial class TblPHLType
    {
        [Key]
        public int PHLID { get; set; }
        [StringLength(150)]
        public string? PHLType { get; set; }
        public int? SortOrder { get; set; }
        public Nullable<bool> IsActive { get; set; }

    }
}

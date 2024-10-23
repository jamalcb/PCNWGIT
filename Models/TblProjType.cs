using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("ProjType")]

    public partial class TblProjType
    {
        [Key]
        public int ProjTypeId { get; set; }
        public string? ProjType { get; set; }
        public int? SortOrder { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}

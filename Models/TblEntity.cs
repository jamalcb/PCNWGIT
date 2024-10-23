using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("Entity")]

    public partial class TblEntity
    {
        [Key]
        public int EntityID { get; set; }
        [StringLength(255)]
        public string? EntityType { get; set; }
        [StringLength(255)]
        public string? EnityName { get; set; }
        public int? Projid { get; set; }
        public int? ProjNumber { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public int? NameId { get; set; }
        public bool chkIssue { get; set; } = false;
        public int? CompType { get; set; }
    }
}

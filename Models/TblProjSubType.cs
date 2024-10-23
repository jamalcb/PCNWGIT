using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("ProjSubType")]

    public partial class TblProjSubType
    {
        [Key]
        public int ProjSubTypeID { get; set; }
        public string? ProjSubType { get; set; }
        public int? SortOrder { get; set; }
        public int? ProjTypeID { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("Phllog")]

    public partial class TblPhllog
    {
        [Key]
        public int LogId { get; set; }
        public int? ProjConId { get; set; }
        public int? ConId { get; set; }
        public int? ConTypeId { get; set; }
        public int? ProjId { get; set; }
        public int? SortOrder { get; set; }
        public bool? DeleteRec { get; set; }
        public bool? Processed { get; set; }
    }
}

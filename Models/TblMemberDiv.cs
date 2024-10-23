using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("MemberDiv")]
    public partial class TblMemberDiv
    {
        [Key]
        public int MemDivId { get; set; }
        public int? WebDivId { get; set; }
        public int? MemberId { get; set; }
        public bool? IsDeleted { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("MembershipExpire")]

    public partial class TblMembershipExpire
    {
        [Key]
        public int Id { get; set; }
        public string? Email { get; set; }
        public int? Timer { get; set; }
    }
}

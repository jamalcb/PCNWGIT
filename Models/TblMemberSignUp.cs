using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("MemberSignUp")]

    public partial class TblMemberSignUp
    {
        [Key]
        public int Id { get; set; }
        public string? Email { get; set; }
    }
}

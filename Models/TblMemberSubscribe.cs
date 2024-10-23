using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("MemberSubscribe")]

    public partial class TblMemberSubscribe
    {
        [Key]
        public int Id { get; set; }
        public string? UserEmail { get; set; }
        public Nullable<bool> Subscribe { get; set; }
        public DateTime? SubscribeDate { get; set; }
        public DateTime? UnSubscribeDate { get; set; }

    }
}

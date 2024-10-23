using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("ProjOrdChrgsDetails")]

    public class tblProjOrdChrgsDetails
    {
        [Key]
        public int Id { get; set; }
        public string? SizeName { get; set; }
        public string? Size { get; set; }
        public decimal? MemberPrice { get; set; }
        public decimal? NonMemberPrice { get; set; }
        public decimal? ColorMemberPrice { get; set; }
        public decimal? ColorNonMemberPrice { get; set; }
        public bool isActive { get; set; }
    }
}

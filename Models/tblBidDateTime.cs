using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("BidDateTime")]

    public partial class tblBidDateTime
    {
        [Key]
        public int ID { get; set; }
        public int? ProjId { get; set; }
        public DateTime? BidDate { get; set; }
        public string? BidDateTime { get; set; }
        public string? PST { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public int? AddendaId { get; set; }
        public int? PhlId { get; set; }
        public Nullable<bool> Isextensions { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? ExtNum { get; set; }
    }
}

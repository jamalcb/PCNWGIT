using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Data
{
    [Table("PreBidInfo")]
    public class tblPreBidInfo
    {
        [Key]
        public int Id { get; set; }
        public DateTime? PreBidDate { get; set; }
        public string? PreBidTime { get; set; }
        public string? Location { get; set; }
        public string? PST { get; set; }
        public bool Mandatory { get; set; } = false;
        public bool PreBidAnd { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public bool? UndecidedPreBid { get; set; } = false;
        public int? ProjId { get; set; }

    }
}

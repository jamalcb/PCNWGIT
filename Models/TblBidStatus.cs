using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("BidStatus")]

    public partial class TblBidStatus
    {
        [Key]
        public int Id { get; set; }
        public string? Uid { get; set; }
        public int? Projid { get; set; }
        public bool? Bidding { get; set; }
        public bool? Undecided { get; set; }
        public int? PHLType { get; set; }
        public string? PHLNote { get; set; }
        public string? Contact { get; set; }
        public string? Company { get; set; }
        public int CompType { get; set; } = 1;
    }
}

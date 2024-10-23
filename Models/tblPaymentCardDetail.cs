using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("PaymentCardDetail")]

    public class tblPaymentCardDetail
    {
        [Key]
        public int Id { get; set; }
        public string? UserText { get; set; }
        public string? RegionText { get; set; }
        public string? PackageName { get; set; }
        public int? MemberType { get; set; }
        public string? RegionHead { get; set; }
    }
}

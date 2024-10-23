using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("Discount")]

    public class tblDiscount
    {
        [Key]
        public int DiscountId { get; set; }
        public int? DiscountRate { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
        public bool isActive { get; set; }
    }
}

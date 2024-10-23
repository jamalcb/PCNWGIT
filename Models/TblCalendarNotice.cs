using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("CalendarNotice")]

    public partial class TblCalendarNotice
    {
        [Key]
        public int Id { get; set; }
        public int? ConId { get; set; }
        public string? PrebidDt { get; set; }
        public string? BidDt { get; set; }
        public bool? Disable { get; set; }
        public int? ProjId { get; set; }
        public int? MemberId { get; set; }
    }
}

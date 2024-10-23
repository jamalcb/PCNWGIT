using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("DailyInfoHoliday")]

    public partial class TblDailyInfoHoliday
    {
        [Key]
        public int DiholidayId { get; set; }
        public DateTime? HolidayDt { get; set; }
        public string? Holiday { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}

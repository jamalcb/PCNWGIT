using System.ComponentModel.DataAnnotations;

namespace PCNW.Models
{
    public partial class DailyFax
    {
        [Key]
        public string? CompanyName { get; set; }
        public string Fax { get; set; } = null!;
        public string? Uid { get; set; }
    }
}

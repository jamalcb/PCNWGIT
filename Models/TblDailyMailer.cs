using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("DailyMailer")]

    public class TblDailyMailer
    {
        [Key]
        public int Id { get; set; }

        public string? MailerText { get; set; }

        public string? MailerPath { get; set; }
        public bool IsActive { get; set; } = false;

    }
}

using Microsoft.AspNetCore.Html;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("FAQ")]

    public class TblFAQ
    {
        [Key]
        public int Id { get; set; }
        public string? Question { get; set; }
        public string? Answer { get; set; }
        public bool? IsActive { get; set; }
        [NotMapped]
        public HtmlString AnswerRaw { get; set; }
    }
}

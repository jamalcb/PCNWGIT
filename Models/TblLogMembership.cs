using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("LogMembership")]

    public partial class TblLogMembership
    {
        [Key]
        public int Id { get; set; }
        public int? MemId { get; set; }
        public DateTime? ChngDt { get; set; }
        public string? FieldName { get; set; }
        public string? Ip { get; set; }
    }
}

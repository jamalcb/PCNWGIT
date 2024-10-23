using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("MemNotes")]
    public partial class TblMemNote
    {
        [Key]
        public int Id { get; set; }
        public int? MemId { get; set; }
        public DateTime? LogDate { get; set; }
        public string? Note { get; set; }
        public string? Name { get; set; }
        public bool? Flag { get; set; }
        public string? CompType { get; set; }
    }
}

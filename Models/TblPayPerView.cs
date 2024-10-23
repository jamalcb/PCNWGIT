using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("PayPerView")]

    public partial class TblPayPerView
    {
        [Key]
        public int Id { get; set; }
        public int? MemId { get; set; }
        public int? ProjId { get; set; }
        public DateTime? Date { get; set; }
    }
}

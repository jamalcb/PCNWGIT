using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("Iprelation")]

    public partial class TblIprelation
    {
        [Key]
        public int Id { get; set; }
        public string? Ip { get; set; }
        public int? MemId { get; set; }
        public DateTime? LastUsed { get; set; }
        public bool? Show { get; set; }
        public string? Notes { get; set; }
    }
}

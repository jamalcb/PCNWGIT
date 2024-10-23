using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("LocList")]

    public class TblLocList
    {
        [Key]
        public int LocId { get; set; } = 0;
        public string? LocAddr { get; set; } = "";
        public string? LocCity { get; set; } = "";
        public string? LocState { get; set; } = "";
        public string? LocCounty { get; set; } = "";
        public string? LocZip { get; set; } = "";
        public string? LocPhone { get; set; } = "";
        public int? MemId { get; set; } = 0;
    }
}

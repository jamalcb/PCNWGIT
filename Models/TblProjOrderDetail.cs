using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("ProjOrderDetail")]

    public partial class TblProjOrderDetail
    {
        [Key]
        public int ProjOrderId { get; set; }
        public int? OrderId { get; set; }
        public string? FileName { get; set; }
        public int? Pages { get; set; }
        public int? Copies { get; set; }
        public string? Size { get; set; }
        public Decimal? Price { get; set; }
        public string? PrintName { get; set; }
    }
}

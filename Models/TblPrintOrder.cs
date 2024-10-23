using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("PrintOrder")]

    public partial class TblPrintOrder
    {
        [Key]
        public int Id { get; set; }
        public string? Email { get; set; }
    }
}

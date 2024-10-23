using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("PHLUpdate")]

    public partial class TblPHLUpdate
    {
        [Key]
        public int Id { get; set; }
        public string? Email { get; set; }
    }
}

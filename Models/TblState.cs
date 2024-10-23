using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("State")]

    public partial class TblState
    {
        [Key]
        public int StateId { get; set; }
        public string? State { get; set; }
        public bool? CustomSearch { get; set; }
    }
}

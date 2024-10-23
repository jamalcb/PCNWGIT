using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("Title")]

    public partial class TblTitle
    {
        [Key]
        public int WordId { get; set; }
        public string Word { get; set; } = null!;
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("Logoff")]

    public partial class TblLogoff
    {
        [Key]
        public int Id { get; set; }
        public int? LogOff { get; set; }
    }
}

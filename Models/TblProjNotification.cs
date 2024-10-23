using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("ProjNotification")]

    public partial class TblProjNotification
    {
        [Key]
        public int Id { get; set; }
        public string? Email { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("UsedProjNuber")]

    public class tblUsedProjNuber
    {
        [Key]
        public int Id { get; set; }
        public string? ProjNumber { get; set; }
        public Nullable<bool> IsUsed { get; set; }
    }
}

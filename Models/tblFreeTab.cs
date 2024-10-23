using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("FreeTab")]

    public class tblFreeTab
    {
        [Key]
        public int Id { get; set; }
        public bool SetTab { get; set; } = true;
    }
}

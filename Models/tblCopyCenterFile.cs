using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("CopyCenterFile")]

    public class tblCopyCenterFile
    {
        [Key]
        public int Id { get; set; }
        public string? CopyCenter { get; set; }
    }
}

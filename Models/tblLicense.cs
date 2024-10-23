using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("License")]

    public class tblLicense
    {
        [Key]
        public int LicId { get; set; }
        public int MemId { get; set; }
        public string? LicNum { get; set; }
        public string? LicDesc { get; set; }
        public int? LicState { get; set; }
    }
}

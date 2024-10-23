using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("ArchOwner")]
    public partial class TblArchOwner
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Addr1 { get; set; }
        public string? Type1 { get; set; }
        public string? Email { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Zip { get; set; }
        public string? Phone { get; set; }
        public string? Fax { get; set; }
        public int? AotypeId { get; set; }
        public string? Uid { get; set; }
        public string? Pwd { get; set; }
        public string? WebAddress { get; set; }
        public string? Cnote { get; set; }
        public bool Active { get; set; }
        public bool? Converted { get; set; }
    }
}

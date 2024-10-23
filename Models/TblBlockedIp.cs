using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("BlockedIp")]

    public partial class TblBlockedIp
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Ipaddress { get; set; }
        public bool? Disable { get; set; }
        public string? Note { get; set; }
    }
}

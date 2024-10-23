using System.ComponentModel.DataAnnotations;

namespace PCNW.Models
{
    public partial class IpLog
    {
        [Key]
        public int Id { get; set; }
        public string? Uid { get; set; }
        public DateTime? LogDate { get; set; }
        public string? Company { get; set; }
        public int? SessionId { get; set; }
        public string? Screen { get; set; }
        public int? ProjId { get; set; }
        public string? Ip { get; set; }
        public bool? Qev { get; set; }
        public bool? Ost { get; set; }
        public string? Expr1 { get; set; }
        public string? Expr2 { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("LogActivity")]

    public partial class TblLogActivity
    {
        [Key]
        public int Id { get; set; }
        public string? UserName { get; set; }
        public DateTime? LoginTime { get; set; }
        public bool? LoginFlag { get; set; }
        public string? Key { get; set; }
        public DateTime? LastActivity { get; set; }
        public string? Activity { get; set; }
        public bool? IsAutoLogout { get; set; }
    }
}

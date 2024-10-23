using System.ComponentModel.DataAnnotations;

namespace PCNW.Models
{
    public partial class SepialineUser
    {
        [Key]
        public string? Company { get; set; }
        public string? Username { get; set; }
        public string? Pin { get; set; }
        public int Email { get; set; }
    }
}

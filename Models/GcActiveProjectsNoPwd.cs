using System.ComponentModel.DataAnnotations;

namespace PCNW.Models
{
    public partial class GcActiveProjectsNoPwd
    {
        [Key]
        public string? Addr1 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Zip { get; set; }
        public string? Pwd { get; set; }
        public string? Name { get; set; }
        public DateTime? BidDt { get; set; }
        public int Id { get; set; }
    }
}

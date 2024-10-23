using System.ComponentModel.DataAnnotations;

namespace PCNW.Models
{

    public partial class VwAspnetApplication
    {
        [Key]
        public string ApplicationName { get; set; } = null!;
        public string LoweredApplicationName { get; set; } = null!;
        public Guid ApplicationId { get; set; }
        public string? Description { get; set; }
    }
}

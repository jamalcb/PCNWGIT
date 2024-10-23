using System.ComponentModel.DataAnnotations;

namespace PCNW.Models
{
    public partial class AspnetSchemaVersion
    {
        [Key]
        public string Feature { get; set; } = null!;
        public string CompatibleSchemaVersion { get; set; } = null!;
        public bool IsCurrentVersion { get; set; }
    }
}

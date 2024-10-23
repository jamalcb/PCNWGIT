using System.ComponentModel.DataAnnotations;

namespace PCNW.Models
{
    public partial class DictionaryU
    {
        [Key]
        public string WordText { get; set; } = null!;
        public bool? CustomYn { get; set; }
        public short? WordLen { get; set; }
    }
}

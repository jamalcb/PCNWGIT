using System.ComponentModel.DataAnnotations;

namespace PCNW.Models
{
    public partial class SuggMatch
    {
        [Key]
        public string WordText { get; set; } = null!;
        public short? MatchNum { get; set; }
    }
}

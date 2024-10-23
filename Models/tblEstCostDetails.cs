using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("EstCostDetails")]

    public partial class tblEstCostDetails
    {
        [Key]
        public int Id { get; set; }
        public string? EstCostTo { get; set; }
        public string? EstCostFrom { get; set; }
        public int? Projid { get; set; }
        public bool Removed { get; set; } = false;
        public string? Description { get; set; }
        public string? RangeSign { get; set; } = "0";
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("AutoSearch")]
    public partial class TblAutoSearch
    {
        [Key]
        public int Id { get; set; }
        public int? MemId { get; set; }
        public string? Uid { get; set; }
        public string? Name { get; set; }
        public string? Keywords { get; set; }
        public bool? TypeB { get; set; } = false;
        public bool? TypeU { get; set; } = false;
        public bool? TypeR { get; set; } = false;
        public bool? TypeV { get; set; } = false;
        public string? Wage { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Distance { get; set; }
        public bool? SearchSpecs { get; set; }
        public string? Title { get; set; }
        public int? PlanNo { get; set; }
        public DateTime? ProjNewerThan { get; set; }
        public int? CountyId { get; set; }
        public string? Aoname { get; set; }
        public string? ConName { get; set; }
        public string? AndOr { get; set; }
        public string? RadioActiveArchAll { get; set; }
        public string? RadioAaachild { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public bool? UseCustomCounty { get; set; }
        public DateTime? LastUpdate { get; set; }
        public string? ProjSubTypeId { get; set; } = string.Empty;
        public string? EstCost { get; set; }
        public string? ProjectScopes { get; set; }
        public string? GeoState { get; set; }
    }
}

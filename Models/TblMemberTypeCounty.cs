using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("MemberTypeCounty")]

    public partial class TblMemberTypeCounty
    {
        [Key]
        public int MemTypeCountyId { get; set; }
        public int? MemberType { get; set; }
        public int? CountyID { get; set; }
        public string? Package { get; set; }
        public bool isActive { get; set; }
    }
}

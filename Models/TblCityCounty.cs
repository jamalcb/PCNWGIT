using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("CityCounty")]

    public partial class TblCityCounty
    {
        [Key]
        public int CityCountyId { get; set; }
        public string? City { get; set; }
        public int? CountyId { get; set; }
    }
}

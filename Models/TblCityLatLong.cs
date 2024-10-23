using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("CityLatLong")]

    public partial class TblCityLatLong
    {
        [Key]
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? Distance { get; set; }
    }
}

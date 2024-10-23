using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("ProjCounty")]

    public partial class TblProjCounty
    {
        [Key]
        public int ProjCountyId { get; set; }
        public int CountyId { get; set; }
        public int ProjId { get; set; }
    }
}

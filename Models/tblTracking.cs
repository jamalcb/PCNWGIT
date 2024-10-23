using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("Tracking")]

    public class tblTracking
    {
        [Key]
        public int TrackId { get; set; }
        public int MemberId { get; set; }
        public int ConId { get; set; }
        public int ProjId { get; set; }
        public bool IsTracking { get; set; }

    }
}

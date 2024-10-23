using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("DeliveryMaster")]

    public class tblDeliveryMaster
    {
        [Key]
        public int DelivId { get; set; }
        public string? DelivName { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}

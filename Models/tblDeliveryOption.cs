using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("DeliveryOption")]

    public class tblDeliveryOption
    {
        [Key]
        public int DelivOptId { get; set; }
        public int DelivId { get; set; }
        public string? DelivOptName { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}

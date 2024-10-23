using System.ComponentModel.DataAnnotations;

namespace PCNW.Models
{
    public class DeliveryViewModel
    {
        [Key]
        public int DelivId { get; set; }
        public string? DelivName { get; set; }
        public int DelivOptId { get; set; }
        public string? DelivOptName { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}

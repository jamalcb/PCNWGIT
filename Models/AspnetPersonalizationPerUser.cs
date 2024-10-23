using System.ComponentModel.DataAnnotations;

namespace PCNW.Models
{
    public partial class AspnetPersonalizationPerUser
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? PathId { get; set; }
        public Guid? UserId { get; set; }
        public byte[] PageSettings { get; set; } = null!;
        public DateTime LastUpdatedDate { get; set; }

        public virtual AspnetPath? Path { get; set; }
        public virtual AspnetUser? User { get; set; }
    }
}

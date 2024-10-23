using System.ComponentModel.DataAnnotations;

namespace PCNW.Models
{
    public partial class AspnetRole
    {
        public AspnetRole()
        {
            Users = new HashSet<AspnetUser>();
        }
        [Key]
        public Guid ApplicationId { get; set; }
        public Guid RoleId { get; set; }
        public string RoleName { get; set; } = null!;
        public string LoweredRoleName { get; set; } = null!;
        public string? Description { get; set; }

        public virtual AspnetApplication Application { get; set; } = null!;

        public virtual ICollection<AspnetUser> Users { get; set; }
    }
}

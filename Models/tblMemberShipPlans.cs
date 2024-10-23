using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("MemberShipPlans")]

    public class tblMemberShipPlans
    {
        [Key]
        public int MemberShipPlanId { get; set; }
        public string MemberShipPlanName { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public string? UpdatedBy { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string? MachineIP { get; set; }
        public bool Active { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("MemberShipSubPlans")]

    public class tblMemberShipSubPlans
    {
        [Key]
        public int SubMemberShipPlanId { get; set; }
        public string SubMemberShipPlanName { get; set; }
        public int MemberShipPlanId { get; set; }
        public Decimal YearlyPrice { get; set; }
        public Decimal MonthlyPrice { get; set; }
        public Decimal QuarterlyPrice { get; set; }
        //public string CreatedBy { get; set; }
        //public DateTime CreatedDate { get; set; }
        //public string? UpdatedBy { get; set; }
        //public DateTime? UpdatedDate { get; set; }
        //public string? MachineIP { get; set; }
        public bool Active { get; set; }
        public Decimal? AddYearlyPrice { get; set; }
        public Decimal? AddQuarterlyPrice { get; set; }
        public Decimal? AddMonthlyPrice { get; set; }
    }
}

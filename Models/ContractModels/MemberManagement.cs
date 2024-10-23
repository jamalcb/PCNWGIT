using System.ComponentModel.DataAnnotations;

namespace PCNW.Models.ContractModels
{
    public class MemberManagement
    {
        public List<Members>? Members { get; set; }
        public List<Contractors>? NonMember { get; set; }
        public List<Contractors>? FreeTrialMember { get; set; }
        public List<Contractors>? InactiveMember { get; set; }
        public List<Contractors>? Contractor { get; set; }
        public List<Contractors>? Architect { get; set; }
        public List<Contractors>? OtherTabData { get; set; }
        public List<Contractors>? ContractorFromMember { get; set; }
        public List<Contractors>? ArchitectFromMember { get; set; }
        public List<Entities>? Entities { get; set; }
        public string ReturnUrl { get; set; }
        public string Searchtext { get; set; } = "";
        public int TotalRecords { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
    }
    public class Members
    {
        public int? Id { get; set; }
        public string? Company { get; set; }
        public string? Contact { get; set; }
        public string? BillCity { get; set; }
        public string? BillState { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? MemberCost { get; set; }
        public string? MemberType { get; set; }
        public DateTime? RenewalDate { get; set; }
        public string? Discipline { get; set; }
        public string? Term { get; set; }
        public string Package { get; set; } = "";
        public bool Inactive { get; set; }
    }
    public class Contractors
    {
        public int? Id { get; set; }
        public string? Company { get; set; }
        public string? Contact { get; set; }
        public string? BillCity { get; set; }
        public string? BillState { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? MemberCost { get; set; }
        public string? MemberType { get; set; }
        public DateTime? RenewalDate { get; set; }
        public string? Discipline { get; set; }
        public string? Term { get; set; }
        public string Package { get; set; } = "";
        public int CompType { get; set; }
    }
    public class Entities
    {
        [Key]
        public int EntityID { get; set; }
        public string EntityType { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}

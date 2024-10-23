using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("IncompleteSignUp")]

    public class tblIncompleteSignUp
    {
        [Key]
        public int ID { get; set; }
        public string? Company { get; set; }
        public string? BillAddress { get; set; }
        public string? BillCity { get; set; }
        public string? BillState { get; set; }
        public string? BillZip { get; set; }
        public string? DBA { get; set; }
        public string? Fax { get; set; }
        public string? MailAddress { get; set; }
        public string? MailCity { get; set; }
        public string? MailState { get; set; }
        public string? MailZip { get; set; }
        public string? CompanyPhone { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ContactPhone { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPassword { get; set; }
        public string? Term { get; set; }
        public string? MemberType { get; set; }
        public string? MemberCost { get; set; }
        public string? BillEmail { get; set; }
        public string? Extension { get; set; }
        public int? DiscountId { get; set; }
        public Guid? UserId { get; set; }
    }
}

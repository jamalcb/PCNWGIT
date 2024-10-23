using System.ComponentModel.DataAnnotations;
namespace PCNW.Models.ContractModels
{
    public class EditMemberProfile
    {
        [Key]
        public int Id { get; set; }
        public string? CompanyName { get; set; }
        public string? CompanyEmail { get; set; }
        public string? CompanyPhone { get; set; }
        public string? CompanyPhoneNumber { get; set; }
        public string? BillAddress { get; set; }
        public string? BillCity { get; set; }
        public string? BillState { get; set; }
        public string? BillZip { get; set; }
        public string? BillCountry { get; set; }
        public string? BillPhone { get; set; }
        public string? Disipline { get; set; }
        public string? Note { get; set; }
        public string? DBA { get; set; }
        public string? DBA2 { get; set; }
        public string? MailAddress { get; set; }
        public string? MailCity { get; set; }
        public string? MailState { get; set; }
        public string? MailZip { get; set; }
        public string? Email { get; set; }
        public string? Password { get; set; }
        public string? Password2 { get; set; }
        public string? Fax { get; set; }
    }
}

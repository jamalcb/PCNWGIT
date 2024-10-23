using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PCNW.ViewModel
{
    public class ActiveMemberViewModel
    {
        [Key]
        public int Id { get; set; }
        [Column("Insert Date")]
        public string? Company { get; set; }
        public string? Discipline { get; set; }
        public bool Inactive { get; set; }
        public string? BillAddress { get; set; }
        public string? MailCity { get; set; }
        public string? MailState { get; set; }
        public string? MailZip { get; set; }
        public DateTime? RenewalDate { get; set; }
        public int? MemberType { get; set; }
        public string? Email { get; set; }
        /// <summary>
        /// Favorites/IEN # control
        /// </summary>

        public int? ConId { get; set; }
        public string? MailAddress { get; set; }
        public bool? IsAutoRenew { get; set; }
        public string? CompanyPhone { get; set; }
        public string? Password { get; set; }
    }
}

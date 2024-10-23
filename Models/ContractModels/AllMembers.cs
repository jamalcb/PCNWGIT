using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
namespace PCNW.Models.ContractModels
{
    public class AllMembers
    {
        [Key]
        public int Id { get; set; }
        [Column("Insert Date")]
        public string? Company { get; set; }
        public string? Contact { get; set; }
        public bool Inactive { get; set; }
        public string? BillAddress { get; set; }
        public string? BillCity { get; set; }
        public string? BillState { get; set; }
        public string? BillZip { get; set; }
        public DateTime? RenewalDate { get; set; }
        public int? MemberType { get; set; }
        public string? Email { get; set; }
        /// <summary>
        /// Favorites/IEN # control
        /// </summary>

        public int? ConId { get; set; }
        public string? MailAddress { get; set; }
        public string? MailCity { get; set; }
        public string? MailState { get; set; }
        public bool? IsAutoRenew { get; set; }
        public string? CompanyPhone { get; set; }
    }
}


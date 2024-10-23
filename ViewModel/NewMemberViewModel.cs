using System.ComponentModel.DataAnnotations;
namespace PCNW.ViewModel
{
    public class NewMemberViewModel
    {
        [Key]
        public int Id { get; set; }
        public string? Company { get; set; }
        public string? Discipline { get; set; }
        public bool Inactive { get; set; }
        public string? BillAddress { get; set; }
        public string? MailCity { get; set; }
        public string? MailState { get; set; }
        public DateTime? InsertDate { get; set; }
        public int? MemberType { get; set; }


    }
}

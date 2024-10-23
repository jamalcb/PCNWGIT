using PCNW.Models.ContractModels;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PCNW.ViewModel
{
    public class CopyCenterMemberInfo
    {
        public int Id { get; set; }
        public string? Company { get; set; }

        [Required(ErrorMessage = "Please enter bill address")]
        [DisplayName("Address")]
        [StringLength(50, ErrorMessage = "Address should be maximum 50 character")]
        public string? BillAddress { get; set; }

        [Required(ErrorMessage = "Please enter bill city")]
        [DisplayName("City")]
        [StringLength(50, ErrorMessage = "City should be maximum 50 character")]
        public string? BillCity { get; set; }

        [Required(ErrorMessage = "Please enter bill state")]
        [DisplayName("State")]
        [StringLength(2, ErrorMessage = "State should be maximum 2 character")]
        public string? BillState { get; set; }

        [Required(ErrorMessage = "Please enter bill zip code")]
        [DisplayName("Zip Code")]
        [StringLength(50, ErrorMessage = "Zip Code should be maximum 50 character")]
        public string? BillZip { get; set; }

        [DisplayName("Billing Mail")]
        [EmailAddress]
        public string? DailyEmail { get; set; }
        public string? Email { get; set; }
        public string? PaperlessBilling { get; set; }
        public List<MemberContactInfo>? ContactList { get; set; }
    }
}

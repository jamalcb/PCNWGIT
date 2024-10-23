using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PCNW.Models.ContractModels
{
    public class MemberShipRegistration
    {
        public MemberShipRegistration()
        {
        }

        public int CompId { get; set; }
        public DateTime? InsertDate { get; set; }
        [DisplayName("Company")]
        public string? Company { get; set; }
        public bool Inactive { get; set; }

        // [Required(ErrorMessage = "Please enter bill address")]
        [DisplayName("Address")]
        public string? BillAddress { get; set; }

        //[Required(ErrorMessage = "Please enter bill city")]
        [DisplayName("City")]
        public string? BillCity { get; set; }


        [DisplayName("State")]
        public string? BillState { get; set; }

        /// <summary>
        /// /[Required(ErrorMessage = "Please enter bill zip code")]
        /// </summary>
        [DisplayName("Zip Code")]
        public string? BillZip { get; set; }
        public string? LastPayDate { get; set; }
        public DateTime? RenewalDate { get; set; }
        public string? Term { get; set; }
        public string? Div { get; set; }
        public string? Discipline { get; set; }
        public string? Note { get; set; }
        public string? MinorityStatus { get; set; }
        public List<string>? DivisionList { get; set; }
        public List<string>? MinorityStatusList { get; set; }
        public List<MemberContactInfo>? ContactList { get; set; }
        /// <summary>
        /// 0 = Regular, 1 = Full, 2 = Partial
        /// </summary>
        public string? MemberType { get; set; }
        public bool AcceptedTerms { get; set; }
        public DateTime? AcceptedTermsDt { get; set; }

        public DateTime? MemNoteDate { get; set; }

        public bool? IsAutoRenew { get; set; }

        [DisplayName("Billing Mail")]
        [EmailAddress]
        public string? DailyEmail { get; set; }
        public bool? Html { get; set; }
        public bool? Overdue { get; set; }
        public bool? Cod { get; set; }
        public byte[]? TmStamp { get; set; }
        public string? PaperlessBilling { get; set; }
        public string? MemberCost { get; set; }
        public decimal? MagCost { get; set; }
        public decimal? ArchPkgCost { get; set; }
        public decimal? AddPkgCost { get; set; }
        public string? ResourceDate { get; set; }
        public decimal? ResourceCost { get; set; }
        public string? WebAdDate { get; set; }
        public decimal? WebAdCost { get; set; }
        public bool? Phl { get; set; }
        public string? Email { get; set; }
        /// <summary>
        /// Favorites/IEN # control
        /// </summary>
        public decimal? NameField { get; set; }
        public DateTime? FavExp { get; set; }
        public int? Grace { get; set; }
        public int? ContactId { get; set; }
        public bool? Gcservices { get; set; }
        public string? ResourceStandard { get; set; }
        public string? ResourceColor { get; set; }
        public string? ResourceLogo { get; set; }
        public string? ResourceAdd { get; set; }
        public string? Dba { get; set; }
        public string? Dba2 { get; set; }
        public string? Fka { get; set; }
        public bool? Suspended { get; set; }
        public string? SuspendedDt { get; set; }
        public string? Fax { get; set; }
        [DisplayName("Address")]
        public string? MailAddress { get; set; }
        [DisplayName("City")]
        public string? MailCity { get; set; }
        [DisplayName("State")]
        public string? MailState { get; set; }
        [DisplayName("MailZip")]
        public string? MailZip { get; set; }
        public decimal? OverdueAmt { get; set; }
        public DateTime? OverdueDt { get; set; }
        public string? CalSort { get; set; }
        public bool? Pdfpkg { get; set; }
        public bool? ArchPkg { get; set; }
        public bool? AddPkg { get; set; }
        public bool? Bend { get; set; }
        public int? Credits { get; set; }
        public bool? FreelanceEstimator { get; set; }
        public string? HowdUhearAboutUs { get; set; }
        public string? Logo { get; set; }
        public bool CheckDirectory { get; set; }
        public bool? Daily { get; set; }

        //tbl Contact Property
        public int ConID { get; set; }
        public int ID { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? ContactName { get; set; }
        public string? ContactTitle { get; set; }
        public Nullable<bool> MainContact { get; set; }
        public string? ContactPhone { get; set; }
        public Nullable<bool> ContactDaily { get; set; }
        public Nullable<bool> ContactTextMsg { get; set; }
        //[UniqueEmail]
        [Remote("ValidateEmailAddress", "Home")]
        [EmailAddress]
        public string? ContactEmail { get; set; }
        public string? ContactUID { get; set; }
        public string? ContactPassword { get; set; }
        public string? ContactPasswordCofirmation { get; set; }
        public Nullable<bool> ContactMessage { get; set; }
        public Nullable<DateTime> ContactMessageDt { get; set; }
        public Nullable<bool> ContactAutoSearch { get; set; }

        // Manage Tab Control
        public string? Scope { get; set; }
        public int? hdnpreStep { get; set; }
        public int? hdnnextStep { get; set; }
        public int? hdncurrentStep { get; set; }

        public string? AuthorizedBy { get; set; }
        public string? REMOTE_ADDR { get; set; }
        public string? strMessage { get; set; }
        public int? lngMemID { get; set; }
        public int? WebDivId { get; set; }
        public MinorityStatusViewModel[]? MinorityLists { get; set; }
        public string? SelectedValue { get; set; }
        public string? hdnPass { get; set; }
        public string? hdnPassConfirm { get; set; }
        public string? hdnTerm { get; set; }
        public string? Certification { get; set; }
        public string? CheckRadio { get; set; }
        public string? CheckRadioEx { get; set; }
        public string? CheckRadioExValue { get; set; }
        public string? CheckRadioValue { get; set; }
        public string? RadioExValue { get; set; }
        public bool isFromFree { get; set; } = false;
        public string? Package { get; set; }
        public string? CompanyPhone { get; set; }
        public string? BillCounty { get; set; }
        public List<NoteInfo>? NoteList { get; set; }
        public string? MSPChk { get; set; } = "N";
        public string? ProfileChk { get; set; } = "N";
        public int ToDelete { get; set; } = 0;
        //tblDirectoryChk
        public TblDirectoryCheck? DirectoryCheck { get; set; } = new();
        public string? License { get; set; }
        public List<LicenseInfo>? LicenseInfos { get; set; }
        public string? ConsBillAddress { get; set; }
        public string? ConsMailAddress { get; set; }
        public IDictionary<string, string>? Locations { get; set; } = new Dictionary<string, string>();
        public List<LocListViewModel>? LocationsList { get; set; }
        public string? UserName { get; set; }
        public string PayStatus { get; set; } = "N";
        public int LocId { get; set; } = 0;
        public string? MailStateId { get; set; }
        public string? BillStateId { get; set; }
        [EmailAddress]
        public string? BillEmail { get; set; }
        public string? Additionalcost { get; set; }
        public string? PayCardString { get; set; }
        public string? ColorString { get; set; }
        public int MemID { get; set; }
        public int InvoiceId { get; set; }
        public string? Extension { get; set; }
        public int CompType { get; set; } = 1;
        public string? Type { get; set; }
        public string? ContractorName { get; set; }
        public string? ArchitectName { get; set; }
        public string? Controller { get; set; }
        public int? DiscountId { get; set; }
        public string? Discount { get; set; }
        public string? PayModeRef { get; set; }
        public string? CreatedBy { get; set; }
        public Guid ASPUserId { get; set; }
        public bool? IsDelete { get; set; } = false;
        public DateTime? AddGraceDate { get; set; }
        public bool? IsMember { get; set; }
        public bool? IsArchitect { get; set; }
        public bool? IsContractor { get; set; }
        public DateTime? ActualRenewalDate { get; set; } = null;
    }
    public class MemberContactInfo
    {
        public string? Contact { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public int? ConID { get; set; }
        public bool? MainContact { get; set; }
        public bool? Daily { get; set; }
        public int LocId { get; set; } = 0;
        public string? UID { get; set; }
    }
    public class NoteInfo
    {
        public int Id { get; set; }
        public int? MemId { get; set; }
        public DateTime? LogDate { get; set; }
        public string? Note { get; set; }
        public string? Name { get; set; }
        public bool? Flag { get; set; }
        public string? CompType { get; set; }
    }
    public class LicenseInfo
    {
        public List<int?>? State { get; set; } = new List<int?> { 0 };
        public string? LicNum { get; set; } = "";
        public string? LicDesc { get; set; } = "";

    }
    public class LocListViewModel
    {
        [Key]
        public int LocId { get; set; } = 0;
        public string? LocAddr { get; set; } = "";
        public string? LocCity { get; set; } = "";
        public string? LocState { get; set; } = "";
        public string? LocCounty { get; set; } = "";
        public string? LocZip { get; set; } = "";
        public string? LocPhone { get; set; } = "";
        public int? MemId { get; set; } = 0;
        public int? LocStateCode { get; set; } = 0;
    }
}
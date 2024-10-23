using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("Member")]
    public partial class TblMember
    {
        [Key]
        public int Id { get; set; }
        [Column("Insert Date")]
        public DateTime? InsertDate { get; set; }
        public string? Company { get; set; }
        public bool Inactive { get; set; }
        public string? BillAddress { get; set; }
        public string? BillCity { get; set; }
        public string? BillState { get; set; }
        public string? BillZip { get; set; }
        public string? LastPayDate { get; set; }
        public DateTime? RenewalDate { get; set; }
        public string? Term { get; set; }
        public string? Div { get; set; }
        public string? Discipline { get; set; }
        public string? Note { get; set; }
        public string? MinorityStatus { get; set; }
        /// <summary>
        /// 0 = Regular, 1 = Full, 2 = Partial
        /// </summary>
        public int? MemberType { get; set; }
        public bool AcceptedTerms { get; set; }
        public DateTime? AcceptedTermsDt { get; set; }
        [Column("Daily_Email")]
        public string? DailyEmail { get; set; }
        public bool? Html { get; set; }
        public bool? Overdue { get; set; }
        public bool? Cod { get; set; }
        //public byte[]? TmStamp { get; set; }
        [Column("Paperless_Billing")]
        public string? PaperlessBilling { get; set; }
        [Column("Member_Cost")]
        public decimal? MemberCost { get; set; }
        [Column("Mag_Cost")]
        public decimal? MagCost { get; set; }
        [Column("ArchPkg_Cost")]
        public decimal? ArchPkgCost { get; set; }
        [Column("AddPkg_Cost")]
        public decimal? AddPkgCost { get; set; }
        [Column("Resource_Date")]
        public string? ResourceDate { get; set; }
        [Column("Resource_Cost")]
        public decimal? ResourceCost { get; set; }
        [Column("WebAd_Date")]
        public string? WebAdDate { get; set; }
        [Column("WebAd_Cost")]
        public decimal? WebAdCost { get; set; }
        public bool? Phl { get; set; }
        public string? Email { get; set; }
        /// <summary>
        /// Favorites/IEN # control
        /// </summary>
        public decimal? NameField { get; set; }
        public DateTime? FavExp { get; set; }
        public int? Grace { get; set; }
        [Column("Con_ID")]
        public int? ConId { get; set; }
        public bool? Gcservices { get; set; }
        [Column("Resource_Standard")]
        public string? ResourceStandard { get; set; }
        [Column("Resource_Color")]
        public string? ResourceColor { get; set; }
        [Column("Resource_Logo")]
        public string? ResourceLogo { get; set; }
        [Column("Resource_Add")]
        public string? ResourceAdd { get; set; }
        public string? Dba { get; set; }
        public string? Dba2 { get; set; }
        public string? Fka { get; set; }
        public bool? Suspended { get; set; }
        public string? SuspendedDt { get; set; }
        public string? Fax { get; set; }
        public string? MailAddress { get; set; }
        public string? MailCity { get; set; }
        public string? MailState { get; set; }
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
        public bool? IsAutoRenew { get; set; }
        public string? CompanyPhone { get; set; }
        public string? Logo { get; set; }
        public bool CheckDirectory { get; set; }
        public int MemID { get; set; }
        public int InvoiceId { get; set; }
        public string? Discount { get; set; }
        public string? PayModeRef { get; set; }
        public string? CreatedBy { get; set; }
        public bool? IsDelete { get; set; } = false;
        public DateTime? AddGraceDate { get; set; }
        public bool? IsMember { get; set; }
        public bool? IsArchitect { get; set; }
        public bool? IsContractor { get; set; }
        public DateTime? ActualRenewalDate { get; set; } = null;

    }
}

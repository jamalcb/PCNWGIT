namespace PCNW.ViewModel
{
    public class MemberSummaryViewModel
    {
        public int? MemberTypeCount { get; set; }
        public int? RenewalCount { get; set; }
        public int? SubscriptionCount { get; set; }
        public int? CancellationCount { get; set; }
    }
    public class ManageCountyViewModel
    {
        public int? MemberTypeId { get; set; }
        public int? CountyName { get; set; }
        public int? CountyId { get; set; }
        public int? Package { get; set; }
        public bool isActive { get; set; }
    }
    public class CountyDescriptionViewModel
    {
        public int Id { get; set; }
        public string? UserText { get; set; }
        public string? RegionText { get; set; }
        public string? PackageName { get; set; }
        public int? MemberType { get; set; }
        public string? RegionHead { get; set; }
    }
    public class CopyCenterAdminViewModel
    {

        public int Id { get; set; }

        public string? SizeName { get; set; }

        public string? Size { get; set; }

        public decimal? MemberPrice { get; set; }

        public decimal? NonMemberPrice { get; set; }
        public decimal? ColorMemberPrice { get; set; }

        public decimal? ColorNonMemberPrice { get; set; }

        public string isActive { get; set; } = "";

    }
    public class DiscountViewModel
    {
        public int DiscountId { get; set; }

        public int? DiscountRate { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string? Description { get; set; }

        public string isActive { get; set; } = "";

    }
    public class DailyMailerViewModel
    {
        public int Id { get; set; }

        public string? MailerText { get; set; }

        public string? MailerPath { get; set; }
        public bool IsActive { get; set; } = false;
        public string? ImageUrl { get; set; }

    }
}

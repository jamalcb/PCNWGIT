namespace PCNW.Models.ContractModels
{
    public class MemberDashboard
    {
        public List<TrackedProjects>? TrackedProject { get; set; }
        public List<MemberProjects>? MemberProject { get; set; }
    }
    public class MemberProjects
    {
        public int? ProjId { get; set; }
        public string? Title { get; set; }
        public DateTime? ArrivalDt { get; set; }
        public DateTime? BidDt { get; set; }
        public string? strBidDt5 { get; set; }
        public string? strBidDt { get; set; }
        public string? ProjTypeIdString { get; set; }
        public string? LocAddr2 { get; set; }
        public string? LocAddr1 { get; set; }
        public string? LocZip { get; set; }
        public string? LocState { get; set; }
        public string? LocCity { get; set; }
        public int? TrackCount { get; set; }
        public int? BidCount { get; set; }
    }
    public class TrackedProjects
    {
        public int? ProjId { get; set; }
        public string? Title { get; set; }
        public DateTime? ArrivalDt { get; set; }
        public DateTime? BidDt { get; set; }
        public string? strBidDt5 { get; set; }
        public string? strBidDt { get; set; }
        public string? ProjTypeIdString { get; set; }
        public string? LocAddr2 { get; set; }
        public string? LocAddr1 { get; set; }
        public string? LocZip { get; set; }
        public string? LocState { get; set; }
        public string? LocCity { get; set; }
        public bool? MemberTrack { get; set; }
        public bool? BiddingStatus { get; set; }
        public bool? chkCalendar { get; set; }

    }
}

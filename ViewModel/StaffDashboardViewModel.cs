
using Microsoft.EntityFrameworkCore;

namespace PCNW.ViewModel
{
    [Keyless]
    public class StaffDashboardViewModel
    {
        public int ProjId { get; set; }
        public string Title { get; set; }
        public string ProjNumber { get; set; }
        public Nullable<bool> Publish { get; set; }
        public Nullable<bool> SpcChk { get; set; }
        public Nullable<bool> SpecsOnPlans { get; set; }
        public string LocCity { get; set; }
        public string LocState { get; set; }
        public string ProjTypeIdString { get; set; }
        public Nullable<DateTime> ArrivalDt { get; set; }
        public Nullable<DateTime> BidDt { get; set; }
        public Nullable<bool> MemberTrack { get; set; }
        public Nullable<int> TrackCount { get; set; }
        public Nullable<bool> BR { get; set; }
        public string strBidDt { get; set; }
        public Nullable<int> memberId { get; set; }
        public string? LBidDt { get; set; }
        public List<string>? Entities { get; set; }
    }
    //Title	
    //ArrivalDt	
    //BidDt	
    //BidDt2	
    //BidDt3	
    //BidDt4	
    //BidDt5	
    //ProjTypeId	
    //ProjId	
    //memberId	
    //MemberIDs	
    //ProjTypeIdString	
    //MemberTrack	
    //TrackCount	
    //BidCount	
    //BiddingStatus
    public class MemberDashboardViewModel
    {
        public Nullable<int> ProjId { get; set; }
        public string? Title { get; set; }
        public Nullable<DateTime> ArrivalDt { get; set; }
        public Nullable<DateTime> BidDt { get; set; }
        public Nullable<DateTime> BidDt2 { get; set; }
        public Nullable<DateTime> BidDt3 { get; set; }
        public Nullable<DateTime> BidDt4 { get; set; }
        public Nullable<DateTime> BidDt5 { get; set; }
        public Nullable<int> ProjTypeId { get; set; }
        public string? ProjTypeIdString { get; set; }
        public Nullable<int> memberId { get; set; }
        public Nullable<int> MemberTrack { get; set; }
        public Nullable<int> TrackCount { get; set; }
        public Nullable<int> BidCount { get; set; }
        public Nullable<int> BiddingStatus { get; set; }
        public Nullable<int> CurrMemberId { get; set; }
        public Nullable<int> MemberIDs { get; set; }
    }
}

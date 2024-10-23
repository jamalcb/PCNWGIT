namespace PCNW.Models.ContractModels
{
    public class FindProjectModel
    {
        public List<FindProjectView>? ActiveProjs { get; set; }
        public List<FindProjectView>? FutureProjs { get; set; }
        public List<FindProjectView>? PrevProjs { get; set; }


        public bool IsSolrSearched { get; set; }=false;
        public string? SolrKeyword { get; set; }=string.Empty;
    }
    public class FindProjectView
    {
        public int? ProjId { get; set; }
        public string? Title { get; set; }
        public string? LocAddr1 { get; set; }
        public string? LocAddr2 { get; set; }
        public string? LocCity { get; set; }
        public string? LocState { get; set; }
        public string? LocZip { get; set; }
        public string? strBidDt5 { get; set; }
        public DateTime? ImportDt { get; set; }
        public string? ProjNumber { get; set; }
        public DateTime? BidDt { get; set; }
        public DateTime? ArrivalDt { get; set; }
        public string? strBidDt { get; set; }
        public bool? Publish { get; set; }
        public bool? SpcChk { get; set; }
        public bool? SpecsOnPlans { get; set; }
        public bool? MemberTrack { get; set; }
        public string? ProjTypeIdString { get; set; }
        public int? TrackCount { get; set; }
        public string? MBDCheck { get; set; }
        public int? AddendaCount { get; set; }
    }

}

using System.ComponentModel.DataAnnotations;

namespace PCNW.Models.ContractModels
{
    public class MemberProjectInfo
    {
        [Key]
        public int ProjId { get; set; }

        public string? Title { get; set; }
        public string? LocAddr1 { get; set; }
        public string? LocAddr2 { get; set; }
        public string? LocCity { get; set; }
        public string? LocState { get; set; }
        public string? LocZip { get; set; }
        public DateTime? PreBidDt { get; set; }
        public DateTime? BidDt { get; set; }
        public string? CompleteDt { get; set; }
        public string? LastBidDt { get; set; }
        public string? IssuingOffice { get; set; }
        public int? ProjTypeId { get; set; }
        public int? PlanNo { get; set; }
        public string? BidBond { get; set; }
        public string? EstCost { get; set; }
        public string? EstCostRange1 { get; set; }
        public string? EstCostRange2 { get; set; }
        public bool PrevailingWage { get; set; }
        public string? SubApprov { get; set; }
        public bool Mandatory { get; set; }
        public DateTime? ExtendedDt { get; set; }
        public int? BidNo { get; set; }
        public DateTime? ArrivalDt { get; set; }
        public string? PreBidLoc { get; set; }
        public string? strBidDt { get; set; }
        public List<TblEntity>? tblEntities { get; set; }
        public string? ProjNote { get; set; }
        public DateTime? BidDt2 { get; set; }
        public string? strBidDt2 { get; set; }
        public DateTime? BidDt3 { get; set; }
        public string? strBidDt3 { get; set; }
        public DateTime? BidDt4 { get; set; }
        public string? strBidDt4 { get; set; }
        public DateTime? BidDt5 { get; set; }
        public string? strBidDt5 { get; set; }
        public DateTime? PreBidDt2 { get; set; }
        public string? strPreBidDt2 { get; set; }
        public string? PreBidLoc2 { get; set; }
        public DateTime? PreBidDt3 { get; set; }
        public string? strPreBidDt3 { get; set; }
        public string? PreBidLoc3 { get; set; }
        public DateTime? PreBidDt4 { get; set; }
        public string? strPreBidDt4 { get; set; }
        public string? PreBidLoc4 { get; set; }
        public DateTime? PreBidDt5 { get; set; }
        public string? strPreBidDt5 { get; set; }
        public string? PreBidLoc5 { get; set; }
        public bool SpcChk { get; set; }
        public bool Publish { get; set; }
        public string? ContactName { get; set; }
        public string? ContactPhone { get; set; }
        public string? ContactMember { get; set; }
        public string? ContactEmail { get; set; }
        public string? ProjNumber { get; set; }
        public int? ProjSubTypeId { get; set; }
        public string? ProjScope { get; set; }
        public List<string>? ProjScopeList { get; set; }
        public int? memberId { get; set; }
        public bool SpecsOnPlans { get; set; }
        public List<EntityInformation>? EIList { get; set; }
        public string ErrorMsg { get; set; } = "";
        public string? AuthorizedBy { get; set; }
        public string? REMOTE_ADDR { get; set; }
        public string? strMessage { get; set; }
        public List<string> EmailsTo { get; set; }
        public double? Longitude { get; set; }
        public double? Latitude { get; set; }
        public int CountyId { get; set; }
        public string? LocalPath { get; set; }
        public string? ProjTypeIdString { get; set; }
        public string? ProjSubTypeIdString { get; set; }
        public bool FutureWork { get; set; }
        public string? Undecided { get; set; }
        public int? tComp { get; set; }
        public int? hComp { get; set; }
        public string? mValue { get; set; }
        public int? lastTComp { get; set; }
        public int? lastHComp { get; set; }
        public string? lastMValue { get; set; }
        public List<PreBidInfo>? preBidInfos { get; set; }
        public List<EstCostInfo>? EstCostDetails { get; set; }
        public string? Story { get; set; }
        public List<EntityInformation>? Entities { get; set; }
        public List<PlanHLInformation>? phlInfo { get; set; }
        public List<PHLInformation>? pInfo { get; set; }
        public string? EstCost2 { get; set; }
        public string? EstCost3 { get; set; }
        public string? EstCost4 { get; set; }
        public string? EstCost5 { get; set; }
        public string? Counties { get; set; }
        public string? PrebidNote { get; set; }

    }

}

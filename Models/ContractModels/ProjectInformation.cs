using PCNW.ViewModel;
using SolrNet.Attributes;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace PCNW.Models.ContractModels
{
    public class ProjectInformation
    {
        [Key]
        public int ProjId { get; set; }

        [Required(ErrorMessage = "Please enter address")]
        public string? Title { get; set; }
        [DisplayName("Local Addess 1")]
        public string? LocAddr1 { get; set; }
        public string? LocAddr2 { get; set; }
        [Display(Name = "Local Addess")]
        public string? LocCity { get; set; }
        public string? LocState { get; set; }
        public string? LocZip { get; set; }
        public DateTime? PreBidDt { get; set; }
        public DateTime? BidDt { get; set; }
        public string? BidDateTime { get; set; }
        //Est. Completion
        public string? CompleteDt { get; set; }

        public string? LastBidDt { get; set; }
        public string? IssuingOffice { get; set; }
        public decimal? RefundAmt { get; set; }
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
        public DateTime? ResultDt { get; set; }
        public string? PreBidLoc { get; set; }
        //Bid Date *Time Component
        public string? strBidDt { get; set; }
        public string? strPreBidDt { get; set; }
        public string? ProjNote { get; set; }
        public decimal? NonRefundAmt { get; set; }
        public string? DFNote { get; set; }
        public DateTime? DupArDt { get; set; }
        public int? Dup1 { get; set; }
        public int? Dup2 { get; set; }
        public bool FutureWork { get; set; }
        public string? BRNote { get; set; }
        public string? Story { get; set; }
        public Single? AdSpacer { get; set; }
        public int? EstCostNum { get; set; }
        public bool Publish { get; set; }
        public string? StoryUNF { get; set; }
        public DateTime? ImportDt { get; set; }
        public string? DiPath { get; set; }
        public bool ShowOnWeb { get; set; }
        public string? DupTitle { get; set; }
        public string? PublishedFrom { get; set; }
        public DateTime? PublishedFromDt { get; set; }
        public bool UnderCounter { get; set; }
        public bool Recycle { get; set; }
        public bool Hold { get; set; }
        public bool CallBack { get; set; }
        public decimal? Deposit { get; set; }
        public decimal? ShipCheck { get; set; }
        public DateTime? CheckSentDt { get; set; }
        public int? SpecVols { get; set; }
        public int? DrawingVols { get; set; }
        public string? InternalNote { get; set; }
        public bool ShowBR { get; set; }
        public string? EstCost2 { get; set; }
        public string? EstCost2Range1 { get; set; }
        public string? EstCost2Range2 { get; set; }
        public string? EstCost3 { get; set; }
        public string? EstCost3Range1 { get; set; }
        public string? EstCost3Range2 { get; set; }
        public string? EstCost4 { get; set; }
        public string? EstCost4Range1 { get; set; }
        public string? EstCost4Range2 { get; set; }
        public string? EstCost5 { get; set; }
        public string? EstCost5Range1 { get; set; }
        public string? EstCost5Range2 { get; set; }
        public int? EstCostNum2 { get; set; }
        public int? EstCostNum3 { get; set; }
        public int? EstCostNum4 { get; set; }
        public int? EstCostNum5 { get; set; }
        public bool UC { get; set; }
        public string? UCPWD { get; set; }
        public string? SpecPath { get; set; }
        public string? DrawingPath { get; set; }
        public string? MaxViewPath { get; set; }
        public bool TopChk { get; set; }
        public bool DwChk { get; set; }
        public bool SpcChk { get; set; }
        public bool RenChk { get; set; }
        public string? OnlineNote { get; set; }
        public int? RegionID { get; set; }
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
        public bool Mandatory2 { get; set; }
        public bool PrebidAND { get; set; }
        public bool PrebidOR { get; set; }
        public byte[]? ProjTimeStamp { get; set; } // timestamp
        public string? Contact { get; set; }
        public string? UCPWD2 { get; set; }
        public bool S11x17 { get; set; }
        public bool S18x24 { get; set; }
        public bool S24x36 { get; set; }
        public bool S30x42 { get; set; }
        public bool NoSpecs { get; set; }
        public bool SpecsOnPlans { get; set; }
        public bool InternetDownload { get; set; }
        public string? BRResultsFrom { get; set; }
        public string? LocCity2 { get; set; }
        public string? LocState2 { get; set; }
        public string? LocCity3 { get; set; }
        public string? LocState3 { get; set; }
        public bool DirtID { get; set; }
        public bool S36X48 { get; set; }
        //Previous Bid Date
        public DateTime? BidDt2 { get; set; }
        //Previous Bid Date PST
        public string? strBidDt2 { get; set; }
        public DateTime? BidDt3 { get; set; }
        //BidDate PST
        public string? strBidDt3 { get; set; }
        public DateTime? BidDt4 { get; set; }
        //Previous Bid Date Time
        public string? strBidDt4 { get; set; }
        public DateTime? BidDt5 { get; set; }
        //TBD Pre Bid Date
        public string? strBidDt5 { get; set; }
        public int? PlanNoMain { get; set; }
        public int? ProjIdMain { get; set; }
        public bool ShowToAll { get; set; }
        public string? PHLnote { get; set; }
        public bool PHLdone { get; set; }
        public string? PHLwebLink { get; set; }
        public DateTime? PHLtimestamp { get; set; }
        public bool BendPC { get; set; }
        public bool NoPrint { get; set; }
        public bool UCPublic { get; set; }
        public bool BidPkg { get; set; }
        public bool BuildSolrIndex { get; set; }
        public float? Longitude { get; set; }
        public float? Latitude { get; set; }
        public string? GeogPt { get; set; }  //geography
        public string? strAddenda { get; set; }
        public DateTime? SolrIndexDt { get; set; }
        public bool IndexPDFFiles { get; set; }
        public DateTime? SolrIndexPDFDt { get; set; }
        public string? ProjNumber { get; set; }
        public int? ProjSubTypeId { get; set; }
        public string? ProjScope { get; set; }
        public List<string>? ProjScopeList { get; set; }
        public List<string>? PdfList { get; set; }
        public List<EntityInformation>? Entities { get; set; }
        public List<BidDateInformation>? BidDateTimes { get; set; }
        public string? ProjTypeIdString { get; set; }
        public string? ProjSubTypeIdString { get; set; }
        public bool? MemberTrack { get; set; }
        public bool? BiddingStatus { get; set; }
        public int? BidCount { get; set; }
        public int? TrackCount { get; set; }
        public int? memberId { get; set; }
        public int? CurrMemberId { get; set; }
        public string? createdBy { get; set; }
        public DateTime? createdDate { get; set; }
        public string? updatedBy { get; set; }
        public DateTime? updatedDate { get; set; }
        public string? machineIP { get; set; }
        public List<string> AddendaS3 { get; set; }
        public List<AddendaFileInfo> AddendaS3Files { get; set; }
        public List<AddendaDIRInfo> AddendaDIRInfos { get; set; }
        public List<BidResultsInfo> BidResultsFiles { get; set; }
        public List<PHLInfo> PHLFiles { get; set; }
        public List<PlansInfo> PlansFiles { get; set; }
        public List<SpecsInfo> SpecsFiles { get; set; }
        public string SuccessMessage { get; set; } = "";
        public List<AddendaInfo> AddendaInformation { get; set; }
        //public List<string> PHLS3 { get; set; }
        public List<string> PlansS3 { get; set; }
        public List<string> SpecsS3 { get; set; }
        public List<string> BidResultsS3 { get; set; }
        public bool? chkCalendar { get; set; }
        public DateTime? HiddenBidDt { get; set; }
        public string? AuthorizedBy { get; set; }
        public string? REMOTE_ADDR { get; set; }
        public List<string>? EmailsTo { get; set; }
        public string? strMessage { get; set; }
        public List<PHLInformation>? pInfo { get; set; }
        public List<PlanHLInformation>? phlInfo { get; set; }
        public int CountyId { get; set; }
        public string? Undecided { get; set; } = "";
        public bool? BoolUndecided { get; set; }
        public string? Counties { get; set; }
        public string MBDCheck { get; set; } = "N";
        public List<PreBidInfo>? preBidInfos { get; set; }
        public string strCounties { get; set; }
        public bool? BoolPreUndecided { get; set; }
        public bool? BoolAllOr { get; set; }
        public bool? BoolAllWa { get; set; }
        public List<EstCostInfo>? EstCostDetails { get; set; }
        public int? tComp { get; set; }
        public int? hComp { get; set; }
        public string? mValue { get; set; }
        public int? lastTComp { get; set; }
        public int? lastHComp { get; set; }
        public string? lastMValue { get; set; }
        public string? AddendaNote { get; set; }
        public string? PrebidNote { get; set; }

        public Dictionary<string, Dictionary<int, List<SearchResultViewModel>>> SolrSearchResult{ get; set; }

    }

    
    public class AddendaFileInfo
    {
        public string? PathInfo { get; set; }
        public string? FileInfo { get; set; }
        public long? Size { get; set; }
        public bool? IsFile { get; set; }
    }
    public class AddendaDIRInfo
    {
        public string? PathInfo { get; set; }
        public string? FileInfo { get; set; }
        public long? Size { get; set; }
        public bool? IsFile { get; set; }
        public List<AddendaFileInfo> addendaFileInfos { get; set; }
    }
    public class BidResultsInfo
    {
        public string? PathInfo { get; set; }
        public string? FileInfo { get; set; }
    }
    public class PHLInfo
    {
        public string? PathInfo { get; set; }
        public string? FileInfo { get; set; }
    }
    public class PlansInfo
    {
        public string? PathInfo { get; set; }
        public string? FileInfo { get; set; }
    }
    public class SpecsInfo
    {
        public string? PathInfo { get; set; }
        public string? FileInfo { get; set; }
    }
    public class EntityInformation
    {
        public int EntityID { get; set; }
        public string? EntityType { get; set; }
        public string? EntityName { get; set; }
        public int NameId { get; set; } = 0;
        public int? Projid { get; set; }
        public int? ProjNumber { get; set; }
        public string? EntAddr { get; set; }
        public string? EntAddr2 { get; set; }
        public string? EntFax { get; set; }
        public string? EntMail { get; set; } = "";
        public string? EntPhone { get; set; }
        public string? EntContact { get; set; }
        public bool chkIssue { get; set; } = false;
        public bool IsActive { get; set; } = true;
        public string? EntityTypeString { get; set; }
        public int? CompType { get; set; }
    }
    public class AddendaInfo
    {
        public int AddendaId { get; set; }
        public int? ProjId { get; set; }
        public string? AddendaNo { get; set; }
        public DateTime? IssueDt { get; set; }

    }
    public class BidDateInformation
    {
        public int ID { get; set; }
        public int? ProjId { get; set; }
        public DateTime? BidDate { get; set; }
        public string? PST { get; set; }
        public Nullable<bool> IsDeleted { get; set; }
        public string? BidDateTime { get; set; }
        public int? AddendaId { get; set; }
        public int? PhlId { get; set; }
        public Nullable<bool> Isextensions { get; set; }
        public DateTime? CreatedDate { get; set; }
        public int? tComp { get; set; }
        public int? hComp { get; set; }
        public string? mValue { get; set; }
        public int? ExtNum { get; set; }

    }
    public class PlanHLInformation : tblPhlInfo
    {
        public string? Company { get; set; }
        public string? StrContact { get; set; }
        public string? contactPhone { get; set; }
        public string? contactEmail { get; set; }
        public string? PHLTypeString { get; set; }
        public string? BidStatusString { get; set; }
    }
    public class PHLInformation
    {
        public int ID { get; set; }
        public int? ProjId { get; set; }
        public string? Company { get; set; }
        public string? Contact { get; set; }
        public bool? bidding { get; set; }
        public int PHLType { get; set; } = 3;
        public string? Uid { get; set; }
        public string PHLNote { get; set; } = "";
        public int memId { get; set; } = 0;
        public string PHLTypeString { get; set; } = "Prime";
        public bool? Undecided { get; set; }
        public int CompType { get; set; } = 1;
        public List<BidDateInformation>? PhlBidDate { get; set; }
        public string? StrContact { get; set; }
        public BidDateInformation phlBid1 { get; set; }
        public BidDateInformation phlBid2 { get; set; }
        public BidDateInformation phlBid3 { get; set; }
        public string? contactPhone { get; set; }
        public string? contactEmail { get; set; }
        //Multiple Bid Date Check

    }
    public class ZipInfo
    {
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string? County { get; set; }
        public int? CountyId { get; set; }

    }

    public class PreBidInfo
    {
        public int Id { get; set; }
        public DateTime? PreBidDate { get; set; }
        public string? PreBidTime { get; set; }
        public string? Location { get; set; }
        public string? PST { get; set; }
        public bool Mandatory { get; set; } = false;
        public bool PreBidAnd { get; set; } = false;
        public bool IsDeleted { get; set; } = false;
        public bool? UndecidedPreBid { get; set; } = false;
        public int? ProjId { get; set; }
        public int? tComp { get; set; }
        public int? hComp { get; set; }
        public string? mValue { get; set; }

    }
    public class EstCostInfo
    {
        public string Id { get; set; }
        public string EstFrom { get; set; } = "";
        public string EstTo { get; set; } = "";
        public string Description { get; set; } = "";
        public bool? Removed { get; set; }
        public string? RangeSign { get; set; } = "0";
    }
}
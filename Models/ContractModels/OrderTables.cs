using System.ComponentModel.DataAnnotations;

namespace PCNW.Models.ContractModels
{
    public class OrderTables
    {
        // tblOrder
        [Key]
        public int? OrderId { get; set; }
        public DateTime? OrderDt { get; set; }
        public string? PO { get; set; }

        public string? UID { get; set; }
        public string? Company { get; set; }
        public string? Name { get; set; }
        public int? ProjId { get; set; }
        public string? Title { get; set; }
        public int? PlanNo { get; set; }
        public string? BluePrints { get; set; }
        public string? SpecSections { get; set; }
        public string? Email { get; set; }
        public int? Sz1Qty { get; set; }
        public Decimal? Sz1Chrg { get; set; }
        public int? Sz2Qty { get; set; }
        public Decimal? Sz2Chrg { get; set; }
        public int? Sz3Qty { get; set; }
        public Decimal? Sz3Chrg { get; set; }
        public int? Sz4Qty { get; set; }
        public Decimal? Sz4Chrg { get; set; }
        public int? Sz5Qty { get; set; }
        public Decimal? Sz5Chrg { get; set; }
        public int? Sz6Qty { get; set; }
        public Decimal? Sz6Chrg { get; set; }
        public string? HowShip { get; set; }
        public Decimal? ShipAmt { get; set; }
        public bool? Done { get; set; }
        public bool? Canceled { get; set; }
        public DateTime? DoneDt { get; set; }
        public DateTime? ShipDt { get; set; }
        public bool? Billed { get; set; }
        public string? Addr { get; set; }
        public string? CSZ { get; set; }
        public string? Phone { get; set; }
        public string? Instructions { get; set; }
        public bool? Okd { get; set; }
        public string? DoneBy { get; set; }
        public bool? WrittenUp { get; set; }
        public string? WrittenUpBy { get; set; }
        public DateTime? WrittenDt { get; set; }
        public bool? CopyRoom { get; set; }
        public string? CopyRoomBy { get; set; }
        public DateTime? CopyRoomDt { get; set; }
        public bool? Shipped { get; set; }
        public string? ShipBy { get; set; }
        public string? strBidDt { get; set; }
        public DateTime? PrintDt { get; set; }
        public int? Prints { get; set; }
        public bool? NonMember { get; set; }
        public string? NotifyAddress { get; set; }
        public string? Fax { get; set; }
        public string? PCLocation { get; set; }
        public bool? Viewed { get; set; }
        public DateTime? DeliveryDt { get; set; }
        //tblProjOrderDetail
        public int? ProjOrderId { get; set; }
        public string? FileName { get; set; }
        //
        public int? OrderChrgId { get; set; }
        //public Decimal? Sz1Chrg { get; set; }
        //public Decimal? Sz2Chrg { get; set; }
        //public Decimal? Sz3Chrg { get; set; }
        //public Decimal? Sz4Chrg { get; set; }
        //public Decimal? Sz5Chrg { get; set; }
        //public Decimal? Sz6Chrg { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Zip { get; set; }
        public List<OrderDetails> GetTblProjs { get; set; }
        public List<string> EmailsTo { get; set; }
        public string? AuthorizedBy { get; set; }
        public string? REMOTE_ADDR { get; set; }
        public string? strMessage { get; set; }
        public string? EmailContent { get; set; }
        public string? BaseUrl { get; set; }
        public string? PaymentMode { get; set; }
        public string? PayStatus { get; set; }
        public string? Source { get; set; }
    }
    public class OrderDetails
    {
        public string? FileName { get; set; }
        public int? Pages { get; set; }
        public int? Copies { get; set; }
        public string? Size { get; set; }
        public string? PrintName { get; set; }
        public Decimal? Price { get; set; }
    }
    public class AutoFillData
    {
        public int Id { get; set; }
        public int ConID { get; set; }
        public int StateCode { get; set; } = 0;
        public string Company { get; set; } = "";
        public string Name { get; set; } = "";
        public string City { get; set; } = "";
        public string Zip { get; set; } = "";
        public string Address { get; set; } = "";
        public string Phone { get; set; } = "";
        public string Email { get; set; } = "";
    }

    public class PrevOrder
    {
        public int NoP { get; set; } = 1;
        public string? FileName { get; set; }
    }
}

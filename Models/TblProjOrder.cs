using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("ProjOrder")]

    public partial class TblProjOrder
    {
        [Key]
        public int OrderId { get; set; }
        public DateTime? OrderDt { get; set; }
        public string? Po { get; set; }
        public string? Uid { get; set; }
        public string? Company { get; set; }
        public string? Name { get; set; }
        public int? ProjId { get; set; }
        public string? Title { get; set; }
        public int? PlanNo { get; set; }
        public string? BluePrints { get; set; }
        public string? SpecSections { get; set; }
        public string? Email { get; set; }
        public int? Sz1Qty { get; set; }
        public decimal? Sz1Chrg { get; set; }
        public int? Sz2Qty { get; set; }
        public decimal? Sz2Chrg { get; set; }
        public int? Sz3Qty { get; set; }
        public decimal? Sz3Chrg { get; set; }
        public int? Sz4Qty { get; set; }
        public decimal? Sz4Chrg { get; set; }
        public int? Sz5Qty { get; set; }
        public decimal? Sz5Chrg { get; set; }
        public int? Sz6Qty { get; set; }
        public decimal? Sz6Chrg { get; set; }
        public string? HowShip { get; set; }
        public decimal? ShipAmt { get; set; }
        /// <summary>
        /// 0 = Not Done 1 = Done
        /// </summary>
        public bool? Done { get; set; }
        /// <summary>
        /// 0 = NotCanceled 1 =Canceled (uses a check box)
        /// </summary>
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
        public string? StrBidDt { get; set; }
        public DateTime? PrintDt { get; set; }
        public int? Prints { get; set; }
        public bool? NonMember { get; set; }
        public string? NotifyAddress { get; set; }
        public string? Fax { get; set; }
        public string? Pclocation { get; set; }
        public bool? Viewed { get; set; }
        public DateTime? DeliveryDt { get; set; }
        public string? PaymentMode { get; set; }
    }
}

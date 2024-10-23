namespace MvcCodeFlowClientManual.Models.Quickbook
{
    public class QuickBookPayment
    {
        public CustomerRefInfo CustomerRef { get; set; }
        public decimal TotalAmt { get; set; }
        public List<LineInfo> Line { get; set; }
    }
    public class CustomerRefInfo
    {
        public string value { get; set; }
    }
    public class LineInfo
    {
        public decimal Amount { get; set; }
        public List<TxnInfo> LinkedTxn { get; set; }
    }
    public class TxnInfo
    {
        public string TxnId { get; set; }
        public string TxnType { get; set; }
    }
}

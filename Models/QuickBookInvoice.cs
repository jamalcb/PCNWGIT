namespace MvcCodeFlowClientManual.Models.Quickbook
{
    public class QuickBookInvoice
    {
        public List<SaleInfo> Line { get; set; }
        public CustomerInfo CustomerRef { get; set; }
    }
    public class CustomerInfo
    {
        public string value { get; set; }
    }
    public class SaleInfo
    {
        public string DetailType { get; set; }
        public decimal Amount { get; set; }
        public SalesItemLineDetailClass SalesItemLineDetail { get; set; }
    }
    public class SalesItemLineDetailClass
    {
        public ItemRefClass ItemRef { get; set; }
    }
    public class ItemRefClass
    {
        public string name { get; set; }
        public string value { get; set; }
    }

    //{
    //  "Line": [
    //    {
    //      "DetailType": "SalesItemLineDetail",
    //      "Amount": 100.0,
    //      "SalesItemLineDetail": {
    //        "ItemRef": {
    //          "name": "Services",
    //          "value": "1"
    //        }
    //}
    //    }
    //  ], 
    //  "CustomerRef": {
    //    "value": "1"
    //  }
    //}
}
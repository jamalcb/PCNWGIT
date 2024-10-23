namespace PCNW.Data.ContractResponse
{
    public class AddNotification
    {
        public int MemberID { get; set; }
        public string? Contact { get; set; }
        public string? Email { get; set; }
        public bool? Daily { get; set; }
    }
}

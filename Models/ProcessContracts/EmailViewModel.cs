namespace PCNW.Models.ProcessContracts
{
    public class EmailViewModel
    {
        public string EmailFrom { get; set; }
        public List<string> EmailTos { get; set; } = new();
        public string Subject { get; set; }
        public string strMessage { get; set; }
        public string REMOTE_ADDR { get; set; }
        public string AuthorizedBy { get; set; }
        public string CcEmail { get; set; }
        public string BccEmail { get; set; }
    }
}

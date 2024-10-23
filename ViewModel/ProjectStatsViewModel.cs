namespace PCNW.ViewModel
{
    public class ProjectStatsViewModel
    {
        public int ActiveCount { get; set; }
        public int ArchiveCount { get; set; }
        public int BActiveCount { get; set; }
        public int BArchiveCount { get; set; }
        public int UActiveCount { get; set; }
        public int UArchiveCount { get; set; }
        public int RActiveCount { get; set; }
        public int RArchiveCount { get; set; }
        public int VActiveCount { get; set; }
        public int VArchiveCount { get; set; }
        public int WACount { get; set; }
        public int ORCount { get; set; }
        public int AKCount { get; set; }
        public int IDCount { get; set; }
        public int CACount { get; set; }
        public int MTCount { get; set; }
    }
    public class UnpublishedProject
    {
        public int ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public string? BidDate { get; set; }
        public string? ArrivalDate { get; set; }

    }
    public class ActiveProject
    {
        public int ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public int? AddendaCount { get; set; }
        public string? BidDate { get; set; }
        public string? ArrivalDate { get; set; }
        public string? ProjNumber { get; set; }
        public string? ProjTypeIdString { get; set; }
    }

}

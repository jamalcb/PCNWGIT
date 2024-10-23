namespace PCNW.ViewModel
{
    public class SearchViewModel
    {
        public int Id { get; set; }
        public string? SearchText { get; set; }
        public string? strBidDateFrom { get; set; }
        public string? strBidDateTo { get; set; }
        public List<string>? ProjectTypeIds { get; set; } = new();
        public List<string>? ProjectSubTypeIds { get; set; } = new();
        public List<string>? ProjectestCosts { get; set; } = new();
        public List<string>? ProjectStates { get; set; } = new();
        public List<string>? ProjectScopes { get; set; }
        public int ProjectTypeId { get; set; }
        public int ProjectSubTypeId { get; set; } = 0;
        public List<string>? StateList { get; set; }
        public bool PrevailingWageFlag { get; set; }
        public bool NewConstructionFlag { get; set; }
        public bool RemodelFlag { get; set; }
        public bool AdditionFlag { get; set; }
        public bool FutureWorkFlag { get; set; }
        public string? Name { get; set; }
        public string City { get; set; } = null!;
        public string State { get; set; } = null!;
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? Distance { get; set; }
        public string? EstCost { get; set; }


        public bool IsSolrSearched { get; set; } = false;
        public string? SolrKeyword { get; set; } = string.Empty;
    }
}

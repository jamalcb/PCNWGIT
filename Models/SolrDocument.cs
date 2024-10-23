using SolrNet.Attributes;

namespace PCNW.Models
{
    public class SolrDocument
    {
        [SolrUniqueKey("id")]
        public string Id { get; set; }

        [SolrField("filename")]
        public List<string> Filename { get; set; }

        [SolrField("projectId")]
        public List<string> projectId { get; set; }

        [SolrField("content")]
        public IList<string> Content { get; set; }

        [SolrField("sourceType")]
        public List<string> SourceType { get; set; }
    }

}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("SubAddenda")]

    public partial class tblSubAddenda
    {
        [Key]
        public int SubAddendaId { get; set; }
        public string? Desc1 { get; set; }
        public string? Pdfpath { get; set; }
        public int? AddendaId { get; set; }
        public int? ProjId { get; set; }
        public string? PdfFileName { get; set; }
        public string? ParentFolder { get; set; }
        public bool Deleted { get; set; } = false;
    }
}

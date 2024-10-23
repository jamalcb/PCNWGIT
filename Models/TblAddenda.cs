using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("Addenda")]
    public partial class TblAddenda
    {
        [Key]
        public int AddendaId { get; set; }
        public string? AddendaNo { get; set; }
        public bool? MoreInfo { get; set; }
        public int? ProjId { get; set; }
        public DateTime? InsertDt { get; set; }
        public string? MvwebPath { get; set; }
        public DateTime? IssueDt { get; set; }
        public string? PageCnt { get; set; }
        public bool? NewBd { get; set; }
        public string? ParentFolder { get; set; }
        public bool Deleted { get; set; } = false;
        public int ParentId { get; set; } = 0;
    }
}

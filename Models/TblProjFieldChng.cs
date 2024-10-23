using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("ProjFieldChng")]
    public partial class TblProjFieldChng
    {
        [Key]
        public int ChngId { get; set; }
        public int? ProjId { get; set; }
        public DateTime? ChngDt { get; set; }
        public DateTime? EmailDt { get; set; }
        public string? FieldName { get; set; }
        public int? SortOrder { get; set; }
        public int? ChangedId { get; set; }
    }
}

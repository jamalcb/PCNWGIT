using System.ComponentModel.DataAnnotations;

namespace PCNW.Models.ContractModels
{
    public class ProjectSubTypeViewModel
    {
        [Key]
        public int ProjSubTypeID { get; set; }

        [StringLength(150, ErrorMessage = "Project sub type name should be maximum 150 character")]
        public string ProjSubType { get; set; }
        public int? SortOrder { get; set; }
        public int? ProjTypeID { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}

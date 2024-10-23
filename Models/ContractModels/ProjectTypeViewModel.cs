using System.ComponentModel.DataAnnotations;

namespace PCNW.Models.ContractModels
{
    public class ProjectTypeViewModel
    {
        [Key]
        public int ProjTypeId { get; set; }

        [StringLength(150, ErrorMessage = "Project Type name should be maximum 150 character")]
        public string ProjType { get; set; }
        public int? SortOrder { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}

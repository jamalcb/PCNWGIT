using System.ComponentModel.DataAnnotations;

namespace PCNW.Models.ContractModels
{
    public class EntityTypeViewModel
    {
        [Key]
        public int EntityID { get; set; }

        [StringLength(150, ErrorMessage = "Entity type should be maximum 150 character")]
        public string EntityType { get; set; }
        public int? SortOrder { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}

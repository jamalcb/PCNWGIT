using System.ComponentModel.DataAnnotations;

namespace PCNW.Models.ContractModels
{
    public class PHLTypeViewModel
    {
        [Key]
        public int PHLID { get; set; }

        [StringLength(150, ErrorMessage = "PHL type should be maximum 150 character")]
        public string PHLType { get; set; }
        public int? SortOrder { get; set; }
        public Nullable<bool> IsActive { get; set; }
    }
}

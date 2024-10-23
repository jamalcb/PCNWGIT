
using System.ComponentModel.DataAnnotations;

namespace PCNW.Models.ResponseContracts
{
    public class TblProjectCode
    {
        [Key]
        public Int64 ProjNumber { get; set; }
    }
}

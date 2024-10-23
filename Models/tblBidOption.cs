using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("BidOption")]

    public class tblBidOption
    {
        [Key]
        public int Id { get; set; }
        public string? BidOption { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

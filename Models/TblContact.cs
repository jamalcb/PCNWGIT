using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("Contact")]
    public partial class TblContact
    {
        [Key]
        [Column("ContactID")]
        public int ConId { get; set; }
        [Column("BusinessEntityID")]
        public int Id { get; set; }

        [Column("UserId")]
        public Guid UserId { get; set; }

        [Column("ContactName", TypeName = "nvarchar(50)")]
        public string? Contact { get; set; }
        [Column("ContactTitle", TypeName = "nvarchar(50)")]
        public string? Title { get; set; }
        [Column("MainContact")]
        public bool? MainContact { get; set; }
        [Column("ContactPhone", TypeName = "nvarchar(50)")]
        public string? Phone { get; set; }
        [Column("Daily")]
        public bool? Daily { get; set; }
        [Column("TextMsg")]
        public bool? TextMsg { get; set; }
        [Column("ContactEmail", TypeName = "nvarchar(50)")]
        public string? Email { get; set; }
        [Column("UID")]
        public string? Uid { get; set; }
        [Column("Password")]
        public string? Password { get; set; }
        [Column("Message")]
        public bool? Message { get; set; }
        [Column("MessageDt")]
        public DateTime? MessageDt { get; set; }
        [Column("AutoSearch")]
        public bool? AutoSearch { get; set; }
        [Column("ContactState")]
        public string? ContactState { get; set; }
        [Column("ContactAddress")]
        public string? ContactCity { get; set; }
        [Column("ContactCity")]
        public string? ContactAddress { get; set; }
        [Column("ContactZip")]
        public string? ContactZip { get; set; }
        [Column("ContactCounty")]
        public string? ContactCounty { get; set; }
        [Column("LocId")]
        public int LocId { get; set; }
        [Column("BillEmail")]
        public string? BillEmail { get; set; }
        [Column("Extension", TypeName = "nvarchar(50)")]
        public string? Extension { get; set; }
        [Column("CompType")]
        public int CompType { get; set; } = 1;
        [Column("Active")]
        public bool? Active { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
    }
}

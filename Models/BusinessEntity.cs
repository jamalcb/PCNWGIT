using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models;

[Table("BusinessEntity")]
public partial class BusinessEntity
{
    public int BusinessEntityId { get; set; }
    public string? BusinessEntityName { get; set; } = null!;
    public string? BusinessEntityEmail { get; set; }
    public string? BusinessEntityPhone { get; set; }
    public bool IsMember { get; set; }
    public bool IsArchitect { get; set; }
    public bool IsContractor { get; set; }
    public int OldAoID { get; set; }
    public int OldConID { get; set; }
    public int OldMemID { get; set; }
    public virtual ICollection<Address> Addresses { get; set; } = new List<Address>();

    // public virtual ICollection<Contact> Contacts { get; set; } = new List<Contact>();

    public virtual ICollection<Member> Members { get; set; } = new List<Member>();
}

namespace PCNW.Models;

public partial class Address
{
    public int AddressId { get; set; }

    public int BusinessEntityId { get; set; }

    public string AddressName { get; set; } = null!;

    public string Addr1 { get; set; } = null!;

    public string City { get; set; } = null!;

    public string State { get; set; } = null!;

    public string Zip { get; set; } = null!;

    public virtual BusinessEntity BusinessEntity { get; set; } = null!;
}

using System.ComponentModel.DataAnnotations;
namespace PCNW.ViewModel
{
    public class ContractorViewModel
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Addr1 { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Zip { get; set; }
        public string? Phone { get; set; }
        public int? Type2 { get; set; }
        public int? ConTypeId { get; set; }
        public string? Email { get; set; }
        public string? WebAddress { get; set; }
    }
}

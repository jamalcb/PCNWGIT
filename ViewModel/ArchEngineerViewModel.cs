using System.ComponentModel.DataAnnotations;
namespace PCNW.ViewModel
{
    public class ArchEngineerViewModel
    {
        [Key]
        public int Id { get; set; }
        public string? Name { get; set; }
        public string? Addr1 { get; set; }
        public string? Type1 { get; set; }
        public string? Email { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Zip { get; set; }
        public string? Phone { get; set; }


    }
}

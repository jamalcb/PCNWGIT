using System.ComponentModel.DataAnnotations;

namespace PCNW.Models.ContractModels
{
    public class FileStorageViewModel
    {
        [Key]
        public int Id { get; set; }
        public string? FileStorage { get; set; }
    }
}

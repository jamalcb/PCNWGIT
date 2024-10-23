using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
    [Table("DirectoryCheck")]

    public class TblDirectoryCheck
    {
        [Key]
        public int DirId { get; set; } = 0;
        public int? MemId { get; set; } = 0;
        public bool Company { get; set; } = false;
        public bool MailAddr { get; set; } = false;
        public bool BillAddr { get; set; } = false;
        public bool DBA { get; set; } = false;
        public bool PrimaryContact { get; set; } = false;
        public bool Phone { get; set; } = false;
        public bool Business { get; set; } = false;
        public bool Speciality { get; set; } = false;
        public bool License { get; set; } = false;
        public bool Email { get; set; } = false;
        public bool Logo { get; set; } = false;
    }
}

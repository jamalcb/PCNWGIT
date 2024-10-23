using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PCNW.Models
{
	[Table("ProjSubType")]
	public class ProjSubTypes
	{
		[Key]
		public int ProjSubTypeID { get; set; }
		public string? ProjSubType { get; set; }
		public Nullable<int> SortOrder { get; set; }
		public Nullable<int> ProjTypeID { get; set; }
	}
}

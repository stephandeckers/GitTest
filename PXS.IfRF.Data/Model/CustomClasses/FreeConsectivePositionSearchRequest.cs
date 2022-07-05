using System.ComponentModel.DataAnnotations;

namespace PXS.IfRF.Data.Model
{
	public class FreeConsectivePositionSearchRequest
	{
		[Required]
		public long SubrackId { get; set; }
		[Required]
		public string PositionType { get; set; }
		[Required]
		public string From_To { get; set; }
		[Required]
		public string Owner { get; set; }

	}
}

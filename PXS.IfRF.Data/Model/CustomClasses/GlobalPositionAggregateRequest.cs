using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace PXS.IfRF.Data.Model
{
	public class GlobalPositionAggregateRequest
	{
		public long RfAreaId { get; set; }

		public List<string> SubrackTypes { get; set; }

	}
}

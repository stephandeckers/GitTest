using System.ComponentModel.DataAnnotations;

namespace PXS.IfRF.Data.Model
{
	public class GlobalPositionAggregateResponse
	{
		public string Owner { get; set; }

		public long RfAreaId { get; set; }

		public string RfAreaCode { get; set; }

		public string RfAreaName { get; set; }

		public long PopId { get; set; }

		public string PopName { get; set; }

		public string PopType { get; set; }

		public string SubrackType { get; set; }

		public string PositionType { get; set; }

		public string OltPositionType { get; set; }

		public int NrTotalPositions { get; set; }

		public int NrFreePositions { get; set; }

		public int NrUsedPositions { get; set; }

		public int NrBlockedPositions { get; set; }

	}
}

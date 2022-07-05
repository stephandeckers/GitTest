using System.ComponentModel.DataAnnotations;

namespace PXS.IfRF.Data.Model
{
	public class SrPositionAggregateResponse
	{
		public string PosGroup { get; set; }

		public string Type { get; set; }

		public int NrTotalPositions { get; set; }

		public int NrFreePositions { get; set; }

		public int NrUsedPositions { get; set; }
		public SrPosition OltPosition { get; set; }

		public string OltPositionName { get; set; }

	}
}

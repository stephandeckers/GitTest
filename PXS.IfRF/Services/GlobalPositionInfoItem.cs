using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PXS.IfRF.Services
{
    public class GlobalPositionInfoItem
    {
		public string Owner { get; set; }
		public long RfAreaId { get; set; }
		public string RfAreaName { get; set; }
		public string RfAreaCode { get; set; }
		public long PopId { get; set; }
		public string PopName { get; set; }
		public string PopType { get; set; }
		public string SubrackType { get; set; }
		public long PositionId { get; set; }
		public string PositionType { get; set; }
		public string OltPositionType { get; set; }
		public string OperationalStatus { get; set; }
		public int NrFroms { get; set; }
		public int NrTos { get; set; }
    }
}

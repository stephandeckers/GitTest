using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PXS.IfRF.Services
{
	#region -- Using directives --
	using PXS.IfRF.Data.Model;
	using System.Collections.Generic;
	#endregion

	public interface IFreeSplitterPositionSearchService
    {
		List<FreeSplitterPositionSearchResponse> GetFreeSplitterPositions	( FreeSplitterPositionSearchRequest request);
	}

	internal class IncomingInfo
	{
		internal SrPosition OltPosition;
		internal SrPosition	PPPosition;
	}
}

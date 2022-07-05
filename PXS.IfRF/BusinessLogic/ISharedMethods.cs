using PXS.IfRF.Data.Model;
using PXS.IfRF.Data.Model.ONTPPositionConnectionSearch;
using System.Collections.Generic;

namespace PXS.IfRF.BusinessLogic
{
	public interface ISharedMethods
	{
		(SrPosition, string) TraceSplInToOlt(long spli_id);

		ResponseConnection TraceOntp2Splo	( long ontp_id);
		ResponseSrPosition TraceSplo2Olt	( long splo_id);
		
		string GetPositionName	( SrPosition position, string owner);
		string GetSubrackName	( Subrack subrack, string owner);
		string GetPopName		( long sr_id);

		IDictionary<long, string> TraceRfAreaSplInToOlt(long rfAreaId);
	}
}
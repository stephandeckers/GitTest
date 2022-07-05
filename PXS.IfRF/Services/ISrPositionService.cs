/**
 * @Name ISrPositionService.cs
 * @Purpose 
 * @Date 27 July 2021, 19:32:35
 * @Author S.Deckers
 * @Description 
 */

namespace PXS.IfRF.Services
{
	#region -- Using directives --
	using Microsoft.AspNet.OData;
	using PXS.IfRF.BusinessLogic;
	using PXS.IfRF.Data.Model;
	using System.Collections.Generic;
	using System.Linq;
	#endregion

	public interface ISrPositionService
	{
		(BusinessLogicResult, SrPosition)		Create	( SrPosition item);
		BusinessLogicResult						Delete	( long id);
		SingleResult<SrPosition>				Get		( long id);
		IQueryable<SrPosition>					GetItems( );
		(BusinessLogicResult, SrPosition)		Patch	( long id, Delta<SrPosition> deltaItem);

		List<PositionConnectionSearchResponse>	GetPositionsConnected (long subrackid);
	}
}
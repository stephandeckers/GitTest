/**
 * @Name ISubrackService.cs
 * @Purpose 
 * @Date 12 July 2021, 07:43:34
 * @Author S.Deckers
 * @Description 
 */

namespace PXS.IfRF.Services
{
	#region -- Using directives --
	using Microsoft.AspNet.OData;
	using Microsoft.AspNetCore.Http;
	using PXS.IfRF.BusinessLogic;
	using PXS.IfRF.Data.Model;
	using System.Collections.Generic;
	using System.Linq;
	#endregion

	public interface ISubrackService
	{
		Subrack					Create					( Subrack subrack);
		List<SrPosition>		CreateCustomPositions	( CustomPositionRequest customPositionRequest);
		BusinessLogicResult		Delete					( long id);
		SingleResult<Subrack>	Get						( long id);
		IQueryable<Subrack>		GetItems				( );
		Subrack					Patch					( long id, Delta<Subrack> subrack);
	}
}
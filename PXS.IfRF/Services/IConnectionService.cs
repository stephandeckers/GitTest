/**
 * @Name IRsConnectionService.cs
 * @Purpose 
 * @Date 15 July 2021, 21:56:43
 * @Author S.Deckers
 * @Description 
 */

namespace PXS.IfRF.Services
{
	#region -- Using directives --
	using PXS.IfRF.BusinessLogic;
	using Microsoft.AspNet.OData;
	using PXS.IfRF.Data.Model;
	using System.Linq;
	#endregion

	public interface IConnectionService
	{
		(BusinessLogicResult, Connection)	Create	( Connection item);
		(BusinessLogicResult, bool)			Delete	( long id);
		SingleResult<Connection>			Get		( long id);
		IQueryable<Connection>				GetItems( );
		(BusinessLogicResult, Connection)	Patch	( long id, Delta<Connection> deltaItem);
	}
}
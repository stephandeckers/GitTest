namespace PXS.IfRF.Services
{
	#region -- Using directives --
	using Microsoft.AspNet.OData;
    using Microsoft.AspNetCore.Http;
    using PXS.IfRF.Data.Model;
	using System.Collections.Generic;
	using System.Linq;
	using PXS.IfRF.BusinessLogic;
	#endregion

	public interface IRackspaceService
	{
		RackSpace				Create	( RackSpace rackspace);
		bool					Delete	( long id);
		//RackSpace				Get		( long id);
		SingleResult<RackSpace>				Get		( long id);
		IQueryable<RackSpace>	GetItems( );
		//RackSpace				Patch	( long id, Delta<RackSpace> rackspace);
		(BusinessLogicResult, RackSpace)		Patch	( long id, Delta<RackSpace> deltaItem);
	}
}
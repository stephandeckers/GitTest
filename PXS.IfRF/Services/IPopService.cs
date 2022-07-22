using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using PXS.IfRF.Data.Model;
using PXS.IfRF.BusinessLogic;
using System.Collections.Generic;
using System.Linq;

namespace PXS.IfRF.Services
{
	public interface IPopService
	{
		(BusinessLogicResult, Pop)		Create	( Pop item);
		bool							Delete	( long id);
		SingleResult<Pop>				Get		( long id);
		IQueryable<Pop>					GetItems( );
		(BusinessLogicResult, Pop)		Patch	( long id, Delta<Pop> deltaItem);
	}
}
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using PXS.IfRF.Data.Model;
using System.Collections.Generic;
using System.Linq;
using PXS.IfRF.BusinessLogic;

namespace PXS.IfRF.Services
{
    public interface IRfAreaService
    {
        RfArea				CreateRfArea ( RfArea rfArea);
        bool				DeleteRfArea( long id);
        //IQueryable<RfArea>	GetRfArea	( long id);
		SingleResult<RfArea>	Get		( long id);
        IQueryable<RfArea>	GetRfAreas	();
        //RfArea				PatchRfArea ( long id, Delta<RfArea> rfArea);
		(BusinessLogicResult, RfArea)		Patch	( long id, Delta<RfArea> deltaItem);
    }
}


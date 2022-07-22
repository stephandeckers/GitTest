/**
 * @Name IAvailabilityService.cs
 * @Purpose 
 * @Date 21 July 2021, 11:56:52
 * @Author S.Deckers
 * @Description 
 */

namespace PXS.IfRF.Services
{
	#region -- Using directives --
	using Microsoft.AspNet.OData;
	using PXS.IfRF.Data.Model;
	using System.Collections.Generic;
	using System.Linq;
	#endregion

    public interface IAvailabilityService
    {
		List<AvailabilityResponse>	Check		( AvailabilityRequest request);
    }
}


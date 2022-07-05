using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using PXS.IfRF.Data.Model;
using System.Collections.Generic;
using System.Linq;

namespace PXS.IfRF.Services
{
	public interface IResourceService
	{
		bool									DeleteResource		( long id);
		Resource								Get					( long id);

		IQueryable<Resource>					GetResources		( );
		Resource								PatchResource		( long id, Delta<Resource> Resource);
		IEnumerable<ResourceCharacteristic>	AddCharacteristics	( long resourceId, IEnumerable<ResourceCharacteristic> newResourceCharacteristics);
		Resource CreateNewResource(SpecificResource specificResource);
		void UpdateResourceMetadataFields(Resource resource);
	}
}


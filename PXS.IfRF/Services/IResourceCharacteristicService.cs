using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using PXS.IfRF.Data.Model;
using System.Collections.Generic;
using System.Linq;

namespace PXS.IfRF.Services
{
	public interface IResourceCharacteristicService
	{
		ResourceCharacteristic				Create	( ResourceCharacteristic ResourceCharacteristic);
		bool								Delete	( long id);
		ResourceCharacteristic				Get		( long id);
		IQueryable<ResourceCharacteristic>	GetItems( );
		ResourceCharacteristic				Patch	( long id, Delta<ResourceCharacteristic> ResourceCharacteristic);
	}
}
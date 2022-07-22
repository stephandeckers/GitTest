using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using PXS.IfRF.Data.Model;
using System.Collections.Generic;
using System.Linq;

namespace PXS.IfRF.Services
{
	public interface IResourceOrderService
	{
		ResourceOrder				Create(ResourceOrder ResourceOrder);
		bool						Delete(long id);
		ResourceOrder				Get(long id);
		IQueryable<ResourceOrder>	GetItems();
		ResourceOrder				Patch(long id, Delta<ResourceOrder> ResourceOrder);
	}
}
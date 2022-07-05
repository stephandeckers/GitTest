using Microsoft.AspNet.OData;
using PXS.IfRF.Data.Model;
using PXS.IfRF.Logging;
using System;
using System.Linq;
using PXS.IfRF.AuthHandling;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace PXS.IfRF.Services
{
    public class ResourceOrderService : IResourceOrderService
    {
        private readonly ModelContext _ifrfContext = null;
        private readonly ILoggerManager _logger = null;
        private readonly IResourceService _resourceService;

        public ResourceOrderService(ModelContext ifrfContext, ILoggerManager logger, IResourceService resourceService)
        {
            _ifrfContext = ifrfContext;
            _logger = logger;
            _resourceService = resourceService;
        }

		public ResourceOrder Get( long id)
		{
			return _ifrfContext.ResourceOrders.SingleOrDefault(a => a.Id == id);
		}

        /// <summary date="11-06-2021, 09:19:39" author="S.Deckers">
        /// Read items from db
        /// </summary>
        public IQueryable<ResourceOrder> GetItems()
        {
            _logger.Trace(string.Format("{0}.{1}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name));

            return _ifrfContext.ResourceOrders;
        }

        public bool Delete(long id)
        {
            _logger.Trace(string.Format("{0}.{1}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name));

            ResourceOrder resourceOrder = _ifrfContext.ResourceOrders.SingleOrDefault(x => x.Id == id);

            if (resourceOrder == null)
            {
                return false;
            }

            // --- Item found, remove it
            _ifrfContext.Remove(resourceOrder);
            _ifrfContext.SaveChanges();

            return true;
        }

        /// <summary date="11-06-2021, 13:00:23" author="S.Deckers">
        /// Update item
        /// </summary>		
        public ResourceOrder Patch(long id, Delta<ResourceOrder> ResourceOrder)
        {
            _logger.Trace(string.Format("{0}.{1}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name));

            ResourceOrder dbResourceOrder = _ifrfContext.ResourceOrders.SingleOrDefault(x => x.Id == id);

            if (dbResourceOrder == null)
            {
                return null;
            }



            string oStatusB = dbResourceOrder.Status;

            ResourceOrder.Patch(dbResourceOrder);

            // delete references in resource table when order gets closed
            // Set resource to modified if some of its characteristics change
            IEnumerable<Resource> dbResources = _ifrfContext.RsResources.Where(x => x.OrderId == dbResourceOrder.Id);
            if (dbResourceOrder.Status == "CL" && oStatusB != "CL")
            {
                foreach (Resource dbResource in dbResources)
                {
                    dbResource.OrderId = null;
                }
            }

            foreach (Resource dbResource in dbResources)
            {
                _resourceService.UpdateResourceMetadataFields(dbResource);
            }

            _ifrfContext.SaveChanges();

            return dbResourceOrder;
        }

        /// <summary date="11-06-2021, 13:00:23" author="S.Deckers">
        /// Create item
        /// </summary>
        public ResourceOrder Create(ResourceOrder resourceOrder)
        {
            _ifrfContext.Add(resourceOrder);

            // Set resource to modified if some of its characteristics change
            IEnumerable<Resource> dbResources = _ifrfContext.RsResources.Where(x => x.OrderId == resourceOrder.Id);

            foreach (Resource dbResource in dbResources)
            {
                _resourceService.UpdateResourceMetadataFields(dbResource);
            }
            _ifrfContext.SaveChanges();

            return resourceOrder;
        }
    }
}
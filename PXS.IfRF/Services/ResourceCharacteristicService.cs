using Microsoft.AspNet.OData;
using PXS.IfRF.Data.Model;
using PXS.IfRF.Logging;
using System;
using System.Linq;
using PXS.IfRF.AuthHandling;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace PXS.IfRF.Services
{
    public class ResourceCharacteristicService : IResourceCharacteristicService
    {
        private readonly ModelContext	_ifrfContext	= null;
		private readonly ILoggerManager _logger			= null;
        private readonly IResourceService _resourceService;

        public ResourceCharacteristicService(ModelContext ifrfContext, ILoggerManager logger, IResourceService resourceService)
        {
            _ifrfContext	= ifrfContext;
			_logger         = logger;
            _resourceService = resourceService;
        }

        /// <summary date="11-06-2021, 09:29:58" author="S.Deckers">
        /// Read item from db 
        /// </summary>
		public ResourceCharacteristic Get( long id)
		{
			return _ifrfContext.ResourceCharacteristics.SingleOrDefault(a => a.Id == id);
		}

        /// <summary date="11-06-2021, 09:19:39" author="S.Deckers">
        /// Read items from db
        /// </summary>
        public IQueryable<ResourceCharacteristic> GetItems()
        {
            return _ifrfContext.ResourceCharacteristics;
        }

        public bool Delete(long id)
        {
            ResourceCharacteristic ResourceCharacteristic = _ifrfContext.ResourceCharacteristics.SingleOrDefault(x => x.Id == id);

            if (ResourceCharacteristic == null)
            {
                return false;
            }

            // --- Item found, remove it
            _ifrfContext.Remove(ResourceCharacteristic);
            _ifrfContext.SaveChanges();

            return true;
        }

        /// <summary date="11-06-2021, 13:00:23" author="S.Deckers">
        /// Update item
        /// </summary>		
        public ResourceCharacteristic Patch(long id, Delta<ResourceCharacteristic> ResourceCharacteristic)
        {
            ResourceCharacteristic dbResourceCharacteristic = _ifrfContext.ResourceCharacteristics
                .Where(x => x.Id == id)
                .Include(ch => ch.Resource)
                .SingleOrDefault();

            if (dbResourceCharacteristic == null)
            {
                return null;
            }

            // Set resource to modified it some of its characteristics change
            //Resource dbResource = _ifrfContext.RsResources.SingleOrDefault(x => x.Id == dbResourceCharacteristic.Id);
            Resource dbResource = dbResourceCharacteristic.Resource;

            ResourceCharacteristic.Patch(dbResourceCharacteristic);

            _resourceService.UpdateResourceMetadataFields(dbResource);

            _ifrfContext.SaveChanges();

            return dbResourceCharacteristic;
        }

        /// <summary date="11-06-2021, 13:00:23" author="S.Deckers">
        /// Create item
        /// </summary>
        public ResourceCharacteristic Create(ResourceCharacteristic ResourceCharacteristic)
        {
            _ifrfContext.Add(ResourceCharacteristic);

            // Set resource to modified it some of its characteristics change
            Resource dbResource = _ifrfContext.RsResources.SingleOrDefault(x => x.Id == ResourceCharacteristic.Id);

            _resourceService.UpdateResourceMetadataFields(dbResource);

            _ifrfContext.SaveChanges();

            return ResourceCharacteristic;
        }
    }
}
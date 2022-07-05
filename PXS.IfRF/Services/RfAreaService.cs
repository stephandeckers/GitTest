using Microsoft.AspNet.OData;
using PXS.IfRF.Data;
using PXS.IfRF.Data.Model;
using System;
using System.Linq;
using PXS.IfRF.Logging;
using PXS.IfRF.AuthHandling;
using Microsoft.AspNetCore.Http;
using PXS.IfRF.BusinessLogic;

namespace PXS.IfRF.Services
{
    public class RfAreaService : IRfAreaService
    {
        private readonly ModelContext		_ifrfContext = null;
        private readonly ILoggerManager		_logger;
        private readonly IResourceService	_resourceService;
		private readonly RfAreaLogic		_businessLogic	= null;

        public RfAreaService( ModelContext ifrfContext, ILoggerManager logger, IResourceService resourceService)
        {
            _ifrfContext	= ifrfContext;
			_logger         = logger;
            _resourceService = resourceService;

			_businessLogic	= new RfAreaLogic( _ifrfContext, _logger);
        }

        /// <summary date="11-06-2021, 09:29:58" author="S.Deckers">
        /// Read item from db
        /// </summary>
   //     public IQueryable<RfArea> GetRfArea(long id)
   //     {            
			//return _ifrfContext.RfAreas.Where(a => a.Id == id);
   //     }

		public SingleResult<RfArea> Get(long id)
		{
            return SingleResult.Create(_ifrfContext.RfAreas.Where(a => a.Id == id));
		}

        /// <summary date="11-06-2021, 09:19:39" author="S.Deckers">
        /// Read items from db
        /// </summary>
        public IQueryable<RfArea> GetRfAreas()
        {
            return _ifrfContext.RfAreas;
        }

        public bool DeleteRfArea(long id)
        {
			RfArea		rfArea		= _ifrfContext.RfAreas.SingleOrDefault(x => x.Id == id);
            Resource	resource	= _ifrfContext.RsResources.SingleOrDefault(x => x.Id == id);
            if (rfArea == null && resource == null)
            {
                return false;
            }

            // --- Item found, remove it (20210611 SDE)

            _ifrfContext.Remove(rfArea);
            _ifrfContext.Remove(resource);
            _ifrfContext.SaveChanges();

            return true;
        }

        /// <summary date="11-06-2021, 13:00:23" author="S.Deckers">
        /// Update item
        /// </summary>		
   //     public RfArea PatchRfArea(long id, Delta<RfArea> rfArea)
   //     {
			//RfArea		dbRfArea = _ifrfContext.RfAreas.SingleOrDefault(x => x.Id == id);
   //         Resource	resource = _ifrfContext.RsResources.SingleOrDefault(x => x.Id == id);

   //         if (dbRfArea == null)
   //         {
   //             return null;
   //         }

   //         rfArea.Patch(dbRfArea);

   //         _resourceService.UpdateResourceMetadataFields(resource) ;

   //         _ifrfContext.SaveChanges();

   //         return dbRfArea;
   //     }

		public (BusinessLogicResult, RfArea) Patch( long id, Delta<RfArea> deltaItem)
		{
			RfArea existing = _ifrfContext.RfAreas.SingleOrDefault( x => x.Id == id);

			if( existing == null)
			{
				BusinessLogicResult blr = new BusinessLogicResult();
				blr.ErrorMessages.Add( $"item { id } not found");
				return( blr, null);
			}

			BusinessLogicResult businessLogicResult = _businessLogic.BeforeUpdate( deltaItem.GetInstance(), existing, deltaItem.GetChangedPropertyNames());
			if( businessLogicResult?.Succeeded == false)
			{
				return( businessLogicResult, existing);
			}

			deltaItem.Patch( existing);

			Resource	resource	= _ifrfContext.RsResources.SingleOrDefault( x => x.Id == id);
			_resourceService.UpdateResourceMetadataFields( resource);

			_ifrfContext.SaveChanges();
			return( businessLogicResult, existing);
		}

        /// <summary date="11-06-2021, 13:00:23" author="S.Deckers">
        /// Create item
        /// </summary>
        public RfArea CreateRfArea( RfArea rfArea)
        {
			using( var dbContextTransaction = this._ifrfContext.Database.BeginTransaction( ))
			{
				try
				{
					Resource resource = _resourceService.CreateNewResource( rfArea);
                    //making it possible to add resource info in the request
                    if (rfArea.Resource != null)
                    {
                        resource.OperationalStatus = rfArea.Resource.OperationalStatus;
                        resource.LifecycleStatus = rfArea.Resource.LifecycleStatus;
                        resource.OrderId = rfArea.Resource.OrderId;

                    }
                    rfArea.Resource = resource;

                    _ifrfContext.Add( rfArea );
					_ifrfContext.SaveChanges( );

					dbContextTransaction.Commit( );
					return( rfArea);
				}
				catch( System.Exception ex)
				{
					this._logger.Error( ex.Message);
					dbContextTransaction.Rollback( );
					throw;
				}
			}
        }
    }
}

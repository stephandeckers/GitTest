using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using PXS.IfRF.AuthHandling;
using PXS.IfRF.Data.Model;
using PXS.IfRF.Logging;
using System;
using PXS.IfRF.BusinessLogic;
using System.Linq;

namespace PXS.IfRF.Services
{
    public class PopService : IPopService
    {
        private readonly ModelContext		_ifrfContext    = null;
        private readonly ILoggerManager		_logger         = null;
		private readonly PopLogic			_popLogic	= null;
        private readonly IResourceService	_resourceService;

        public PopService
		(
			ModelContext			ifrfContext
		,	ILoggerManager			logger
		,	IHttpContextAccessor	httpContextAccessor
		,	IResourceService		resourceService
		)
        {
            _ifrfContext		= ifrfContext;
			_logger				= logger;
            _resourceService	= resourceService;

			_popLogic	= new PopLogic( _ifrfContext, httpContextAccessor, _logger);
        }

		public SingleResult<Pop> Get(long id)
		{
            return SingleResult.Create( _ifrfContext.Pops.Where(a => a.Id == id));
		}

        /// <summary date="11-06-2021, 09:19:39" author="S.Deckers">
        /// Read items from db
        /// </summary>
        public IQueryable<Pop> GetItems()
        {
            return _ifrfContext.Pops;
        }

        public bool Delete( long id)
        {
            Pop			pop			= _ifrfContext.Pops.SingleOrDefault(x => x.Id == id);
            Resource	resource	= _ifrfContext.RsResources.SingleOrDefault(x => x.Id == id);
            if (pop == null && resource == null)
            {
                return false;
            }

            // --- Item found, remove it (20210611 SDE)

            _ifrfContext.Remove(pop);
            _ifrfContext.Remove(resource);
            _ifrfContext.SaveChanges();

            return true;
        }

		public (BusinessLogicResult, Pop) Patch( long id, Delta<Pop> deltaItem)
		{
			Pop existing = _ifrfContext.Pops.SingleOrDefault( x => x.Id == id);

			if( existing == null)
			{
				BusinessLogicResult blr = new BusinessLogicResult();
				blr.ErrorMessages.Add( $"item { id } not found");
				return( blr, null);
			}

			BusinessLogicResult businessLogicResult = _popLogic.BeforeUpdate( deltaItem.GetInstance(), existing, deltaItem.GetChangedPropertyNames());
			if( businessLogicResult?.Succeeded == false)
			{
				return( businessLogicResult, existing);
			}

			deltaItem.Patch( existing);

			Resource resource = _ifrfContext.RsResources.SingleOrDefault( x => x.Id == id);
			_resourceService.UpdateResourceMetadataFields( resource);

			_ifrfContext.SaveChanges();
			return( businessLogicResult, existing);
		}

		public (BusinessLogicResult, Pop) Create ( Pop item)
		{
			BusinessLogicResult businessLogicResult = _popLogic.BeforeCreate( item);
			if( businessLogicResult?.Succeeded == false)
			{
				return( businessLogicResult, item);
			}

			using( var dbContextTransaction = this._ifrfContext.Database.BeginTransaction( ))
			{
				try
				{
					Resource resource = _resourceService.CreateNewResource(item);
					//making it possible to add resource info in the request
					if (item.Resource != null)
					{
						resource.OperationalStatus = item.Resource.OperationalStatus;
						resource.LifecycleStatus = item.Resource.LifecycleStatus;
						resource.OrderId = item.Resource.OrderId;

					}
					item.Resource = resource;

					_ifrfContext.Add( item);
					_ifrfContext.SaveChanges( );

					dbContextTransaction.Commit( );
					return ( businessLogicResult, item);
				}
				catch( Exception ex)
				{
					this._logger.Error( ex.Message);
					dbContextTransaction.Rollback( );
					throw;
				}
			}
		}
	}
}
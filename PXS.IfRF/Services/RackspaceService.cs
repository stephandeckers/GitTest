using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using PXS.IfRF.AuthHandling;
using PXS.IfRF.Data.Model;
using PXS.IfRF.Logging;
using PXS.IfRF.BusinessLogic;
using System.Linq;

namespace PXS.IfRF.Services
{
	public class RackspaceService : IRackspaceService
	{
		private readonly ModelContext		_ifrfContext = null;
		private readonly ILoggerManager		_logger = null;
		private readonly IResourceService	_resourceService;
		private readonly RfAreaLogic		_businessLogic	= null;

		public RackspaceService(ModelContext ifrfContext, ILoggerManager logger, IResourceService resourceService)
		{
			_ifrfContext = ifrfContext;
			_logger = logger;
			_resourceService = resourceService;
			_businessLogic	= new RfAreaLogic( _ifrfContext, _logger);
		}

		/// <summary date="12-07-2021, 11:07:11" author="S.Deckers">
		/// Read item from db
		/// </summary>
		//public RackSpace Get( long id)
		//{
		//	return _ifrfContext.RackSpaces.SingleOrDefault(a => a.Id == id);
		//}

		public SingleResult<RackSpace> Get(long id)
		{
            return SingleResult.Create(_ifrfContext.RackSpaces.Where(a => a.Id == id));
		}

		/// <summary date="11-06-2021, 09:19:39" author="S.Deckers">
		/// Read items from db
		/// </summary>
		public IQueryable<RackSpace> GetItems()
		{
			return _ifrfContext.RackSpaces;
		}

		public bool Delete(long id)
		{
			RackSpace rackspace = _ifrfContext.RackSpaces.SingleOrDefault(x => x.Id == id);
			Resource resource = _ifrfContext.RsResources.SingleOrDefault(x => x.Id == id);
			if (rackspace == null && resource == null)
			{
				return false;
			}

			// --- Item found, remove it (20210611 SDE)

			_ifrfContext.Remove(rackspace);
			_ifrfContext.Remove(resource);
			_ifrfContext.SaveChanges();

			return true;
		}

		/// <summary date="11-06-2021, 13:00:23" author="S.Deckers">
		/// Update item
		/// </summary>	
		public (BusinessLogicResult, RackSpace) Patch( long id, Delta<RackSpace> deltaItem)
		{
			RackSpace existing = _ifrfContext.RackSpaces.SingleOrDefault( x => x.Id == id);

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

		//public RackSpace Patch(long id, Delta<RackSpace> rackspace)
		//{
		//	RackSpace dbRackspace = _ifrfContext.RackSpaces.SingleOrDefault(x => x.Id == id);
		//	Resource resource = _ifrfContext.RsResources.SingleOrDefault(x => x.Id == id);

		//	if (rackspace == null)
		//	{
		//		return null;
		//	}

		//	rackspace.Patch(dbRackspace);
		//	_resourceService.UpdateResourceMetadataFields(resource);

		//	_ifrfContext.SaveChanges();
		//	return dbRackspace;
		//}

		/// <summary date="11-06-2021, 13:00:23" author="S.Deckers">
		/// Create item
		/// </summary>
		public RackSpace Create(RackSpace item)
		{
			using (var dbContextTransaction = this._ifrfContext.Database.BeginTransaction())
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

					_ifrfContext.Add(item);
					_ifrfContext.SaveChanges();

					dbContextTransaction.Commit();
					return (item);
				}
				catch (System.Exception ex)
				{
					this._logger.Error(ex.Message);
					dbContextTransaction.Rollback();
					throw;
				}
				finally
				{
				}
			}
		}
	}
}

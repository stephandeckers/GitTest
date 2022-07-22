using System.Collections.Generic;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using PXS.IfRF.AuthHandling;
using PXS.IfRF.BusinessLogic;
using PXS.IfRF.Data.Model;
using PXS.IfRF.Logging;
using PXS.IfRF.Supporting;
using System.Linq;
using d=System.Diagnostics.Debug;

namespace PXS.IfRF.Services
{
	public class SubrackService : ISubrackService
	{
		private readonly ModelContext			_ifrfContext	= null;
		private readonly ILoggerManager			_logger			= null;
		private readonly SubrackLogic			_subrackLogic;
		private readonly IResourceService		_resourceService;
		private readonly IHttpContextAccessor	_httpContextAccessor;

		public SubrackService
		(
			ModelContext			ifrfContext
		,	ILoggerManager			logger
		,	IResourceService		resourceService
		,	IHttpContextAccessor	httpContextAccessor
		,	ISettingsService		settingsService
		)
		{
			_ifrfContext			= ifrfContext;
			_logger					= logger;
			_subrackLogic			= new SubrackLogic(_ifrfContext, _logger, settingsService);
			_resourceService		= resourceService;
			_httpContextAccessor	= httpContextAccessor;
		}

		public SingleResult<Subrack> Get( long id)
		{
			return SingleResult.Create(_ifrfContext.Subracks.Where(a => a.Id == id));
		}

		/// <summary date="11-06-2021, 09:19:39" author="S.Deckers">
		/// Read items from db
		/// </summary>
		public IQueryable<Subrack> GetItems( )
		{
			return _ifrfContext.Subracks;
		}

		public BusinessLogicResult Delete( long id )
		{
			Subrack		subrack		= _ifrfContext.Subracks.Where(x => x.Id == id)
				.Include(sr => sr.Resource)
				.SingleOrDefault();
			Resource	resource	= subrack?.Resource;

			if (subrack == null && resource == null)
			{
				return new BusinessLogicResult();
			}

			IQueryable<SrPosition> srPositions = _ifrfContext.SrPositions.Where(p => p.SubrackId == id)
				.Include(p => p.ConnectionFroms)
				.Include(p => p.ConnectionTos);

			subrack.SrPositions = srPositions.ToList();

			// --- Item found, remove it (20210611 SDE)
			BusinessLogicResult validationResult = _subrackLogic.ValidateDelete(subrack);
			if (validationResult.Succeeded)
			{
				_ifrfContext.Remove(subrack);
				_ifrfContext.Remove(resource);
				_subrackLogic.AfterDelete(subrack);
				_ifrfContext.SaveChanges();
			}
			return validationResult;
		}

		/// <summary date="11-06-2021, 13:00:23" author="S.Deckers">
		/// Update item
		/// </summary>		
		public Subrack Patch( long id, Delta<Subrack> subrack)
		{
			Subrack		dbSubrack	= _ifrfContext.Subracks.SingleOrDefault( x => x.Id == id );
			Resource	resource	= _ifrfContext.RsResources.SingleOrDefault( x => x.Id == id );

			if(subrack == null)
			{
				return null;
			}

			subrack.Patch( dbSubrack);
			_resourceService.UpdateResourceMetadataFields(resource);

			_ifrfContext.SaveChanges( );
			return dbSubrack;
		}

		/// <summary date="11-06-2021, 13:00:23" author="S.Deckers">
		/// Create item
		/// </summary>
		public Subrack Create( Subrack item)
		{
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

					_subrackLogic.AfterCreate( item);

					_ifrfContext.Add( item );
					_ifrfContext.SaveChanges( );

					if( item.SpecId.HasValue)
					{
						HttpContext context = _httpContextAccessor.HttpContext;
						//string		theUser = (string)(context.Items["iv-user"]);

						string theUser = _httpContextAccessor.HttpContext.Request.Headers.getUsername( );
						_subrackLogic.CreateMetadataPositions( item.Id, item.SpecId.Value, theUser);
					}

					dbContextTransaction.Commit( );
					return( item);
				}
				catch( System.Exception ex)
				{
					this._logger.Error( ex.Message);
					dbContextTransaction.Rollback( );
					throw;
				}
				finally
				{
				}
			}
		}

		/// <summary>
		/// Creates the custom positions.
		/// </summary>
		/// <param name="customPositionRequest">The custom position request.</param>
		/// <returns></returns>
		public List<SrPosition>	CreateCustomPositions( CustomPositionRequest r)
		{
			using( var dbContextTransaction = this._ifrfContext.Database.BeginTransaction( ))
			{
				try
				{
					long theSubrack = r.SubrackId;
					long theSpecid	= r.PositionSpecId;

					HttpContext context = _httpContextAccessor.HttpContext;
					string		theUser = (string)(context.Items["iv-user"]);
					_subrackLogic.CreateCustomPositions( v_sr_id:r.SubrackId, v_pos_spec_id:r.PositionSpecId, v_group:r.Group, v_operational_status:r.OperationalStatus, v_created_by:theUser);

					dbContextTransaction.Commit( );

					List<SrPosition> items = this._ifrfContext.SrPositions.Where( x => x.SubrackId == r.SubrackId && x.SpecId == r.PositionSpecId).ToList();
					return( items);
				}
				catch( System.Exception ex)
				{
					this._logger.Error( ex.Message);
					dbContextTransaction.Rollback( );
					throw;
				}
				finally
				{
				}
			}
		}
	}
}
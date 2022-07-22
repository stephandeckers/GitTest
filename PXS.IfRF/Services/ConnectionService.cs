/**
 * @Name RsConnectionService.cs
 * @Purpose 
 * @Date 15 July 2021, 21:58:53
 * @Author S.Deckers
 * @Description 
 */

namespace PXS.IfRF.Services
{
	#region -- Using directives --
	using Microsoft.AspNet.OData;
	using PXS.IfRF.Data.Model;
	using System.Linq;
	using PXS.IfRF.Logging;
    using PXS.IfRF.AuthHandling;
    using Microsoft.AspNetCore.Http;
	using PXS.IfRF.BusinessLogic;
    using System;
	using d=System.Diagnostics.Debug;
	using Microsoft.EntityFrameworkCore;
	#endregion

	public class ConnectionService : IConnectionService
	{
		private readonly ModelContext   _ifrfContext    = null;
		private readonly ILoggerManager _logger         = null;
		private readonly ConnectionLogic _connectionLogic;
		private readonly IResourceService _resourceService;

		public ConnectionService
		(
			ModelContext		ifrfContext
		,	ILoggerManager		logger
		,	IResourceService	resourceService
		,	ISettingsService	settingsService
		)
		{
			_ifrfContext		= ifrfContext;
			_logger				= logger;
			_resourceService	= resourceService;
			_connectionLogic	= new ConnectionLogic( _ifrfContext, _logger, settingsService);
		}

		public SingleResult<Connection> Get( long id)
		{
			var items = _ifrfContext.Connections.Where(a => a.Id == id)
				.Include( c => c.To.Subrack.RackSpace)
				.Include( c => c.From.Subrack.RackSpace);

			Connection item = items.FirstOrDefault();

			item.CustomerReference = SharedMethods.getCustomerReference( item);
			return SingleResult.Create( items);
		}

		/// <summary date="11-06-2021, 09:19:39" author="S.Deckers">
		/// Read items from db
		/// </summary>
		public IQueryable<Connection> GetItems( )
		{
			return( _ifrfContext.Connections);
		}

		public (BusinessLogicResult, bool) Delete( long id)
		{
			Connection	item		= _ifrfContext.Connections.SingleOrDefault( x => x.Id == id );
			Resource	resource	= _ifrfContext.RsResources.SingleOrDefault( x => x.Id == id );
			if( item == null && resource == null)
			{
				BusinessLogicResult blr = new BusinessLogicResult();
				blr.ErrorMessages.Add($"item { id } not found");
				return (blr, false);
			}

			// --- Item found, remove it (20210611 SDE)

			_ifrfContext.Remove( item);
			_ifrfContext.Remove( resource);
			BusinessLogicResult businessLogicResult = _connectionLogic.AfterDelete(item);
			
			_ifrfContext.SaveChanges();
			return (businessLogicResult, true);
		}

		public (BusinessLogicResult, Connection)	Patch( long id, Delta<Connection> deltaItem)
		{
			Connection existing = _ifrfContext.Connections.SingleOrDefault( x => x.Id == id);

			if( existing == null)
			{
				BusinessLogicResult blr = new BusinessLogicResult();
				blr.ErrorMessages.Add( $"item { id } not found");
				return( blr, null);
			}

			BusinessLogicResult businessLogicResult = _connectionLogic.BeforeUpdate( deltaItem.GetInstance(), existing, deltaItem.GetChangedPropertyNames());
			if( businessLogicResult?.Succeeded == false)
			{
				return( businessLogicResult, existing);
			}

			deltaItem.Patch( existing);
			_connectionLogic.AfterUpdate(existing, deltaItem.GetInstance(), deltaItem.GetChangedPropertyNames());

			Resource	resource	= _ifrfContext.RsResources.SingleOrDefault( x => x.Id == id);
			_resourceService.UpdateResourceMetadataFields( resource);

			_ifrfContext.SaveChanges();
			return( businessLogicResult, existing);
		}

		/// <summary>
		/// Creates the specified item.
		/// </summary>
		/// <param name="item">The item.</param>
		/// <returns></returns>
		public (BusinessLogicResult, Connection) Create( Connection item)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			BusinessLogicResult businessLogicResult = _connectionLogic.BeforeCreate( item);
			if( businessLogicResult?.Succeeded == false)
			{
				return( businessLogicResult, item);
			}

			using( var dbContextTransaction = this._ifrfContext.Database.BeginTransaction( ))
			{
				try
				{
					Resource resource = _resourceService.CreateNewResource( item);
					//making it possible to add resource info in the request
					if (item.Resource != null)
					{
						resource.OperationalStatus = item.Resource.OperationalStatus;
						resource.LifecycleStatus = item.Resource.LifecycleStatus;
						resource.OrderId = item.Resource.OrderId;
					}

					item.Resource = resource;

					_connectionLogic.AfterCreate(item);

					_ifrfContext.Add( item );
					_ifrfContext.SaveChanges( );

					dbContextTransaction.Commit( );
					return ( businessLogicResult, item);
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
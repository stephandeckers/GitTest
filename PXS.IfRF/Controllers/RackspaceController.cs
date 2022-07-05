using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PXS.IfRF.BusinessLogic;

namespace PXS.IfRF.Controllers
{
	#region -- Using directives --
	using System;
	using System.Text;
	using Microsoft.AspNetCore.Mvc;
	using Swashbuckle.AspNetCore.Annotations;
	using Microsoft.AspNet.OData;
	using PXS.IfRF.Services;
	using System.Collections.Generic;
	using PXS.IfRF.Logging;
	using PXS.IfRF.Data.Model;
	using PXS.IfRF.ErrorHandling;
	using System.Linq;
	using d = System.Diagnostics.Debug;
	using Microsoft.AspNet.OData.Routing;
	using Microsoft.Extensions.Logging;
	using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Authorization;
    #endregion

    public class RackspaceController : IfRFBaseController
	{
		private const string swaggerControllerDescription = "Rackspaces";
		private readonly IRackspaceService _rackspaceService;

		public RackspaceController
		(
			IRackspaceService rackspaceService, ILoggerManager logger, IRefdataService refdataService
		)
			: base(logger, refdataService)
		{
			_rackspaceService = rackspaceService;
		}

		[ HttpGet()]
		[ EnableQuery(MaxExpansionDepth = 4)]
		[ SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve list of Rackspaces")]
		public IQueryable<RackSpace> GetItems()
		{
			var items = _rackspaceService.GetItems();

			return (items);
		}

		/// <summary>
		/// Gets the specified identifier.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		//[HttpGet("{id:long}")]
		//[EnableQuery(MaxExpansionDepth = 4)]
		//[SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve single Rackspace")]
		//public ActionResult<RackSpace> Get( long id )
		//{
		//	RackSpace item = _rackspaceService.Get( id );
		//	if (item == null)
		//	{
		//			return Ok( new OperationFailureResult( $"Rackspace with id {id} not found" ) );
		//	}

		//	return Ok( item );
		//}

		[HttpGet("{id:long}")]
		[EnableQuery(MaxExpansionDepth = 4)]
		[SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve single Rackspace")]
		public ActionResult<RackSpace> Get( long id )
		{
			SingleResult<RackSpace> item = _rackspaceService.Get(id);
			//if (item == null)
			if( item.Queryable.Count() == 0)
			{
				return Ok(new OperationFailureResult($"Rackspace with id {id} not found"));
			}

			return Ok(item);
		}

		/// <summary>
		/// Creates the specified item.
		/// </summary>
		/// <param name="pop">The item.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		[HttpPost()]
		[Authorize(AuthenticationSchemes = AUTHSCHEME, Policy = EDITORROLE_lvl_2)]
		[SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Create single Rackspace")]
		public ActionResult<RackSpace> Create([FromBody] RackSpace rackspace)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			RackSpace createdRackspace = _rackspaceService.Create(rackspace);
			return Ok(createdRackspace);
		}

		/// <summary>
		/// Updates the specified Pop.
		/// </summary>
		/// 
		/// <notes>Only attributes to be changed are posted</notes>
		/// <param name="item">item</param>
		/// <returns></returns>
		[HttpPatch()]
		[Authorize(AuthenticationSchemes = AUTHSCHEME, Policy = EDITORROLE_lvl_2)]
		[SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Update a specific Rackspace")]
		public ActionResult<RackSpace> Patch(long id, [FromBody] Delta<RackSpace> item)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			( BusinessLogicResult res, RackSpace patched) = _rackspaceService.Patch( id, item);
			if( res.Succeeded)
			{
				return Ok( patched);
			}

			 return Ok(new OperationFailureResult(res.ErrorMessages));
		}

		//public ActionResult<RackSpace> Patch(long id, [FromBody] Delta<RackSpace> item)
		//{
		//	if (!ModelState.IsValid)
		//	{
		//		return BadRequest(ModelState);
		//	}

		//	RackSpace patched = _rackspaceService.Patch(id, item);
		//	if( patched != null)
		//	{
		//		return Ok( patched);
		//	}

		//	BusinessLogicResult res = new BusinessLogicResult( );
		//	res.ErrorMessages.Add( $"Rackspace with id {id} not found");

		//	return Ok(new OperationFailureResult(res.ErrorMessages));
		//}

		/// <summary>
		/// Deletes the specified identifier.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns></returns>
		[HttpDelete("{id:int}")]
		[Authorize(AuthenticationSchemes = AUTHSCHEME, Policy = EDITORROLE_lvl_2)]
		[SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Delete a Rackspace")]
		public IActionResult Delete(long id)
		{
			if (!_rackspaceService.Delete(id))
			{
				return Ok( new OperationFailureResult( $"Error deleting Rackspace with id {id}") );
			}

			return Ok();
		}
	}
}
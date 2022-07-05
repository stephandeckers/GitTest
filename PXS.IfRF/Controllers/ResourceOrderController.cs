using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
    public class ResourceOrderController : IfRFBaseController
	{
		private const string swaggerControllerDescription = "Resource Orders";
		private readonly IResourceOrderService _resourceOrderService;

		public ResourceOrderController
		(
			IResourceOrderService resourceOrderService, ILoggerManager logger, IRefdataService refdataService
		)
			: base(logger, refdataService)
		{
			_resourceOrderService = resourceOrderService;
		}

		[HttpGet()]
		[EnableQuery()]
		[SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve list of Resource Orders")]
		public IQueryable<ResourceOrder> GetItems()
		{
			var items = _resourceOrderService.GetItems();

			return (items);
		}

		/// <summary>
		/// Gets the specified identifier.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		[HttpGet("{id:long}")]
		[EnableQuery(MaxExpansionDepth = 4)]
		[SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve Single Resource Order")]
		public ActionResult<Pop> Get( long id )
		{
			ResourceOrder item = _resourceOrderService.Get( id );
			if (item == null)
			{
					return Ok( new OperationFailureResult( $"Resource order with id {id} not found" ) );
			}

			return Ok( item );
		}

		/// <summary>
		/// Creates the specified item.
		/// </summary>
		/// <param name="pop">The item.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		[HttpPost()]
		[Authorize(AuthenticationSchemes = AUTHSCHEME, Policy = EDITORROLE_lvl_2)]
		[SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Create single Resource Order")]
		public ActionResult<ResourceOrder> Create([FromBody] ResourceOrder resourceOrder)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			ResourceOrder createdResourceOrder = _resourceOrderService.Create(resourceOrder);
			return Ok(createdResourceOrder);
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
		[SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Update a specific Resource Order")]
		public ActionResult<ResourceOrder> Patch(long id, [FromBody] Delta<ResourceOrder> item)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			ResourceOrder patched = _resourceOrderService.Patch(id, item);

			if (patched == null)
			{
				return Ok( new OperationFailureResult( $"Resource order with id {id} not found"));
			}

			return Ok(patched);
		}

		/// <summary>
		/// Deletes the specified identifier.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns></returns>
		[HttpDelete("{id:int}")]
		[Authorize(AuthenticationSchemes = AUTHSCHEME, Policy = EDITORROLE_lvl_2)]
		[SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Delete a Resource Order")]
		public IActionResult Delete(long id)
		{
			if (!_resourceOrderService.Delete(id))
			{
				return Ok( new OperationFailureResult( $"Error deleting Resource order with id {id}"));
			}

			return Ok();
		}
	}
}
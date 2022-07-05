/**
* @Name PopController.cs
* @Purpose 
* @Date 04 June 2021, 13:23:04
* @Author S.Deckers
* @Description 
*/

namespace PXS.IfRF.Controllers
{
	#region -- Using directives --
	using System;
	using System.Linq;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNet.OData;
	using Swashbuckle.AspNetCore.Annotations;
	using PXS.IfRF.Services;
	using PXS.IfRF.Logging;
	using PXS.IfRF.BusinessLogic;
	using PXS.IfRF.Data.Model;
	using Microsoft.AspNetCore.Http;
	using Microsoft.AspNetCore.Authorization;
	#endregion

	public partial class PopController : IfRFBaseController
	{
		private const string swaggerControllerDescription = "Points of Presence";
		private readonly IPopService _popService;

		public PopController(
			IPopService popService,
			ILoggerManager logger,
			IRefdataService refdataService )
			: base(logger, refdataService)
		{
			_popService = popService;
		}

		[HttpGet]
		[EnableQuery(MaxExpansionDepth = 4)]
		[SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve list of Pops")]
		public IQueryable<Pop> GetItems()
		{
			var localItems = _popService.GetItems();
			return localItems;
		}

		[HttpGet("{id:long}")]
		[EnableQuery(MaxExpansionDepth = 4)]
		[SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve single Pop")]
		public ActionResult<Pop> Get( long id )
		{
			SingleResult<Pop> item = _popService.Get(id);
			if( item.Queryable.Count() == 0)
			{
				return Ok(new OperationFailureResult($"Pop with id {id} not found"));
			}

			return Ok(item);
		}

		[HttpPost()]
		[Authorize(AuthenticationSchemes = AUTHSCHEME, Policy = EDITORROLE_lvl_2)]
		[SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Create single Pop")]
		public ActionResult<Pop> Create([FromBody] Pop item)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			BusinessLogicResult res;
			(res, item) = _popService.Create( item);
			if(res.Succeeded)
			{
				return Ok( item);
			}

			return Ok(new OperationFailureResult(res.ErrorMessages));
		}

		[ HttpPatch()]
		[ Authorize(AuthenticationSchemes = AUTHSCHEME, Policy = EDITORROLE_lvl_2)]
		[ SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Update a specific Pop")]
		public ActionResult<Pop> Patch(long id, [FromBody] Delta<Pop> item)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			( BusinessLogicResult res, Pop patched) = _popService.Patch( id, item);
			if( res.Succeeded)
			{
				return Ok( patched);
			}

			 return Ok(new OperationFailureResult(res.ErrorMessages));
		}

		/// <summary>
		/// Deletes the specified identifier.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns></returns>
		[HttpDelete("{id:int}")]
		[Authorize(AuthenticationSchemes = AUTHSCHEME, Policy = EDITORROLE_lvl_2)]
		[SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Delete a Pop")]
		public IActionResult Delete(long id)
		{
			if (!_popService.Delete(id))
			{
				return Ok( new OperationFailureResult( $"Error deleting Pop with id {id}") );
			}

			return Ok();
		}
	}
}
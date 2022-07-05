/**
 * @Name SrPositionController.cs
 * @Purpose 
 * @Date 27 July 2021, 19:43:45
 * @Author S.Deckers
 * @Description 
 */

namespace PXS.IfRF.Controllers
{
	#region -- Using directives --
	using System;
	using Microsoft.AspNetCore.Mvc;
	using Swashbuckle.AspNetCore.Annotations;
	using Microsoft.AspNet.OData;
	using PXS.IfRF.Services;
	using System.Collections.Generic;
	using PXS.IfRF.Logging;
	using PXS.IfRF.Data.Model;
	using PXS.IfRF.BusinessLogic;
	using PXS.IfRF.ErrorHandling;
	using System.Linq;
	using Microsoft.AspNetCore.Http;
	using d = System.Diagnostics.Debug;
	using Microsoft.AspNetCore.Authorization;
	#endregion

	public partial class SrPositionController : IfRFBaseController
	{
		private const string swaggerControllerDescription = "SrPositions";
		private readonly ISrPositionService _srPositionService;

		public SrPositionController
		(
			ISrPositionService	srPositionService
			, ILoggerManager	logger
			, IRefdataService	refdataService
		)
			: base(logger, refdataService)
		{
			_srPositionService = srPositionService;
		}

		/// <summary>
		/// Gets the specified identifier.
		/// </summary>
		/// <param name="id">The identifier.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		[HttpGet("{id:long}")]
		[EnableQuery(MaxExpansionDepth = 4)]
		[SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve single SrPosition")]
		public ActionResult<SrPosition> Get(long id)
		{
			SingleResult<SrPosition> item = _srPositionService.Get(id);
			if( item.Queryable.Count() == 0)
			{
				return Ok(new OperationFailureResult($"SrPosition with id {id} not found"));
			}

			return Ok(item);
		}

		[HttpGet()]
		[EnableQuery(MaxExpansionDepth = 4)]
		[SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve list of SrPositions")]
		public IQueryable<SrPosition> GetItems()
		{
			var localItems = _srPositionService.GetItems();
			return (localItems);
		}

		[HttpGet("getbysubrackID/{subrackid:long}")]
		[SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Fetch positions and related positions for subrack")]
		public IQueryable<PositionConnectionSearchResponse> GetPositionsConnected(long subrackid)
		{
			List<PositionConnectionSearchResponse> allItems = _srPositionService.GetPositionsConnected(subrackid);

			return (allItems.AsQueryable());
		}

		/// <summary>
		/// Creates the specified item.
		/// </summary>
		/// <param name="pop">The item.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		[ HttpPost()]
		[ Authorize(AuthenticationSchemes = AUTHSCHEME, Policy = EDITORROLE_lvl_2)]
		[ SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Create single SrPosition")]
		public ActionResult<SrPosition> Create([FromBody] SrPosition item)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			BusinessLogicResult res;
			(res, item) = _srPositionService.Create( item);
			if(res.Succeeded)
			{
				return Ok( item);
			}

			return Ok(new OperationFailureResult(res.ErrorMessages));
		}

		/// <summary>
		/// Updates the specified SrPosition.
		/// </summary>
		/// 
		/// <notes>Only attributes to be changed are posted</notes>
		/// <param name="item">item</param>
		/// <returns></returns>
		[ HttpPatch()]
		[ Authorize(AuthenticationSchemes = AUTHSCHEME, Policy = EDITORROLE_lvl_2)]
		[ SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Update a specific SrPosition")]
		public ActionResult<SrPosition> Patch(long id, [FromBody] Delta<SrPosition> item)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			( BusinessLogicResult res, SrPosition patched) = _srPositionService.Patch( id, item);
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
		[SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Delete a SrPosition")]
		public IActionResult Delete(long id)
		{
			BusinessLogicResult res = _srPositionService.Delete(id);
			if (!res.Succeeded)
			{
				return Ok(new OperationFailureResult(res.ErrorMessages));
			}

			return Ok();
		}
	}
}
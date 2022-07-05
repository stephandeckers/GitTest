 /**
 * @Name RfaController.cs
 * @Purpose 
 * @Date 04 June 2021, 13:23:04
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
    using PXS.IfRF.Logging;
    using PXS.IfRF.Data.Model;
    using System.Linq;
	using PXS.IfRF.BusinessLogic;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Authorization;
    #endregion

    public partial class RfAreaController : IfRFBaseController
    {
        private const string swaggerControllerDescription = "Rented Fiber Areas";
        private readonly IRfAreaService _rfAreaService;

        public RfAreaController(
            IRfAreaService rfAreaService, 
            ILoggerManager logger,
            IRefdataService refdataService )
            :base(logger, refdataService)
        {
            _rfAreaService  = rfAreaService;
        }

        /// <summary date="09-06-2021, 12:44:39" author="S.Deckers">
        /// Gets the specified area code.
        /// </summary>
        /// <param name="areaCode">The area code.</param> JST: remove this, we use OData to filter
        /// <param name="areaName">Name of the area.</param> JST: remove this, we use OData to filter
        /// <returns></returns>
        [HttpGet]
        [EnableQuery(MaxExpansionDepth = 4)]
        [SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve list of Rented Fiber Areas")]
        public IQueryable<RfArea> GetRfAreas()
        {
            var localItems = _rfAreaService.GetRfAreas();
            return localItems;
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
    //    [HttpGet("{id:long}")]
    //    [EnableQuery(MaxExpansionDepth = 4)]
    //    [SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve single Renter Fiber Area")]
    //    public ActionResult<RfArea> GetRfArea(long id)
    //    {
    //        IQueryable<RfArea> item = _rfAreaService.GetRfArea(id);

    //        if (item == null)
    //        {
				//return Ok( new OperationFailureResult( $"Item with id {id} not found"));
    //        }

    //        return Ok(SingleResult.Create(item));
    //    }

        [HttpGet("{id:long}")]
        [EnableQuery(MaxExpansionDepth = 4)]
        [SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve single Rented Fiber Area")]
        public ActionResult<RfArea> GetRfArea(long id)
        {
			SingleResult<RfArea> item = _rfAreaService.Get(id);
			//if (item == null)
			if( item.Queryable.Count() == 0)
			{
				return Ok(new OperationFailureResult($"Rented Fiber Area with id {id} not found"));
			}

			return Ok(item);
        }

        /// <summary>
        /// Creates the specified item.
        /// </summary>
        /// <param name="rfArea">The item.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        [HttpPost()]
        [Authorize(AuthenticationSchemes = AUTHSCHEME, Policy = EDITORROLE_lvl_2)]
        [SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Create single Rented Fiber Area")]
		public ActionResult<RfArea> CreateRfArea([FromBody] RfArea rfArea)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            RfArea createdArea = _rfAreaService.CreateRfArea(rfArea);
            return Ok(createdArea);
        }

        /// <summary>
        /// Updates the specified RfArea.
        /// </summary>
        /// 
        /// <notes>Only attributes to be changed are posted</notes>
        /// <param name="item">item</param>
        /// <returns></returns>
        [HttpPatch()]
        [Authorize(AuthenticationSchemes = AUTHSCHEME, Policy = EDITORROLE_lvl_2)]
        [SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Update a specific Rented Fiber Area")]
		public ActionResult<RfArea> Patch(long id, [FromBody] Delta<RfArea> item)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			( BusinessLogicResult res, RfArea patched) = _rfAreaService.Patch( id, item);
			if( res.Succeeded)
			{
				return Ok( patched);
			}

			 return Ok(new OperationFailureResult(res.ErrorMessages));
		}

		//public ActionResult<SrPosition> Patch(long id, [FromBody] Delta<SrPosition> item)
		//{
		//	if (!ModelState.IsValid)
		//	{
		//		return BadRequest(ModelState);
		//	}

		//	( BusinessLogicResult res, SrPosition patched) = _srPositionService.Patch( id, item);
		//	if( res.Succeeded)
		//	{
		//		return Ok( patched);
		//	}

		//	 return Ok(new OperationFailureResult(res.ErrorMessages));
		//}

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        [Authorize(AuthenticationSchemes = AUTHSCHEME, Policy = EDITORROLE_lvl_2)]
        [SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Delete a Rented Fiber Area")]
        public IActionResult DeleteRfArea(long id)
        {
            if (!_rfAreaService.DeleteRfArea(id))
            {
				return Ok( new OperationFailureResult( $"Error deleting item with id {id}"));
            }

            return Ok();
        }
    }
}
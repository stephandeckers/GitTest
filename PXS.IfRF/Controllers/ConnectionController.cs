/**
 * @Name ConnectionController.cs
 * @Purpose 
 * @Date 15 July 2021, 23:38:39
 * @Author S.Deckers
 * @Description 
 */

namespace PXS.IfRF.Controllers
{
    #region -- Using directives --
    using System;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Swashbuckle.AspNetCore.Annotations;
    using Microsoft.AspNet.OData;
    using PXS.IfRF.Services;
    using System.Collections.Generic;
    using PXS.IfRF.Logging;
    using PXS.IfRF.Data.Model;
	using PXS.IfRF.BusinessLogic;
    using PXS.IfRF.ErrorHandling;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using d = System.Diagnostics.Debug;
    #endregion

    public partial class ConnectionController : IfRFBaseController
    {
        private const string swaggerControllerDescription = "Connections";
        private readonly IConnectionService _connectionService;

        public ConnectionController
        (
            IConnectionService connectionService
            , ILoggerManager logger
            , IRefdataService refdataService

        ) : base(logger, refdataService)
        {
            _connectionService = connectionService;
        }

		[HttpGet("{id:long}")]
		[EnableQuery(MaxExpansionDepth = 4)]
		[SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve single Connection")]
		public ActionResult<Connection> Get(long id)
		{
			SingleResult<Connection> item = _connectionService.Get(id);
			if( item.Queryable.Count() == 0)
			{
				return Ok(new OperationFailureResult($"Connection with id {id} not found"));
			}

			return Ok(item);
		}

        [HttpGet()]
        [EnableQuery(MaxExpansionDepth = 4)]
        [SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve list of Connections")]
        public IQueryable<Connection> GetItems()
        {
            var localItems = _connectionService.GetItems();
            return (localItems);
        }

		/// <summary>
		/// Creates the specified item.
		/// </summary>
		/// <param name="pop">The item.</param>
		/// <returns></returns>
		/// <exception cref="ArgumentException"></exception>
		[HttpPost()]
		[Authorize(AuthenticationSchemes = AUTHSCHEME, Policy = EDITORROLE_lvl_1)]
		[SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Create single Connection")]
		public ActionResult<Connection> Create([FromBody] Connection item)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			BusinessLogicResult res;
			(res, item) = _connectionService.Create( item);
			if(res.Succeeded)
			{
				return Ok( item);
			}

			return Ok(new OperationFailureResult(res.ErrorMessages));
		}

		[ HttpPatch()]
		[ Authorize(AuthenticationSchemes = AUTHSCHEME, Policy = EDITORROLE_lvl_1)]
		[ SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Update a specific Connection")]
		public ActionResult<Connection> Patch(long id, [FromBody] Delta<Connection> item)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			( BusinessLogicResult res, Connection patched) = _connectionService.Patch( id, item);
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
        [Authorize(AuthenticationSchemes = AUTHSCHEME, Policy = EDITORROLE_lvl_1)]
        [SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Delete a Connection")]
        public IActionResult Delete(long id)
        {
            (BusinessLogicResult res, bool deleted) = _connectionService.Delete(id);
            if (res.Succeeded)
            {
                return Ok();
            }

            return Ok(new OperationFailureResult(res.ErrorMessages));
        }
    }
}
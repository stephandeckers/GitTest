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
    using PXS.IfRF.ErrorHandling;
    using System.Linq;
    using Microsoft.AspNetCore.Authorization;
    #endregion

    public partial class ResourceCharacteristicController : IfRFBaseController
    {
        private const string swaggerControllerDescription = "ResourceCharacteristics";
        private readonly IResourceCharacteristicService _ResourceCharacteristicService;
        public ResourceCharacteristicController(
            IResourceCharacteristicService ResourceCharacteristicService, 
            ILoggerManager logger,
            IRefdataService refdataService)
            :base (logger, refdataService)
        {
            _ResourceCharacteristicService = ResourceCharacteristicService;
        }

        [HttpGet]
        [EnableQuery(MaxExpansionDepth = 4)]
        [SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve list of ResourceCharacteristics")]
        public IQueryable<ResourceCharacteristic> GetItems()
        {
            var localItems = _ResourceCharacteristicService.GetItems();
            return localItems;
        }

        /// <summary>
        /// Gets the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        [HttpGet("{id:long}")]
        [EnableQuery(MaxExpansionDepth = 4)]
        [SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve single ResourceCharacteristic")]
		public ActionResult<ResourceCharacteristic> Get( long id )
		{
			ResourceCharacteristic item = _ResourceCharacteristicService.Get( id );
			if (item == null)
			{
					return Ok( new OperationFailureResult( $"ResourceCharacteristic with id {id} not found" ) );
			}

			return Ok( item );
		}

        /// <summary>
        /// Creates the specified item.
        /// </summary>
        /// <param name="ResourceCharacteristic">The item.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        [HttpPost()]
        [Authorize(AuthenticationSchemes = AUTHSCHEME, Policy = EDITORROLE_lvl_2)]
        [SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Create single ResourceCharacteristic")]
        public IActionResult Create([FromBody] ResourceCharacteristic ResourceCharacteristic)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ResourceCharacteristic createdResourceCharacteristic = _ResourceCharacteristicService.Create(ResourceCharacteristic);
            return Ok(createdResourceCharacteristic);
        }

        /// <summary>
        /// Updates the specified ResourceCharacteristic.
        /// </summary>
        /// 
        /// <notes>Only attributes to be changed are posted</notes>
        /// <param name="item">item</param>
        /// <returns></returns>
        [HttpPatch()]
        [Authorize(AuthenticationSchemes = AUTHSCHEME, Policy = EDITORROLE_lvl_2)]
        [SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Update a specific ResourceCharacteristic")]
        public ActionResult<ResourceCharacteristic> Patch(long id, [FromBody] Delta<ResourceCharacteristic> item)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            ResourceCharacteristic patchedResourceCharacteristic = _ResourceCharacteristicService.Patch(id, item);

            if (patchedResourceCharacteristic == null)
            {
				return Ok( new OperationFailureResult( $"ResourceCharacteristic with id {id} not found") );
            }

            return Ok(patchedResourceCharacteristic);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        [Authorize(AuthenticationSchemes = AUTHSCHEME, Policy = EDITORROLE_lvl_2)]
        [SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Delete a ResourceCharacteristic")]
        public IActionResult DeleteResourceCharacteristic(long id)
        {
            if (!_ResourceCharacteristicService.Delete(id))
            {
				return Ok( new OperationFailureResult( $"Error deleting ResourceCharacteristic with id {id}") );
            }

            return Ok();
        }
    }
}
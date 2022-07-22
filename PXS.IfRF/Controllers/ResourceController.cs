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
    using System.Threading.Tasks;
    using Microsoft.AspNet.OData.Routing;
    using Microsoft.AspNetCore.Authorization;
	using d=System.Diagnostics.Debug;
    #endregion

    public partial class ResourceController : IfRFBaseController
    {
        private const string swaggerControllerDescription = "Resources";
        private readonly IResourceService _resourceService;

        public ResourceController(
            IResourceService resourceService, 
            ILoggerManager logger,
            IRefdataService refdataService)
            :base(logger, refdataService)
        {
            _resourceService = resourceService;
        }

        [HttpGet]
        [EnableQuery(MaxExpansionDepth = 4)]
        [SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve list of Resources")]
        public IQueryable<Resource> GetResources()
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

            var localItems = _resourceService.GetResources();
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
        [SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve single Resource")]
		public ActionResult<Resource> Get( long id )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			Resource item = _resourceService.Get( id );
			if (item == null)
			{
					return Ok( new OperationFailureResult( $"Resource with id {id} not found" ) );
			}

			return Ok( item );
		}

        /// <summary>
        /// Updates the specified Resource.
        /// </summary>
        /// 
        /// <notes>Only attributes to be changed are posted</notes>
        /// <param name="item">item</param>
        /// <returns></returns>
        [HttpPatch()]
        [Authorize(AuthenticationSchemes = AUTHSCHEME, Policy = EDITORROLE_lvl_2)]
        [SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Update a specific Resource")]
        public ActionResult<Resource> PatchResource(long id, [FromBody] Delta<Resource> item)
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Resource patchedResource = _resourceService.PatchResource(id, item);

            if (patchedResource == null)
            {
				return Ok( new OperationFailureResult( $"Resource with id {id} not found") );

            }

            return Ok(patchedResource);
        }

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpDelete("{id:long}")]
        [Authorize(AuthenticationSchemes = AUTHSCHEME, Policy = EDITORROLE_lvl_2)]
        [SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Delete a Resource")]
        public IActionResult DeleteResource(long id)
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

            if (!_resourceService.DeleteResource(id))
            {
				return Ok( new OperationFailureResult( $"Error deleting Resource with id {id}") );
            }

            return Ok();
        }

        [HttpPost("Batch/{resourceId:long}/Characteristics")]
        [SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Add characteristics to a resource")]
        public IActionResult Characteristics(long resourceId, [FromBody] IEnumerable<ResourceCharacteristic> newResourceCharacteristics)
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            IEnumerable <ResourceCharacteristic> allResourceCharacteristics = 
                _resourceService.AddCharacteristics(resourceId, newResourceCharacteristics);

            if (allResourceCharacteristics == null)
            {
				return Ok( new OperationFailureResult( $"Resource with id {resourceId} not found.") );
            }

            return Ok(allResourceCharacteristics);
        }
    }
}


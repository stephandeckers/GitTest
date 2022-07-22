/**
 * @Name SubrackController.cs
 * @Purpose 
 * @Date 12 July 2021, 09:55:48
 * @Author S.Deckers
 * @Description 
 */

namespace PXS.IfRF.Controllers
{
    #region -- Using directives --
    using System;
	using System.Linq;
	using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using Swashbuckle.AspNetCore.Annotations;
    using Microsoft.AspNet.OData;
    using PXS.IfRF.Services;
    using PXS.IfRF.Logging;
    using PXS.IfRF.Data.Model;
    using PXS.IfRF.BusinessLogic;
    using Microsoft.AspNetCore.Authorization;
	using System.ComponentModel.DataAnnotations;
	using d=System.Diagnostics.Debug;
    #endregion

    public partial class SubrackController : IfRFBaseController
    {
        private const string swaggerControllerDescription = "Subracks";
        private readonly ISubrackService _subrackService;

        public SubrackController
        (
            ISubrackService subrackService, ILoggerManager logger, IRefdataService refdataService
        )
            : base(logger, refdataService)
        {
            _subrackService = subrackService;
        }

        [HttpGet()]
        [EnableQuery(MaxExpansionDepth = 4)]
        [SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve list of Subracks")]
        public IQueryable<Subrack> GetItems()
        {
            var items = _subrackService.GetItems();

            return (items);
        }

        [HttpGet("{id:long}")]
        [EnableQuery(MaxExpansionDepth = 4)]
        [SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve single Subrack")]
        public ActionResult<Subrack> Get(long id)
        {
			SingleResult<Subrack> item = _subrackService.Get(id);
			if( item.Queryable.Count() == 0)
			{
				return Ok(new OperationFailureResult($"SrPosition with id {id} not found"));
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
        [SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Create single Subrack")]
        public ActionResult<Subrack> Create([FromBody] Subrack subrack)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            Subrack createdSubrack = _subrackService.Create(subrack);
            return Ok(createdSubrack);
        }

		/// <summary date="25-03-2022, 11:08:18" author="S.Deckers">
		/// Creates custom positions.
		/// </summary>
		/// <param name="subrack">The subrack.</param>
		/// <returns></returns>
		[ HttpPost			( )]
		[ Route				( "custom_positions")]
        [ Authorize			( AuthenticationSchemes = AUTHSCHEME, Policy = EDITORROLE_lvl_2)]
        [ SwaggerOperation	( Tags = new[] { swaggerControllerDescription }, Summary = "Create custom positions for Subrack")]
        public IQueryable<SrPosition> CreateCustomPositions( [FromBody] CustomPositionRequest request)
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			List<SrPosition> items = this._subrackService.CreateCustomPositions( request);
			return( items.AsQueryable( ));
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
        [SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Update a specific Subrack")]
		public ActionResult<Subrack> Patch(long id, [FromBody] Delta<Subrack> item)
		{
			if (!ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			Subrack patched = _subrackService.Patch(id, item);
			if (patched != null)
			{
				return Ok( patched);
			}

			BusinessLogicResult res = new BusinessLogicResult( );
			res.ErrorMessages.Add( $"Subrack with id {id} not found");

			return Ok(new OperationFailureResult(res.ErrorMessages));
		}

        /// <summary>
        /// Deletes the specified identifier.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <returns></returns>
        [HttpDelete("{id:int}")]
        [Authorize(AuthenticationSchemes = AUTHSCHEME, Policy = EDITORROLE_lvl_2)]
        [SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Delete a Subrack")]
        public IActionResult Delete(long id)
        {
            BusinessLogicResult res = _subrackService.Delete(id);
            if (!res.Succeeded)
            {
                return Ok(new OperationFailureResult(res.ErrorMessages));
            }

            return Ok();
        }
    }
}
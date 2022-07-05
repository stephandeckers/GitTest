/**
 * @Date 17 Sept 2021
 * @Author J.Stuckens
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
    using PXS.IfRF.ErrorHandling;
    using System.Linq;
    #endregion

    public partial class SrPositionAggregateController : IfRFBaseController
    {
        private const string swaggerControllerDescription = PXS.IfRF.Supporting.SharedSwaggerNames.Utilities;
        private readonly ISrPositionAggregateService _srPositionAggregateService;

        public SrPositionAggregateController
        (
            ISrPositionAggregateService srPositionAggregateService
            , ILoggerManager logger
            , IRefdataService refdataService
        ) : base(logger, refdataService)
        {
            _srPositionAggregateService = srPositionAggregateService;
        }

        [HttpPost()]
        [SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Generate aggregate statistics for positions in a subrack")]
        public ActionResult<IQueryable<SrPositionAggregateResponse>> GenerateSrPositionAggregate([FromBody] long subrackId)
        {

            List<SrPositionAggregateResponse> allItems = _srPositionAggregateService.GenerateSrPositionAggregate(subrackId);

            if (allItems == null)
            {
                return Ok(new OperationFailureResult($"Subrack with id {subrackId} not found"));
            }

            return Ok(allItems.AsQueryable());
                        
        }

        [HttpGet()]
        [SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Generate aggregate statistics for positions in a subrack")]
        public ActionResult<IQueryable<SrPositionAggregateResponse>> GetGenerateSrPositionAggregate([FromHeader] long subrackId)
        {

            List<SrPositionAggregateResponse> allItems = _srPositionAggregateService.GenerateSrPositionAggregate(subrackId);

            if (allItems == null)
            {
                return Ok(new OperationFailureResult($"Subrack with id {subrackId} not found"));
            }

            return Ok(allItems.AsQueryable());

        }
    }
}
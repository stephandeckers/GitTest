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

    public partial class GlobalPositionAggregateController : IfRFBaseController
    {
        private const string swaggerControllerDescription = PXS.IfRF.Supporting.SharedSwaggerNames.Utilities;
        private readonly IGlobalPositionAggregateService _globalPositionAggregateService;

        public GlobalPositionAggregateController
        (
            IGlobalPositionAggregateService globalPositionAggregateService
            , ILoggerManager logger
            , IRefdataService refdataService
        ) : base(logger, refdataService)
        {
            _globalPositionAggregateService = globalPositionAggregateService;
        }

        [HttpPost()]
        [SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Generate aggregate statistics for positions in a subrack")]
        public ActionResult<IQueryable<GlobalPositionAggregateResponse>> GenerateSrPositionAggregate
            ([FromBody] GlobalPositionAggregateRequest request)
        {

            (List<GlobalPositionAggregateResponse> allItems, string errorMessage) = 
                _globalPositionAggregateService.GenerateGlobalPositionAggregate(request);

            if (errorMessage != string.Empty)
            {
                return Ok(new OperationFailureResult(errorMessage));
            }

            return Ok(allItems.AsQueryable());
            
            
        }
    }
}
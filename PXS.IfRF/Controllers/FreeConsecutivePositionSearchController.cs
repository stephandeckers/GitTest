/**
 * @Name NetworkPathController.cs
 * @Purpose 
 * @Date 21 July 2021, 10:51:54
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
    using PXS.IfRF.ErrorHandling;
    using System.Linq;
    #endregion

    public partial class FreeConsecutivePositionSearchController : IfRFBaseController
    {
        private const string swaggerControllerDescription = PXS.IfRF.Supporting.SharedSwaggerNames.Utilities;
        private readonly IFreeConsecutivePositionSearchService _freeConsecutivePositionSearchService;

        public FreeConsecutivePositionSearchController
        (
            IFreeConsecutivePositionSearchService freeConsecutivePositionSearchService
            , ILoggerManager logger
            , IRefdataService refdataService
        ) : base(logger, refdataService)
        {
            _freeConsecutivePositionSearchService = freeConsecutivePositionSearchService;
        }

        [HttpPost()]
        [EnableQuery()]
        [SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Check availability")]
        public IQueryable<FreeConsecutivePositionSearchResponse> FindPositions([FromBody] FreeConsectivePositionSearchRequest request)
        {

            List<FreeConsecutivePositionSearchResponse> allItems = this._freeConsecutivePositionSearchService.GetFreePositions(request);
            
            return (allItems.AsQueryable( ));
            
            
        }
    }
}
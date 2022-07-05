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
	using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using Swashbuckle.AspNetCore.Annotations;
    using Microsoft.AspNet.OData;
    using PXS.IfRF.Services;    
    using PXS.IfRF.Logging;
    using PXS.IfRF.Data.Model;
    using System.Linq;
    using d=System.Diagnostics.Debug;
    #endregion

    public partial class FreeSplitterPositionSearchController : IfRFBaseController
    {
        private const string swaggerControllerDescription = PXS.IfRF.Supporting.SharedSwaggerNames.Utilities;
        private readonly IFreeSplitterPositionSearchService _freeSplitterPositionSearchService;

        public FreeSplitterPositionSearchController
        (
            IFreeSplitterPositionSearchService	freeSplitterPositionSearchService
		,	ILoggerManager						logger
		,	IRefdataService						refdataService
        ) : base(logger, refdataService)
        {
            _freeSplitterPositionSearchService = freeSplitterPositionSearchService;
        }

        [ HttpPost			( )]
        [ EnableQuery		( )]        
        [ SwaggerOperation	( Tags = new[] { swaggerControllerDescription }, Summary = "Check availability - new implementation")]
        public IQueryable<FreeSplitterPositionSearchResponse> FindPositions([FromBody] FreeSplitterPositionSearchRequest request)
        {
            System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch( );
            stopwatch.Start( );

            List<FreeSplitterPositionSearchResponse> allItems = this._freeSplitterPositionSearchService.GetFreeSplitterPositions( request);

            stopwatch.Stop( );
            d.WriteLine ( string.Format( "Ellapsed: {0:hh}:{0:mm}:{0:ss}.{1}", stopwatch.Elapsed, stopwatch.ElapsedMilliseconds));

            return( allItems.AsQueryable( ));
        }
    }
}
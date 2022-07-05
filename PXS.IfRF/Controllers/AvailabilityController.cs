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
	using d=System.Diagnostics.Debug;
    #endregion

    public partial class AvailabilityController : IfRFBaseController
    {
        private const string swaggerControllerDescription = PXS.IfRF.Supporting.SharedSwaggerNames.Utilities;
        private readonly IAvailabilityService _availabilityService;

        public AvailabilityController
		( 
			IAvailabilityService	availabilityService
		,	ILoggerManager			logger
            , IRefdataService refdataService
		)
            :base(logger,refdataService)
        {
			_availabilityService	= availabilityService;
        }

		[ HttpPost			( )]
        [ EnableQuery		( )]
        [ SwaggerOperation	( Tags = new[] { swaggerControllerDescription }, Summary = "Check availability")]
        public IQueryable<AvailabilityResponse> Check( [FromBody] AvailabilityRequest request)
        {
			List<AvailabilityResponse> allItems = this._availabilityService.Check( request);
			List<AvailabilityResponse> items	= new List<AvailabilityResponse>( );
			items.Add( allItems.FirstOrDefault( ));

			return( items.AsQueryable( ) );
        }
	}
}
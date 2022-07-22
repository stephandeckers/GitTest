/**
 * @Name ONTPPositionConnectionSearchController.cs
 * @Purpose 
 * @Date 27 September 2021, 13:54:10
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
	using PXS.IfRF.Data.Model.ONTPPositionConnectionSearch;
	using PXS.IfRF.ErrorHandling;
	using System.Linq;
	#endregion

	public partial class ONTPPositionConnectionSearchController : IfRFBaseController
	{
		private const string swaggerControllerDescription = PXS.IfRF.Supporting.SharedSwaggerNames.Utilities;
		private readonly IONTPPositionConnectionSearchService _ontpPositionConnectionSearchService;

		public ONTPPositionConnectionSearchController
		(
			IONTPPositionConnectionSearchService	ONTPPositionConnectionSearchService
		,	ILoggerManager							logger
		,	IRefdataService							refdataService
		) : base( logger, refdataService)
		{
			this._ontpPositionConnectionSearchService = ONTPPositionConnectionSearchService;
		}

		[ HttpPost		( )]
		[ EnableQuery	( )]
		[ SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Search ONTP positions")]
		public ActionResult<IQueryable<ONTPPositionConnectionSearchResponse>> Execute
		(
			[FromBody] ONTPPositionConnectionSearchRequest request
		)
		{
			(List<ONTPPositionConnectionSearchResponse> allItems, string errorMessage) = _ontpPositionConnectionSearchService.Execute( request);

			if (errorMessage != string.Empty)
			{
				return Ok(new OperationFailureResult(errorMessage));
			}

			return Ok(allItems.AsQueryable());
		}
	}
}
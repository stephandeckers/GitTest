using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using PXS.IfRF.Logging;
using PXS.IfRF.Services;

namespace PXS.IfRF.Controllers
{
	[ApiController]
	[ApiExplorerSettings(IgnoreApi = false)]
	[Produces("application/json", "application/json;odata.metadata=none")]
	[Route("api/[controller]")]
	[Authorize(AuthenticationSchemes = AUTHSCHEME, Policy = VIEWERROLE)]
	[EnableCors(Startup.PRXCORS)]
	//[Authorize(AuthenticationSchemes = "Webseal,"+Startup.CERTIFICATESCHEME)]
	public abstract class IfRFBaseController : ODataController
	{
		public const string VIEWERROLE			= "Viewer";
		//public const string EDITORROLE			= "Editor";
		public const string EDITORROLE_lvl_1	= "Editor1";
		public const string EDITORROLE_lvl_2	= "Editor2";
		public const string ADMINROLE			= "Admin";
		public const string AUTHSCHEME			= "Webseal";

		// --- needs to be present in appsettings.json
		public const string NEDA_GRP_001		= "NEDA001";

		protected readonly ILoggerManager _logger;
		protected readonly IRefdataService _refdataService;

		protected IfRFBaseController( ILoggerManager logger)
		{
			_logger = logger;
		}

		protected IfRFBaseController(ILoggerManager logger, IRefdataService refdataService)
		{
			_logger = logger;
			_refdataService = refdataService;
		}
	}
}

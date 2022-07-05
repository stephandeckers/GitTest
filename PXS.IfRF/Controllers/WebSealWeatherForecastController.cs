namespace PXS.IfRF.Controllers
{
	//#region -- Using directives --
	//using System;
 //   using System.Text;
	//using System.Collections.Generic;
	//using System.Linq;
 //   using Microsoft.AspNetCore.Authorization;
 //   using Microsoft.AspNetCore.Mvc;

	//using Swashbuckle.AspNetCore.Annotations;
	
	//using PXS.IfRF.Logging;
	//using Microsoft.AspNetCore.OData.Routing.Controllers;

	//using d=System.Diagnostics.Debug;
	//#endregion

    #region -- Using directives --
    using System;
    using Microsoft.AspNetCore.Mvc;
	using Microsoft.AspNetCore.Authorization;
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

	//[ ApiController			( )]
	//[ ApiExplorerSettings	( IgnoreApi = false)]
	//[ Produces				( "application/json", "application/json;odata.metadata=none")]
	//[ Route					( "api/[controller]")]
	//[ Authorize				( AuthenticationSchemes = AUTHSCHEME, Policy = VIEWERROLE)]
	//public abstract class IfRFBaseController : ODataController
	//{
	//	public const string VIEWERROLE			= "Viewer";
	//	public const string EDITORROLE_lvl_1	= "Editor1";
	//	public const string EDITORROLE_lvl_2	= "Editor2";
	//	public const string ADMINROLE			= "Admin";
	//	public const string AUTHSCHEME			= "CertAndWebseal";

	//	// --- needs to be present in appsettings.json
	//	public const string NEDA_GRP_001		= "NEDA001";
	//}
    

	public class WebSealWeatherForecastController : IfRFBaseController
	{
        public WebSealWeatherForecastController
		( 
			ILoggerManager		logger
		)
            :base(logger)
        {
        }

		private static readonly string[] strings = new[]
		{
			"one", "two", "three", "four"
		};

		private const string swaggerControllerDescription = "Neda test - WebSeal";

		[ HttpGet			( )]
		//[ Authorize			( AuthenticationSchemes = AUTHSCHEME, Policy = VIEWERROLE )]
		[ SwaggerOperation	( Tags = new[] { swaggerControllerDescription }, Summary = "Get All items" )]
		public IEnumerable<string> Get( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			return (strings).ToArray( );
		}

		[ HttpGet			( "lvl1")]
		[ Authorize			( AuthenticationSchemes = AUTHSCHEME, Policy = EDITORROLE_lvl_1 )]
		[ SwaggerOperation	( Tags = new[] { swaggerControllerDescription }, Summary = "Editor lvl1-IfRF" )]
		public IEnumerable<string> Get_lvl1( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			return (strings).ToArray( );
		}

		[ HttpGet			( "lvl2")]
		[ Authorize			( AuthenticationSchemes = AUTHSCHEME, Policy = EDITORROLE_lvl_2 )]
		[ SwaggerOperation	( Tags = new[] { swaggerControllerDescription }, Summary = "Editor lvl2-IfRF/Neda" )]
		public IEnumerable<string> Get_lvl2( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			return (strings).ToArray( );
		}
	}
}

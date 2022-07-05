/**
 * @Name InfoController.cs
 * @Purpose 
 * @Date 09 June 2021, 09:04:23
 * @Author S.Deckers
 * @Description 
 */

namespace PXS.IfRF.Controllers
{
	#region -- Using directives --
	using System;
	using System.IO;
	using System.Linq;
	using Microsoft.AspNet.OData;
	using Microsoft.AspNetCore.Authorization;
	using Microsoft.AspNetCore.Cors;
	using Microsoft.AspNetCore.Mvc;
	using PXS.IfRF.Logging;
	using PXS.IfRF.Services;
	using System.Collections.Generic;
	using Swashbuckle.AspNetCore.Annotations;
	using Microsoft.Extensions.Configuration;
	using PXS.IfRF.Data.Model;
	using d=System.Diagnostics.Debug;    
	#endregion

	
	public class InfoController : IfRFBaseController
	{
		private readonly IInfoService _infoService;

		private const string swaggerControllerDescription = "General information";

		public InfoController
		(
			ILoggerManager	logger
		,	IInfoService	infoService
		,	IRefdataService refdataService
		)
			: base (logger,refdataService)
		{
			_infoService	= infoService;
		}		

		/// <summary>
		/// Gets this instance.
		/// </summary>
		/// <returns></returns>
		[ HttpGet()]
		[ SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve build version")]
		public ActionResult<InfoResponse> Get()
		{
			return( this._infoService.Get());
		}

		[ HttpGet( "groups" )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve available groups for a given role" )]
		public List<string> GetGroups( string role )
		{
			return (this._infoService.GetGroups( role));
		}
	}

	/// <summary date="05-07-2022, 06:39:23" author="S.Deckers">
	/// 2do:Remove once mTLS is working in OPC4
	/// </summary>
	/// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
	[ ApiController			( )]
	[ ApiExplorerSettings	( IgnoreApi = false)]
	[ Produces				( "application/json", "application/json;odata.metadata=none")]
	[ Route					( "api/[controller]")]
	[ EnableCors			( Startup.PRXCORS)]
	public class InfoController2 : ControllerBase
	{
		private readonly ILoggerManager	_logger = null;

		private readonly IInfoService _infoService;

		private const string swaggerControllerDescription = "General information - no auth";

		public InfoController2
		(
			ILoggerManager	logger
		,	IInfoService	infoService
		,	IRefdataService refdataService
		)
			//: base (logger,refdataService)
		{
			_infoService	= infoService;
			_logger			= logger;
		}		

		[ HttpGet()]
		[ SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve build version")]
		public ActionResult<InfoResponse> Get()
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			this._logger.Trace( $"Get:{System.DateTime.Now}");
			return( this._infoService.Get());
		}

		[ HttpGet( "groups" )]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve available groups for a given role" )]
		public List<string> GetGroups( string role )
		{
			return (this._infoService.GetGroups( role));
		}
	}
}

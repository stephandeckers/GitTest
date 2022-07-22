/**
 * @Name SwaggerOperationFilters.cs
 * @Purpose 
 * @Date 07 June 2022, 11:52:06
 * @Author S.Deckers
 * @Description 
 * @Description Filter types
 * 
 * - 2 parts of authorisation, type-level and method-level
 * - authorisation is applied through headers
 * - whenever authorisation is missing, methods/types requiring authorisation are not show
 * - authorisation on type-level doesn't check the type of access, t.i. when defining role 'Viewer' on type-level
 *   no check is done when accessing post/patch
 *
 * DocumentFilters : Remove part of Swagger documents
 * OperationFilter : Extend supported operation
 * ParameterFilter
 * SchemaFilter
 */

namespace Whoua.Core.Api.Controllers
{
	#region -- Using directives --

    #region -- System --
	using System;
    using System.Text;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
    using System.IdentityModel.Tokens.Jwt;
	using Whoua.Core.Api.Model;
	using Swashbuckle.AspNetCore.Annotations;
    #endregion

    #region -- Microsoft --
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using Microsoft.Extensions.Logging;
    #endregion

	using d=System.Diagnostics.Debug;
	using Microsoft.AspNetCore.OData.Deltas;
	#endregion

	#region -- Type level authorisation --

	/*
	[ ApiController	( )]
	[ Route			( "api/[controller]" )]
	[ Produces		( "application/json")]
	[ Authorize		( AuthenticationSchemes = Global.AUTHSCHEME, Policy = Global.VIEWERROLE)]
	public class ViewableTypeController : Microsoft.AspNetCore.Mvc.Controller
	{
		private const string swaggerControllerDescription = "'View'- Method level authorisation'";

		private static readonly string[] Summaries = new[]
		{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};
		
		[ HttpGet	(  )]
		[ Route		( "Get")]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Get WeatherForecast" )]
		public IEnumerable<WeatherForecast> Get( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			Random random = new Random( );
			return Enumerable.Range( 1, 5 ).Select( index => new WeatherForecast
			{
				Id				= index
			,	Date			= DateTime.Now.AddDays( index )
			,	TemperatureC	= random.Next( -20, 55 )
			,	Summary			= Summaries[ random.Next( Summaries.Length )]
			} )
			.ToArray( );
		}

		[ HttpGet("{id:long}")]
		[ SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve single WeatherForecast")]
		public ActionResult<WeatherForecast> Get(long id)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );
			Random random = new Random( );

			return Enumerable.Range( 1, 5 ).Select( index => new WeatherForecast
			{
				Id				= index
			,	Date			= DateTime.Now.AddDays( index )
			,	TemperatureC	= random.Next( -20, 55 )
			,	Summary			= Summaries[ random.Next( Summaries.Length )]
			} )
			.FirstOrDefault( );
		}

		[ HttpPost()]
		[ SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Create WeatherForecast")]
		public ActionResult<WeatherForecast> Create([FromBody] WeatherForecast item)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			return Ok( item);
		}

		[ HttpPatch()]
		[ SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Update a WeatherForecast")]
		public ActionResult<WeatherForecast> Patch(long id, [FromBody] Delta<WeatherForecast> item)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			return Ok( item);
		}

        [ HttpDelete("{id:int}")]
        [ SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Delete a WeatherForecast")]
        public IActionResult Delete(long id)
        {
			return Ok();
        }
	}

	[ ApiController	( )]
	[ Route			( "api/[controller]" )]
	[ Produces		( "application/json")]
	[ Authorize		( AuthenticationSchemes = Global.AUTHSCHEME, Policy = Global.EDITORROLE)]
	public class EditableTypeController : Microsoft.AspNetCore.Mvc.Controller
	{
		private const string swaggerControllerDescription = "'Edit'-Method level authorisation";

		private static readonly string[] Summaries = new[]
		{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};
		
		[ HttpGet	(  )]
		[ Route		( "Get")]
		[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Get WeatherForecast" )]
		public IEnumerable<WeatherForecast> Get( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			Random random = new Random( );
			return Enumerable.Range( 1, 5 ).Select( index => new WeatherForecast
			{
				Id				= index
			,	Date			= DateTime.Now.AddDays( index )
			,	TemperatureC	= random.Next( -20, 55 )
			,	Summary			= Summaries[ random.Next( Summaries.Length )]
			} )
			.ToArray( );
		}

		[ HttpGet("{id:long}")]
		[ SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Retrieve single WeatherForecast")]
		public ActionResult<WeatherForecast> Get(long id)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );
			Random random = new Random( );

			return Enumerable.Range( 1, 5 ).Select( index => new WeatherForecast
			{
				Id				= index
			,	Date			= DateTime.Now.AddDays( index )
			,	TemperatureC	= random.Next( -20, 55 )
			,	Summary			= Summaries[ random.Next( Summaries.Length )]
			} )
			.FirstOrDefault( );
		}

		[ HttpPost()]
		[ SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Create WeatherForecast")]
		public ActionResult<WeatherForecast> Create([FromBody] WeatherForecast item)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			return Ok( item);
		}

		[ HttpPatch()]
		[ SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Update a WeatherForecast")]
		public ActionResult<WeatherForecast> Patch(long id, [FromBody] Delta<WeatherForecast> item)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			return Ok( item);
		}

        [ HttpDelete("{id:int}")]
        [ SwaggerOperation(Tags = new[] { swaggerControllerDescription }, Summary = "Delete a WeatherForecast")]
        public IActionResult Delete(long id)
        {
			return Ok();
        }
	}
    
	*/

	#endregion

	//#region -- Always available --

	//[ ApiController( )]
	//[ Route( "api/[controller]" )]
	//[ Produces( "application/json" )]
	//public class PublicAccessibleTypeController : Controller
	//{
	//	private const string swaggerControllerDescription = "PublicAccessibleType";

	//	private static readonly string[] strings = new[]
	//	{
	//		"one", "two", "three", "four"
	//	};

	//	[HttpGet( )]
	//	[Route( "Get" )]
	//	[SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Get" )]
	//	public IEnumerable<string> Get( )
	//	{
	//		d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

	//		return (strings).ToArray( );
	//	}
	//}

	//#endregion

	#region -- Method level authorisation --

	//[ApiController( )]
	//[Route( "api/[controller]" )]
	//[Produces( "application/json" )]
	//public class CoolController : Microsoft.AspNetCore.Mvc.Controller
	//{
	//	private const string swaggerControllerDescription = "Controller visible for everyone";

	//	[HttpGet( )]
	//	[Authorize( AuthenticationSchemes = Global.AUTHSCHEME, Policy = Global.VIEWERROLE )]
	//	[SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "1-GetAuthorizedMethodDefaultRoute" )]
	//	public IEnumerable<string> GetAuthorizedMethodDefaultRoute( )
	//	{
	//		d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

	//		return (new List<string> { "a", "b" }).ToArray( );
	//	}

	//	[HttpPost( )]
	//	[SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "4b-CreateMethod (IFRF)" )]
	//	[Authorize( AuthenticationSchemes = Global.AUTHSCHEME, Policy = Global.EDITORROLE )]
	//	public ActionResult<WeatherForecast> CreateMethod2( [FromBody] WeatherForecast item )
	//	{
	//		d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

	//		return Ok( item );
	//	}
	//}

	[ ApiController	( )]
	[ Route			( "api/[controller]" )]
	[ Produces		( "application/json")]
	public class AllInOneController : Microsoft.AspNetCore.Mvc.Controller
	{
		private static readonly string[] strings = new[]
		{
			"one", "two", "three", "four"
		};

		private const string swaggerControllerDescription = "NEDAP test";

		#region -- HttpGet --

		//[HttpGet( )]
		//[Route( "all" )]
		//[SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "viewable Get" )]
		//public IEnumerable<string> GetAuthorizedMethodAll( )
		//{
		//	d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

		//	return (strings).ToArray( );
		//}

		//[HttpGet( )]
		//[Authorize( AuthenticationSchemes = Global.AUTHSCHEME, Policy = Global.VIEWERROLE )]
		//[SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "1-GetAuthorizedMethodDefaultRoute" )]
		//public IEnumerable<string> GetAuthorizedMethodDefaultRoute( )
		//{
		//	d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

		//	return (strings).ToArray( );
		//}

		//[HttpGet( )]
		//[Route( "a1" )]
		//[Authorize( AuthenticationSchemes = Global.AUTHSCHEME, Policy = Global.VIEWERROLE )]
		//[SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "1a-GetAuthorizedMethodDefaultRoute" )]
		//public IEnumerable<string> GetAuthorizedMethodDefaultRoute1a( )
		//{
		//	d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

		//	return (strings).ToArray( );
		//}

		//[HttpGet( "{id:int}" )]
		//[Authorize( AuthenticationSchemes = Global.AUTHSCHEME, Policy = Global.VIEWERROLE )]
		//[SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "2-GetAuthorizedMethod( int id)" )]
		//public IEnumerable<string> GetAuthorizedMethod( int id )
		//{
		//	d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

		//	return (strings).ToArray( );
		//}

		//[HttpGet( )]
		//[Route( "GetAuthorized666" )]
		//[Authorize( AuthenticationSchemes = Global.AUTHSCHEME, Policy = Global.VIEWERROLE )]
		//[SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "3-GetAuthorizedMethod2222" )]
		//public IEnumerable<string> GetAuthorizedMethod2222( )
		//{
		//	d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

		//	return (strings).ToArray( );
		//}

		//[HttpGet( )]
		//[Route( "GetAuthorized66688" )]
		//[SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "Public viewable Get" )]
		//public IEnumerable<string> GetAuthorizedMethodAll666( )
		//{
		//	d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

		//	return (strings).ToArray( );
		//}
		#endregion

		#region -- HttpPost --

		// if no NEDAP-EDITORROLE
		//   hide everyhing without NEDAP_EDITORROLE
		[HttpPost( )]
		[SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "4a-CreateMethod (IFRF)" )]
		//[Authorize( AuthenticationSchemes = Global.AUTHSCHEME, Policy = Global.EDITORROLE )]
		//[Authorize( AuthenticationSchemes = Global.AUTHSCHEME, Policy = Global.NEDAP_EDITORROLE )]
		public ActionResult<WeatherForecast> CreateMethod1( [FromBody] WeatherForecast item )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			return Ok( item );
		}

		[HttpPost( "a" )]
		[SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "4b-CreateMethod (No Nedap)" )]
		//[Authorize( AuthenticationSchemes = Global.AUTHSCHEME, Policy = Global.EDITORROLE )]
		[Authorize( AuthenticationSchemes = Global.AUTHSCHEME, Policy = Global.NEDAP_EDITORROLE )]
		public ActionResult<WeatherForecast> CreateMethod2( [FromBody] WeatherForecast item )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			return Ok( item );
		}

		//[HttpPost( )]
		//[Route( "b" )]
		//[SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "4c-CreateMethod" )]
		//[Authorize( AuthenticationSchemes = Global.AUTHSCHEME, Policy = Global.EDITORROLE )]
		//public ActionResult<WeatherForecast> CreateMethod3( [FromBody] WeatherForecast item )
		//{
		//	d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

		//	return Ok( item );
		//}

		//[HttpPost( "{id1:int}")]
		//[SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "4d-CreateMethod" )]
		//[Authorize( AuthenticationSchemes = Global.AUTHSCHEME, Policy = Global.EDITORROLE )]
		//public ActionResult<WeatherForecast> CreateMethod4( [FromBody] WeatherForecast item )
		//{
		//	d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

		//	return Ok( item );
		//}

		//[HttpPost( "{id2:long}")]
		//[SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "4e-CreateMethod" )]
		//[Authorize( AuthenticationSchemes = Global.AUTHSCHEME, Policy = Global.EDITORROLE )]
		//public ActionResult<WeatherForecast> CreateMethod5( [FromBody] WeatherForecast item )
		//{
		//	d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

		//	return Ok( item );
		//}

		//[ HttpPost( "a")]
		//[ Route( "SomeCrazyRoute" )]
		//[ SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "4e-CreateMethod (invalid swaggerdocument" )]
		//[ Authorize( AuthenticationSchemes = Global.AUTHSCHEME, Policy = Global.EDITORROLE )]
		//public ActionResult<WeatherForecast> CreateMethod5( [FromBody] WeatherForecast item )
		//{
		//	d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

		//	return Ok( item );
		//}

		#endregion

		#region -- HttpPatch --

		//[HttpPatch( )]
		//[SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "5-PatchMethod" )]
		//[Authorize( AuthenticationSchemes = Global.AUTHSCHEME, Policy = Global.EDITORROLE )]
		//public ActionResult<WeatherForecast> PatchMethod( long id, [FromBody] Delta<WeatherForecast> item )
		//{
		//	d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

		//	return Ok( item );
		//}

		#endregion

		#region -- HttpDelete --

		//[HttpDelete( "{id:int}" )]
		//[SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "6-DeleteMethod( long id )" )]
		//[Authorize( AuthenticationSchemes = Global.AUTHSCHEME, Policy = Global.EDITORROLE )]
		//public IActionResult DeleteMethod1( int id )
		//{
		//	return Ok( );
		//}

		//[HttpDelete( "{id1:int}&{id2:int}" )]
		//[SwaggerOperation( Tags = new[] { swaggerControllerDescription }, Summary = "8-DeleteMethod( long id1, long id2 )" )]
		//[Authorize( AuthenticationSchemes = Global.AUTHSCHEME, Policy = Global.EDITORROLE )]
		//public IActionResult DeleteMethod3( long id1, long id2 )
		//{
		//	return Ok( );
		//}

		#endregion

	}

	#endregion
}

namespace Whoua.Core.Api.Model
{
	#region -- Using directives --
	using System;
	using System.ComponentModel.DataAnnotations;
	using d=System.Diagnostics.Debug;
	#endregion

	public class WeatherForecast
	{
		[ Key ()]
		public int		Id					{ get; set; }
		public DateTime Date				{ get; set; }
		public int		TemperatureC		{ get; set; }
		public int		TemperatureF => 32 + (int)(TemperatureC / 0.5556);
		public string	Summary				{ get; set; }
	}
}

namespace Whoua.Core.Api
{
	#region -- Using directives --
	using System;
	using System.Linq;
	using System.Collections.Generic;
	using System.Reflection;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;
	using Microsoft.AspNetCore.Http;
	using Microsoft.Extensions.Configuration;
	using Microsoft.AspNetCore.Mvc.Authorization;
	using Microsoft.AspNetCore.Authorization;
	using Swashbuckle.AspNetCore.SwaggerGen;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.OpenApi.Models;
	using d = System.Diagnostics.Debug;
	#endregion

	public static class Global
	{
		public static int	 CallCount		= 0;

		public const string VIEWERROLE			= "Viewer";		
		public const string EDITORROLE			= "Editor";
		public const string NEDAP_EDITORROLE	= "Nedap_Editor";
		//public const string ADMINROLE	= "Admin";
		public const string AUTHSCHEME			= "Webseal";
	}

	public class Program
	{
		public static void Main( string[] args )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", System.Reflection.MethodBase.GetCurrentMethod().DeclaringType.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			CreateHostBuilder( args ).Build( ).Run( );
		}

		public static IHostBuilder CreateHostBuilder( string[] args ) =>
			Host.CreateDefaultBuilder( args )
				.ConfigureWebHostDefaults( webBuilder =>
				 {
					 webBuilder.UseStartup<Startup>( );
				 } );
	}

	public class Startup
	{
		public void Configure( IApplicationBuilder app, IWebHostEnvironment env )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			app.UseDeveloperExceptionPage	( );
            app.UseHttpsRedirection			( );
            app.UseRouting					( );
            app.UseAuthorization			( );
            app.UseEndpoints				( e => { e.MapControllers(); });
            app.UseAuthentication			( );            
            app.UseSwagger					( );
            app.UseSwaggerUI				( c => 
			{ 
				c.SwaggerEndpoint	( "/swagger/v1/swagger.json", "Foo API v1"); 
				c.DisplayOperationId( );
			});
		}

		public void ConfigureServices( IServiceCollection services )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			services.AddControllers			( );
			services.AddHttpContextAccessor	( );

			string srcTitle		= $"Swagger filters - {System.DateTime.Now.ToString()}";
			string src			= "no source";
			string theUrl		= @$"<a href={src} target=_blank>{srcTitle}</a>";
			string description	= $"Swagger document filters<br/><br/>{theUrl}";

			services.AddSwaggerGen( c =>
			{
				c.SwaggerDoc									( name: "v1", info: new OpenApiInfo { Title = "Swagger Document filters", Version = "v1", Description = description } );
				c.EnableAnnotations								( );
				c.ResolveConflictingActions						( apiDescription => apiDescription.First());				
				//c.DocumentFilter	<UnauthorizedTypeFilter>	( ); // --- pxs proto
				c.DocumentFilter	<UnauthorizedMethodFilter>	( ); // --- pxs proto
				//c.DocumentFilter		<PublicRouteRemovalFilter>	( ); // --- Removes a route if it contains the name 'public'
				//c.OperationFilter		<SomeOperationFilter>		( );
				//c.ParameterFilter
				//c.SchemaFilter
			});
		}
	}

	public abstract class UnauthorizedFilter : IDocumentFilter
	{
		protected HttpContext _httpContext => new HttpContextAccessor().HttpContext;

		protected readonly IConfiguration	_configuration;

		public UnauthorizedFilter
		(
			IConfiguration configuration
		)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));
			_configuration	= configuration;
		}

		protected virtual void RemoveAuthorizedSwaggerPaths( OpenApiDocument swaggerDoc)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			RemoveSwaggerPathWithRole( swaggerDoc, Global.VIEWERROLE);
			RemoveSwaggerPathWithRole( swaggerDoc, Global.EDITORROLE);
		}

		protected abstract void RemoveSwaggerPathWithRole	( OpenApiDocument swaggerDoc, string theRole);

		public void Apply( OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));
			//2do:Andere approach, uitgaan van swaggerDoc
			//DumpMemberRoutes( );
			//return;
			//swaggerDoc.Dump( );
			//return;

			swaggerDoc.DumpOperations();
			//swaggerDoc.OperationRemovalDemo( );
			//return;

			// p1
			//Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();
			//assembly.DumpMethodsWithAuthorisationAndHttpOperation();
			//return;

			IEnumerable<IConfigurationSection> roleGroupMappings = _configuration.GetSection("RoleGroupMappings").GetChildren();

			/* --- iv-groups sample (20220614 SDE)
			 
			string ivGroup = this._httpContext.Request.Headers[ "iv-groups"];

			if( string.IsNullOrEmpty( ivGroup))
			{
				d.WriteLine( "'ivGroup'-header not found, removing all authorized types");
				RemoveAuthorizedSwaggerPaths( swaggerDoc);
				return;
			}

			if( this._httpContext.Request.Headers.HasRole( theRole:Global.EDITORROLE, theGroup:ivGroup, section:roleGroupMappings) == false)
			{
				d.WriteLine( "'ivGroup'-header found, role 'Editor' not found => removing all 'Editor'-authorized types");
				RemoveSwaggerPathWithRole( swaggerDoc, Global.EDITORROLE);
			}

			if( this._httpContext.Request.Headers.HasRole( theRole:Global.VIEWERROLE, theGroup:ivGroup, section:roleGroupMappings) == false)
			{
				d.WriteLine( "'ivGroup'-header found, role 'Viewer' not found => removing all 'Viewer'-authorized types");
				RemoveSwaggerPathWithRole( swaggerDoc, Global.VIEWERROLE);
			}
			*/

			string theGroup = this._httpContext.Request.Headers[ "nedap_group"];

			if( string.IsNullOrEmpty( theGroup))
			{
				d.WriteLine( $"'{theGroup}'-header not found, removing all authorized types");
				RemoveAuthorizedSwaggerPaths( swaggerDoc);
				return;
			}

			string theRole = Global.NEDAP_EDITORROLE;

			if( this._httpContext.Request.Headers.HasRole( theRole:theRole, theGroup:theGroup, section:roleGroupMappings) == false)
			{
				//d.WriteLine( "'ivGroup'-header found, role 'Editor' not found => removing all 'Editor'-authorized types");
				d.WriteLine( $"'{theGroup}'-header found, role '{theRole}' removing all authorized types");
				RemoveSwaggerPathWithRole( swaggerDoc, theRole);
			}

			//if( this._httpContext.Request.Headers.HasRole( theRole:Global.VIEWERROLE, theGroup:ivGroup, section:roleGroupMappings) == false)
			//{
			//	d.WriteLine( "'ivGroup'-header found, role 'Viewer' not found => removing all 'Viewer'-authorized types");
			//	RemoveSwaggerPathWithRole( swaggerDoc, Global.VIEWERROLE);
			//}
		}
	}

	/// <summary>
	/// Check access on Method level
	/// </summary>
	public class UnauthorizedMethodFilter : UnauthorizedFilter
	{
		public UnauthorizedMethodFilter
		(
			IConfiguration			configuration
		)
			:base( configuration:configuration)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));
		}

		protected override void RemoveAuthorizedSwaggerPaths( OpenApiDocument swaggerDoc)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			//RemoveSwaggerPathWithRole( swaggerDoc, Global.VIEWERROLE);
			//RemoveSwaggerPathWithRole( swaggerDoc, Global.EDITORROLE);
			RemoveSwaggerPathWithRole( swaggerDoc, Global.NEDAP_EDITORROLE);
		}

		protected override void RemoveSwaggerPathWithRole( OpenApiDocument swaggerDoc, string theRole)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

			// --- Authorisation attribute on Method level

			IEnumerable<Type> types = assembly.GetExportedTypes( ).Where( x => x.BaseType == typeof(Microsoft.AspNetCore.Mvc.Controller));

			foreach( Type type in types)
			{
				RouteAttribute routeAttribute = type.GetCustomAttribute(typeof(RouteAttribute)) as RouteAttribute;

				if( routeAttribute == null)
				{
					d.WriteLine( $"RouteAttribute not found on type {type.Name}");
					continue;
				}

				MethodInfo[] methodInfo = type.GetMethods( BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);

				foreach( var m in methodInfo)
				{
					//d.WriteLine( m.ToString());										

					if( routeAttribute == null)
					{
						d.WriteLine( "No route attribute found on Controller [{type}]");
						continue;
					}
					Microsoft.AspNetCore.Authorization.AuthorizeAttribute a = (AuthorizeAttribute)Attribute.GetCustomAttribute( m, typeof (AuthorizeAttribute));
					if( a == null)
					{
						//d.WriteLine( $"type=[{type.Name}], Method=[{m.Name}], No Policy");
						continue;
					}

					//d.WriteLine( $"type=[{type.Name}], Method=[{m.Name}], Policy=[{a.Policy}]");

					if( a.Policy != theRole)
					{
						d.WriteLine( $"!=: a.Policy=[{a.Policy}], theRole[{theRole}]");
						continue;
					}

					//Type[] args = m.GetGenericArguments();				

					// --- User is not authorized, we need to remove the SwaggerOperation (20220611 SDE)

					d.WriteLine( $"Remove:type=[{type.Name}], Method=[{m.Name}], Policy={a.Policy}");

					assembly.DumpMethodsWithAuthorisationAndHttpOperation();

					//swaggerDoc.DumpOperations( );

					// p2
					(string swaggerPathKey, OperationType operationType, int parameterCount) = m.GetSwaggerKeys( type);
					d.WriteLine( $"swaggerPathKey={swaggerPathKey}, operationType={operationType}, parameterCount={parameterCount}");

					swaggerDoc.RemoveOperation( type, swaggerPathKey, operationType, parameterCount);
				}
			}
		}
	}

	/// <summary>
	/// Checks access on type level
	/// </summary>
	public class UnauthorizedTypeFilter : UnauthorizedFilter
    {
		public UnauthorizedTypeFilter
		(
			IConfiguration			configuration
		)
			:base( configuration:configuration)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));
		}

		protected override void RemoveAuthorizedSwaggerPaths( OpenApiDocument swaggerDoc)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			throw new System.NotImplementedException ( "Hoeft niet gedaan te worden");
			//RemoveSwaggerPathWithRole( swaggerDoc, Global.VIEWERROLE);
			//RemoveSwaggerPathWithRole( swaggerDoc, Global.EDITORROLE);
			//RemoveSwaggerPathWithRole( swaggerDoc, Global.NEDAP_EDITORROLE);
		}

		protected override void RemoveSwaggerPathWithRole( OpenApiDocument swaggerDoc, string theRole)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			Assembly	assembly	= System.Reflection.Assembly.GetExecutingAssembly();
			var			types		= assembly.GetExportedTypes( ).Where( x => x.BaseType == typeof(Microsoft.AspNetCore.Mvc.Controller));
			
			foreach( var type in types)
			{
				Microsoft.AspNetCore.Authorization.AuthorizeAttribute a = (AuthorizeAttribute)Attribute.GetCustomAttribute(type, typeof (AuthorizeAttribute));
				if( a == null)
				{
					continue;
				}

				//2do:same removal approach as with members
				// --- remove routes, not paths
				int pos = a.Policy.IndexOf( theRole);
				if( pos < 0)
				{ 
					continue;
				}				

				string theType = type.Name.Replace( "Controller", string.Empty).ToLower();

				d.WriteLine( $"Role [{theRole}] defined on [{type.Name}] [{theType}]");

				//var routes = swaggerDoc.Paths.Where(x => x.Key.ToLower().Contains( theType)).ToList();
				var routes = swaggerDoc.Paths.Where(x => x.Key.ToLower().Contains( theType)).ToList();

				routes.ForEach(x => { swaggerDoc.Paths.Remove(x.Key); });
			}
		}
    }

	/// <summary>
	/// Removes all routes with the string 'public' in it
	/// </summary>
	public class PublicRouteRemovalFilter : IDocumentFilter
    {
		public HttpContext _httpContext => new HttpContextAccessor().HttpContext;

        public void Apply( OpenApiDocument swaggerDoc, DocumentFilterContext context)
        {
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			var routes = swaggerDoc.Paths.Where(x => x.Key.ToLower().Contains("public")).ToList();

			routes.ForEach(x => { swaggerDoc.Paths.Remove(x.Key); });
		}
    }

	public class SomeOperationFilter : IOperationFilter
	{
		private readonly IHttpContextAccessor httpContextAccessor;
 
		public SomeOperationFilter(IHttpContextAccessor httpContextAccessor)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			this.httpContextAccessor = httpContextAccessor;
		}
 
		public void Apply( OpenApiOperation operation, OperationFilterContext context)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			var filterDescriptor	= context.ApiDescription.ActionDescriptor.FilterDescriptors;
			var isAuthorized		= filterDescriptor.Select(filterInfo => filterInfo.Filter).Any(filter => filter is AuthorizeFilter);
			var allowAnonymous		= filterDescriptor.Select( filterInfo => filterInfo.Filter).Any(filter => filter is IAllowAnonymousFilter);
		}
	}

	public static class ApiExtensions
	{
		//public static KeyValuePair<OperationType, OpenApiOperation> GetSwaggerOperationKey
		public static ( string, KeyValuePair<OperationType, OpenApiOperation>) GetSwaggerOperationKey
		( 
			this MethodInfo		methodInfo
		,		 OpenApiPaths	openApiPaths
		)
		{
			int k = 1;
			int i = 1;

			//openApiPaths.Keys
			foreach( var path in openApiPaths)
			{
				d.WriteLine( $"{k++}:{path.Key}");

				foreach (var item in path.Value.Operations)
				{
					d.WriteLine( $"{i++}:{item.Key}" );

					if( i == 2)
					{
						return( path.Key, item);
					}
				}
			}

			throw new System.NotSupportedException( "Unable to find swagger operation");
			//KeyValuePair<OperationType, OpenApiOperation> nlResult = new KeyValuePair<OperationType, OpenApiOperation>() { };
			//nlResult.Key = OperationType.Trace;
			//nlResult.Value = null;
			//return( nlResult);
		}
/*
    "RoleGroupMappings": [
        {
            "Role": "Viewer",
            "Groups": [ "PIFRF0001", "PIFRF0002", "PIFRF0100", "PIFRF1001", "PIFRF1002", "BC" ]
        },
        {
            "Role": "Editor",
            "Groups": [ "PIFRF0002", "PIFRF0100", "PIFRF1001", "PIFRF1002", "BC" ]
        },
        {
            "Role": "Admin",
            "Groups": [ "PIFRF1001", "PIFRF1002" ]
        }
    ]
*/
		/// <summary>
		/// returns true if a header value can be found in the "Editor"-section
		/// </summary>
		/// <param name="header"></param>
		/// <param name="configurationSection"></param>
		/// <returns></returns>
		public static bool HasRole
		( 
			this IHeaderDictionary				header
		,	string								theRole
		,	string								theGroup
		,	IEnumerable<IConfigurationSection>	section
		)
		{
			IDictionary<string, IList<string>> roleGroupMappings = new Dictionary<string, IList<string>>();

            foreach(IConfigurationSection anRgMap in section)
            {
                var		children	= anRgMap.GetChildren();
                string	role		= children.Single(c => c.Key.Equals("Role")).Value;

				if( role != theRole)
				{
					continue;
				}
                var		groups		= children.Single(c => c.Key.Equals("Groups")).GetChildren().Select(gr => gr.Value).ToList();
				roleGroupMappings.Add(role, groups);
            }

			if( roleGroupMappings.Count() == 0)
			{
				return( false);
			}

			foreach( var itm in roleGroupMappings)
			{
				d.WriteLine( $"{itm.Key} {itm.Value}");
				foreach( string value in itm.Value)
				{
					d.WriteLine( $"{itm.Key} {value}");
				}
			}

			//string theRole = "Editor";
			//var item = roleGroupMappings.Where( x => x.Key == theRole);
			int cnt = roleGroupMappings.Where( x => x.Value.IndexOf(theGroup) >= 0).Count();

			if( cnt > 0)
			{
				return( true);
			}
			return( false);
		}

		public static bool HasAuthorisation( this Type type, string authorisation)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", type.Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, authorisation));

			Microsoft.AspNetCore.Authorization.AuthorizeAttribute a = (AuthorizeAttribute)Attribute.GetCustomAttribute(type, typeof (AuthorizeAttribute));

			if( a == null)
			{
				return( false);
			}

			d.WriteLine( $"{type.Name}");
			d.WriteLine( $"AuthenticationSchemes={a.AuthenticationSchemes}");
			d.WriteLine( $"Policy={a.Policy}");			

			int pos = a.Policy.IndexOf( authorisation);
			if( pos >= 0)
			{
				d.WriteLine( $"pos={pos}");
				return( true);
			}
			return( false);
		}

		public static void DumpHeaders( this HttpContext httpContext )
		{
			int headerCount = httpContext.Request.Headers.Count;

			d.WriteLine( $"{headerCount} headers");

			foreach( var item in httpContext.Request.Headers)
			{
				d.WriteLine( $"key=[{item.Key}], value=[{item.Value}]");
			}
		}

		public static void Dump( this OpenApiPathItem item)
		{
			//item.Description;
			//item.Operations.Count();
			//item.Parameters.Count();
			//item.Summary;

			d.WriteLine( string.Format( "item.Description {0}", item.Description));
			d.WriteLine( string.Format( "  Operations:{0}",		item.Operations.Count()));
			d.WriteLine( string.Format( "  Parameters:{0}",		item.Parameters.Count()));
			d.WriteLine( string.Format( "  Summary:{0}",		item.Summary));
		}

		public static void Dump( this OpenApiDocument swaggerDoc)
		{
			d.WriteLine( string.Format( "item.Description {0}", swaggerDoc.ToString()));

/*
SwaggerDoc :
swaggerDoc.Path
  1:/api/PartlyViewable
  2:/api/PartlyViewable/{id}
  3:/api/PartlyViewable/GetAuthorized666

swaggerDoc.Paths.Operations
  1:/api/PartlyViewable-Get/1-GetAuthorizedMethodDefaultRoute
  2:/api/PartlyViewable-Post/4-CreateMethod
  3:/api/PartlyViewable-Patch/5-PatchMethod
  4:/api/PartlyViewable/{id}-Get/2-GetAuthorizedMethod( int id)
  5:/api/PartlyViewable/{id}-Delete/6-DeleteMethod( long id )
  6:/api/PartlyViewable/GetAuthorized666-Get/3-GetAuthorizedMethod2222

Reflection :
1:PartlyViewableController.GetAuthorizedMethodDefaultRoute()
2:PartlyViewableController.GetAuthorizedMethod(id)
3:PartlyViewableController.GetAuthorizedMethod2222()
4:PartlyViewableController.CreateMethod(item)
5:PartlyViewableController.PatchMethod(id, item)
6:PartlyViewableController.DeleteMethod(id)
*/
			int k = 1;

			int i = 1;
			foreach (var path in swaggerDoc.Paths)
			{
				d.WriteLine( $"{k++}:{path.Key}");
				//continue;

				foreach (var item in path.Value.Operations)
				{
					d.WriteLine( $"{i++}:{item.Key}" );
					//d.WriteLine( $"{i++}:{path.Key}-{item.Key}/{item.Value.Summary}" );

					if( i == 2)
					{
						path.Value.Operations.Remove( item); // --- remove single item
					}
				}

				//path.Value.Operations.Remove(OperationType.Get); // --- Removes all 'Get'-operations
			}

			// --- Dump items who make up a SwaggerPath (20220610 SDE)

			Assembly assembly = System.Reflection.Assembly.GetExecutingAssembly();

			IEnumerable<Type> types = assembly.GetExportedTypes( ).Where( x => x.BaseType == typeof(Microsoft.AspNetCore.Mvc.Controller));

			i = 1;

			foreach( Type type in types)
			{
				MethodInfo[] methodInfo = type.GetMethods( BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);

				foreach( var m in methodInfo)
				{
					ParameterInfo[] parameterInfo = m.GetParameters();

					if( parameterInfo.Count() == 0)
					{
						d.WriteLine( $"	{i++}:{type.Name}.{m.Name}()");
						continue;
					}

					string theParameters = string.Empty;

					foreach( ParameterInfo p in parameterInfo)
					{
						if( string.IsNullOrEmpty( theParameters))
						{
							theParameters = p.Name;
							continue;
						}
						theParameters += $", {p.Name}";
					}

					d.WriteLine( $"	{i++}:{type.Name}.{m.Name}({theParameters})");
				}
			}
		}		

		/// <summary date="11-06-2022, 06:04:03" author="S.Deckers">
		/// Operations make up a unique SwaggerPath
		/// </summary>
		/// <param name="swaggerDoc"></param>
		public static void DumpOperations( this OpenApiDocument swaggerDoc)
		{
			int i = 1;

			// --- Paths

			d.WriteLine( "Dumping Paths");

			foreach( var path in swaggerDoc.Paths)
			{
				d.WriteLine( $"{i++}:{path.Key}");
			}

			d.WriteLine( "Dumping Operations");

			i = 1;

			foreach( var path in swaggerDoc.Paths)
			{
				foreach( var operation in path.Value.Operations)
				{
					//d.WriteLine( $"{i++}:key={path.Key}, OperationId={operation.Value.OperationId}");
					d.WriteLine( $"{i++}:key={path.Key}, op.Key={operation.Key}, op.Parameters={operation.Value.Parameters.Count()}");
				}
			}
		}

		public static (string, Microsoft.OpenApi.Models.OperationType, int) GetSwaggerKeys( this MethodInfo methodInfo, Type type)
		{
			/// <summary date="13-06-2022, 12:12:09" author="S.Deckers">
			/// Gets the route from template attribute.
			/// </summary>
			string getRouteFromTemplateAttribute( Microsoft.AspNetCore.Mvc.Routing.HttpMethodAttribute attribute, string route)
			{
				if( attribute.Template != null)
				{ 
					string theTemplate = attribute.Template;
					theTemplate = theTemplate.Replace( ":int", string.Empty);	// --- [HttpGet( "{id:int}" )] -> {id}
					theTemplate = theTemplate.Replace( ":long", string.Empty);	// --- [HttpGet( "{id:long}" )] -> {id}
					return( $"{route}/{theTemplate}");
				}
				return( route);
			}

			/* --- GetSwaggerKeys entry --- */

			RouteAttribute routeAttribute = type.GetCustomAttribute(typeof(RouteAttribute)) as RouteAttribute;

			/* --- If there is a route on the method it needs to be appended to the route (20220611 SDE)
 			 *
			 *		type.method													route
			 *		-----------------------------------------------------------	-----------------
			 *		PartlyViewableController.GetAuthorizedMethodDefaultRoute( ) /api/PartlyViewable
			 *
			 *								 [Route( "GetAuthorized666" )]
			 *		PartlyViewableController.GetAuthorizedMethod2222( )			/api/PartlyViewable/GetAuthorized666
			 */					

			string theType	= type.Name.Replace( "Controller", string.Empty);
			string theRoute = routeAttribute.Template.Replace( "[controller]", theType);

			theRoute = $"/{theRoute}";

			RouteAttribute r2 = methodInfo.GetCustomAttribute(typeof(RouteAttribute)) as RouteAttribute;
			if( ! string.IsNullOrEmpty( r2?.Template))
			{
				theRoute += $"/{r2.Template}";
			}
					
			int parameterCount = methodInfo.GetParameters().Count();

			// --- HttpOperation (20220611 SDE)

			HttpGetAttribute httpGetAttribute = methodInfo.GetCustomAttribute(typeof(HttpGetAttribute)) as HttpGetAttribute;

			if( httpGetAttribute != null)
			{
				theRoute = getRouteFromTemplateAttribute( httpGetAttribute, theRoute);

				d.WriteLine( $"  route={theRoute}, op=Get, {parameterCount} parameters");
				return( theRoute, OperationType.Get, parameterCount);
			}

			HttpPostAttribute httpPostAttribute = methodInfo.GetCustomAttribute( typeof( HttpPostAttribute ) ) as HttpPostAttribute;

			if (httpPostAttribute != null)
			{
				theRoute = getRouteFromTemplateAttribute( httpPostAttribute, theRoute);

				d.WriteLine( $"  route={theRoute}, op=Post, {parameterCount} parameters" );
				return( theRoute, OperationType.Post, parameterCount);
			}

			HttpDeleteAttribute httpDeleteAttribute = methodInfo.GetCustomAttribute( typeof( HttpDeleteAttribute ) ) as HttpDeleteAttribute;

			if (httpDeleteAttribute != null)
			{
				theRoute = getRouteFromTemplateAttribute( httpDeleteAttribute, theRoute);

				d.WriteLine( $"  route={theRoute}, op=Delete, {parameterCount} parameters" );
				return( theRoute, OperationType.Delete, parameterCount);
			}

			HttpPatchAttribute httpPatchAttribute = methodInfo.GetCustomAttribute( typeof( HttpPatchAttribute ) ) as HttpPatchAttribute;

			if (httpPatchAttribute != null)
			{
				theRoute = getRouteFromTemplateAttribute( httpPatchAttribute, theRoute);

				d.WriteLine( $"  route={theRoute}, op=Patch, {parameterCount} parameters" );
				return( theRoute, OperationType.Patch, parameterCount);
			}

			return ( string.Empty, OperationType.Options, -1);
		}

		/// <summary>
		/// Dumps the methods with authorisation and HTTP operation.
		/// </summary>
		/// <param name="assembly">The assembly.</param>
		public static void DumpMethodsWithAuthorisationAndHttpOperation( this Assembly assembly)
		{
			/// <summary date="13-06-2022, 12:02:25" author="S.Deckers">
			/// Gets the route from template attribute.
			/// </summary>
			string getRouteFromTemplateAttribute( Microsoft.AspNetCore.Mvc.Routing.HttpMethodAttribute attribute, string route)
			{
				if( attribute.Template != null)
				{ 
					string theTemplate = attribute.Template;
					theTemplate = theTemplate.Replace( ":int", string.Empty);	// --- [HttpGet( "{id:int}" )] -> {id}
					theTemplate = theTemplate.Replace( ":long", string.Empty);	// --- [HttpGet( "{id:long}" )] -> {id}
					return( $"{route}/{theTemplate}");
				}
				return( route);
			}

			/* --- DumpMethodsWithAuthorisationAndHttpOperation entry --- */

			IEnumerable<Type> types = assembly.GetExportedTypes( ).Where( x => x.BaseType == typeof(Microsoft.AspNetCore.Mvc.Controller));

			int i = 1;

			foreach( Type type in types)
			{
				MethodInfo[] methodInfo = type.GetMethods( BindingFlags.DeclaredOnly | BindingFlags.Public | BindingFlags.Instance);

				foreach( var m in methodInfo)
				{
					// --- Route

					RouteAttribute routeAttribute = type.GetCustomAttribute(typeof(RouteAttribute)) as RouteAttribute;

					if( routeAttribute == null)
					{
						d.WriteLine( "No route attribute found on Controller [{type}]");
						continue;
					}

					/* --- If there is a route on the method it needs to be appended to the route (20220611 SDE)
					 *
					 *		type.method													route
					 *		-----------------------------------------------------------	-----------------
					 *		PartlyViewableController.GetAuthorizedMethodDefaultRoute( ) /api/PartlyViewable
					 *
					 *		[HttpGet( "{id:int}" )]
					 *		AllInOne.GetAuthorizedMethod								/api/AllInOne/{id}
					 *		
					 *		[Route( "GetAuthorized666" )]
					 *		AllInOne.GetAuthorizedMethod2222( )							/api/AllInOne/GetAuthorized666
					 */										

					string theType	= type.Name.Replace( "Controller", string.Empty);
					string theRoute = routeAttribute.Template.Replace( "[controller]", theType);

					theRoute = $"/{theRoute}";

					RouteAttribute r2 = m.GetCustomAttribute(typeof(RouteAttribute)) as RouteAttribute;
					if( ! string.IsNullOrEmpty( r2?.Template))
					{
						theRoute += $"/{r2.Template}";
					}
					
					//d.WriteLine( $"  route={theRoute}");

					int parameterCount = m.GetParameters().Count();

					// --- HttpOperation (20220611 SDE)

					HttpGetAttribute httpGetAttribute = m.GetCustomAttribute(typeof(HttpGetAttribute)) as HttpGetAttribute;

					if( httpGetAttribute != null)
					{
						theRoute = getRouteFromTemplateAttribute( httpGetAttribute, theRoute);

						d.WriteLine( $"  route={theRoute}, op=Get, {parameterCount} parameters");
						continue;
					}

					HttpPostAttribute httpPostAttribute = m.GetCustomAttribute(typeof(HttpPostAttribute)) as HttpPostAttribute;

					if( httpPostAttribute != null)
					{
						theRoute = getRouteFromTemplateAttribute( httpPostAttribute, theRoute);
						d.WriteLine( $"  route={theRoute}, op=Post, {parameterCount} parameters");
						continue;
					}

					HttpDeleteAttribute httpDeleteAttribute = m.GetCustomAttribute(typeof(HttpDeleteAttribute)) as HttpDeleteAttribute;

					if( httpDeleteAttribute != null)
					{
						if( ! string.IsNullOrEmpty( httpDeleteAttribute.Template))
						{
							theRoute += $"/{httpDeleteAttribute.Template}";
							d.WriteLine( $"  route={theRoute}, op=Delete, {parameterCount} parameters");
							continue;
						}
						d.WriteLine( $"  route={theRoute}, op=Delete, {parameterCount} parameters");
						continue;
					}

					HttpPatchAttribute httpPatchAttribute = m.GetCustomAttribute(typeof(HttpPatchAttribute)) as HttpPatchAttribute;

					if( httpPatchAttribute != null)
					{
						if( ! string.IsNullOrEmpty( httpPatchAttribute.Template))
						{
							theRoute += $"/{httpPatchAttribute.Template}";
							d.WriteLine( $"  route={theRoute}, op=Patch, {parameterCount} parameters");
							continue;
						}

						d.WriteLine( $"  route={theRoute}, op=Patch, {parameterCount} parameters");
						continue;
					}

					// --- Authorisation (20220611 SDE)

					Microsoft.AspNetCore.Authorization.AuthorizeAttribute a = (AuthorizeAttribute)Attribute.GetCustomAttribute( m, typeof (AuthorizeAttribute));
					if( a == null)
					{
						d.WriteLine( $"type=[{type.Name}], Method=[{m.Name}], No Authorisation");
						continue;
					}

					ParameterInfo[] parameterInfo = m.GetParameters();

					if( parameterInfo.Count() == 0)
					{
						d.WriteLine( $"	{i++}:{type.Name}.{m.Name}()");
						continue;
					}

					string theParameters = string.Empty;

					foreach( ParameterInfo p in parameterInfo)
					{
						if( string.IsNullOrEmpty( theParameters))
						{
							theParameters = p.Name;
							continue;
						}
						theParameters += $", {p.Name}";
					}

					d.WriteLine( $"	{i++}:{type.Name}.{m.Name}({theParameters})");
				}
			}		
		}

		/// <summary date="11-06-2022, 06:41:37" author="S.Deckers">
		/// Operations removal demo.
		/// </summary>
		/// <param name="swaggerDoc">The swagger document.</param>
		public static void OperationRemovalDemo
		( 
			this OpenApiDocument swaggerDoc
		)
		{
			d.WriteLine( string.Format( "item.Description {0}", swaggerDoc.ToString()));

/* --- swagger doc
1:key=/api/Cool, op.Key=Get, op.Parameters=0
2:key=/api/PartlyViewable, op.Key=Get, op.Parameters=0
3:key=/api/PartlyViewable, op.Key=Post, op.Parameters=0
4:key=/api/PartlyViewable, op.Key=Patch, op.Parameters=1
5:key=/api/PartlyViewable/a1, op.Key=Get, op.Parameters=0
6:key=/api/PartlyViewable/{id}, op.Key=Get, op.Parameters=1
7:key=/api/PartlyViewable/{id}, op.Key=Delete, op.Parameters=1
8:key=/api/PartlyViewable/GetAuthorized666, op.Key=Get, op.Parameters=0
9:key=/api/PartlyViewable/{id}&{key}, op.Key=Get, op.Parameters=2
10:key=/api/PartlyViewable/{id1}&{id2}, op.Key=Delete, op.Parameters=2*/

			//Microsoft.OpenApi.Models
			string			apiPath			= "/api";
			string			type			= string.Empty;
			string			methodName		= string.Empty;
			string			opArgument		= string.Empty;
			OperationType	operationType	= Microsoft.OpenApi.Models.OperationType.Get;
			int				paramCnt		= 0;

			/* --- 9:key=/api/AllInOne/a, op.Key=Post, op.Parameters=1
			[HttpPost( "a")]	
			type			= "AllInOne";
			methodName		= string.Empty;
			operationType	= OperationType.Post;
			opArgument		= "a";
			paramCnt		= 1;
			*/

			/* --- 9:key=/api/PartlyViewable/{id1}&{id2}, op.Key=Delete, op.Parameters=2
			type			= "PartlyViewable";
			methodName		= "{id1}&{id2}";
			operationType	= OperationType.Delete;
			paramCnt		= 2;
			*/

			/* --- 8:key=/api/PartlyViewable/{id}, op.Key=Get, op.Parameters=1 */
			type			= "AllInOne";
			methodName		= "{id}";
			operationType	= OperationType.Get;
			paramCnt		= 1;

			/* --- 8:key=/api/PartlyViewable/{id}&{key}, op.Key=Get, op.Parameters=2
			type			= "PartlyViewable";
			methodName		= "{id}&{key}";
			operationType	= OperationType.Get;
			paramCnt		= 2;
			*/

			/* --- 7:key=/api/PartlyViewable/GetAuthorized666, op.Key=Get, op.Parameters=0
			type			= "PartlyViewable";
			methodName		= "GetAuthorized666";
			operationType	= OperationType.Get;
			paramCnt		= 0;
			*/

			/* --- 6:key=/api/PartlyViewable/{id}, op.Key=Delete, op.Parameters=1
			type			= "PartlyViewable";
			methodName		= "{id}";
			operationType	= OperationType.Delete;
			paramCnt		= 1;
			*/

			/* --- 5:key=/api/PartlyViewable/{id}, op.Key=Get, op.Parameters=1
			type			= "PartlyViewable";
			methodName		= "{id}";
			operationType	= OperationType.Get;
			paramCnt		= 1;
			*/

			/* --- 4:key=/api/PartlyViewable/a1, op.Key=Get, op.Parameters=0
			type			= "PartlyViewable";
			methodName		= "a1";
			operationType	= OperationType.Get;
			paramCnt		= 0;
			*/

			/* --- 3:key=/api/PartlyViewable, op.Key=Patch, op.Parameters=1			
			type			= "PartlyViewable";
			operationType	= OperationType.Patch;
			paramCnt		= 1;
			*/

			/* --- 1:key=/api/PartlyViewable, op.Key=Get, op.Parameters=0
			type			= "PartlyViewable";
			operationType	= OperationType.Get;
			paramCnt		= 0;
			*/

			/* --- 2:key=/api/PartlyViewable, op.Key=Post, op.Parameters=0
			type			= "PartlyViewable";
			operationType	= OperationType.Post;
			paramCnt		= 0;
			*/

			string pathKey = $"{apiPath}/{type}";

			if( !string.IsNullOrEmpty( methodName))
			{
				pathKey = $"{apiPath}/{type}/{methodName}";
			}

			if( !string.IsNullOrEmpty( opArgument))
			{
				pathKey = $"{apiPath}/{type}/{opArgument}";
			}

			var paths = swaggerDoc.Paths.Where( x => x.Key == pathKey);

			foreach( var item in paths)
			{
				d.WriteLine( $"key={item.Key}, value={item.Value}");

				foreach( var op in item.Value.Operations)
				{
					d.WriteLine( $"op.Key={op.Key}, op.Value={op.Value}, op.Parameters={op.Value.Parameters.Count()}");

					switch( op.Key)
					{
						case( OperationType.Post):
						{
							d.WriteLine( $"Removing {item.Key}.{op.Key}, op.Value={op.Value}, ignoring op.Parameters.Count={op.Value.Parameters.Count()}");
							item.Value.Operations.Remove( op.Key);
							continue;
						}

						case( OperationType.Get):
						case( OperationType.Patch):
						case( OperationType.Delete):
						{						
							if( op.Value.Parameters.Count() == paramCnt)
							{
								d.WriteLine( $"Removing {item.Key}.{op.Key}, op.Value={op.Value}, op.Parameters={op.Value.Parameters.Count()}");
								item.Value.Operations.Remove( op.Key);
								continue;
							}

							d.WriteLine( $"Skipping:cnt={op.Value.Parameters.Count()}, paramCnt={paramCnt}");
							continue;
						}

						default:
						{ 
							throw new System.NotSupportedException( $"Unsupported type {op.Key}");
						}
					}
				}
			}
		}

		public static void RemoveOperation
		( 
			this OpenApiDocument	swaggerDoc
		,	Type					type
		,	string					pathKey
		,	OperationType			operationType
		,	int						paramCnt
		)
		{
			d.WriteLine( string.Format( "item.Description {0}", swaggerDoc.ToString()));

/* --- swagger doc
1:key=/api/Cool, op.Key=Get, op.Parameters=0
2:key=/api/PartlyViewable, op.Key=Get, op.Parameters=0
3:key=/api/PartlyViewable, op.Key=Post, op.Parameters=0
4:key=/api/PartlyViewable, op.Key=Patch, op.Parameters=1
5:key=/api/PartlyViewable/a1, op.Key=Get, op.Parameters=0
6:key=/api/PartlyViewable/{id}, op.Key=Get, op.Parameters=1
7:key=/api/PartlyViewable/{id}, op.Key=Delete, op.Parameters=1
8:key=/api/PartlyViewable/GetAuthorized666, op.Key=Get, op.Parameters=0
9:key=/api/PartlyViewable/{id}&{key}, op.Key=Get, op.Parameters=2
10:key=/api/PartlyViewable/{id1}&{id2}, op.Key=Delete, op.Parameters=2*/

		//	this OpenApiDocument	swaggerDoc
		//,	Type					type
		//,	OperationType			operation
		//,	int						parameterCount

			//string			apiPath			= "/api";
			//string			type			= string.Empty;
			//string			methodName		= string.Empty;
			//OperationType	operationType	= Microsoft.OpenApi.Models.OperationType.Get;
			//int				paramCnt		= 0;

			//type			= "Cool";
			//methodName		= string.Empty;
			//operationType	= OperationType.Get;
			//paramCnt		= 0;

			/* --- 9:key=/api/PartlyViewable/{id1}&{id2}, op.Key=Delete, op.Parameters=2
			type			= "PartlyViewable";
			methodName		= "{id1}&{id2}";
			operationType	= OperationType.Delete;
			paramCnt		= 2;
			*/

			/* --- 8:key=/api/PartlyViewable/{id}&{key}, op.Key=Get, op.Parameters=2
			type			= "PartlyViewable";
			methodName		= "{id}&{key}";
			operationType	= OperationType.Get;
			paramCnt		= 2;
			*/

			/* --- 7:key=/api/PartlyViewable/GetAuthorized666, op.Key=Get, op.Parameters=0
			type			= "PartlyViewable";
			methodName		= "GetAuthorized666";
			operationType	= OperationType.Get;
			paramCnt		= 0;
			*/

			/* --- 6:key=/api/PartlyViewable/{id}, op.Key=Delete, op.Parameters=1
			type			= "PartlyViewable";
			methodName		= "{id}";
			operationType	= OperationType.Delete;
			paramCnt		= 1;
			*/

			/* --- 5:key=/api/PartlyViewable/{id}, op.Key=Get, op.Parameters=1
			type			= "PartlyViewable";
			methodName		= "{id}";
			operationType	= OperationType.Get;
			paramCnt		= 1;
			*/

			/* --- 4:key=/api/PartlyViewable/a1, op.Key=Get, op.Parameters=0
			type			= "PartlyViewable";
			methodName		= "a1";
			operationType	= OperationType.Get;
			paramCnt		= 0;
			*/

			/* --- 3:key=/api/PartlyViewable, op.Key=Patch, op.Parameters=1			
			type			= "PartlyViewable";
			operationType	= OperationType.Patch;
			paramCnt		= 1;
			*/

			/* --- 1:key=/api/PartlyViewable, op.Key=Get, op.Parameters=0
			type			= "PartlyViewable";
			operationType	= OperationType.Get;
			paramCnt		= 0;
			*/

			/* --- 2:key=/api/PartlyViewable, op.Key=Post, op.Parameters=0
			type			= "PartlyViewable";
			operationType	= OperationType.Post;
			paramCnt		= 0;
			*/

			//string pathKey = $"{apiPath}/{type}";

			//if( !string.IsNullOrEmpty( methodName))
			//{
			//	pathKey = $"{apiPath}/{type}/{methodName}";
			//}

			var paths = swaggerDoc.Paths.Where( x => x.Key == pathKey);

			foreach( var item in paths)
			{
				d.WriteLine( $"key={item.Key}, value={item.Value}");

				foreach( var op in item.Value.Operations)
				{
					d.WriteLine( $"op.Key={op.Key}, op.Value={op.Value}, op.Parameters={op.Value.Parameters.Count()}");

					/* --- If the operation is Post/Delete the # of parameters doesn't match
					 
							Method	Reflection	Swagger
							------	-----------	-------
							Post	1			0

					*/

					switch( op.Key)
					{
						// --- HttpPost/Patch:Just remove it (20220613 SDE)

						case( OperationType.Post):
						//case( OperationType.Patch):
						case( OperationType.Delete):
						{
							d.WriteLine( $"Removing {item.Key}.{op.Key}, op.Value={op.Value}, op.Parameters={op.Value.Parameters.Count()}");
							item.Value.Operations.Remove( op.Key);
							return;
						}

						// --- For a get the # of parameters needs to match (20220613 SDE)
						
						case( OperationType.Patch):
						case( OperationType.Get):
						{
							if( op.Value.Parameters.Count() == paramCnt)
							{
								d.WriteLine( $"Removing {item.Key}.{op.Key}, op.Value={op.Value}, op.Parameters={op.Value.Parameters.Count()}");
								item.Value.Operations.Remove( op.Key);
								return;
							}

							d.WriteLine( $"Skipping:cnt={op.Value.Parameters.Count()}, paramCnt={paramCnt}");
							continue;
						}

						// --- Can't handle this (20220613 SDE)

						default:
						{ 
							throw new System.NotSupportedException( $"Unsupported type {op.Key}");
						}
					}
				}
			}
		}
	}
}
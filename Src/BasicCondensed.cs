/**
 * @Name BasicCondensed.cs
 * @Purpose TopLevel statements voor een Web-Api. Je kunt ook nog gebruik maken van de 
 * @Date 06 January 2022, 20:04:43
 * @Author S.Deckers
 * @Description 
 */

/*
namespace Whoua.Core.Api.Controllers
{
	#region -- Using directives --
	using System;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.OData.Edm;
	using Microsoft.OpenApi.Models;
	using d=System.Diagnostics.Debug;
	#endregion

	[ Microsoft.AspNetCore.Mvc.ApiController	( )]	
	[ Microsoft.AspNetCore.Mvc.Route			( "[controller]" )]
	public class FooController : Microsoft.AspNetCore.Mvc.ControllerBase
	{
		public FooController( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));			
		}

		[ Microsoft.AspNetCore.Mvc.HttpGet		( )]
		public string Get( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));			

            string theString = string.Format( "[{0}]", System.DateTime.Now.Second);
            return( theString);
		}
	}

	/// <summary date="03-06-2021, 16:07:16" author="S.Deckers">
	/// Startup class
	/// </summary>
	public class Startup
	{
		private readonly Microsoft.Extensions.Configuration.IConfiguration _configuration;

		public Startup( Microsoft.Extensions.Configuration.IConfiguration configuration)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			this._configuration = configuration;
		}

		public void ConfigureServices( Microsoft.Extensions.DependencyInjection.IServiceCollection services)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			services.AddControllers	( );
			services.AddSwaggerGen	( );

			string someValue = this._configuration.GetConnectionString( "DefaultConnection");
			d.WriteLine( string.Format( "[{0}]", someValue));

			someValue = this._configuration.GetValue<string>("AppSettings:Version");

			d.WriteLine( string.Format( "[{0}]", someValue));

			// --- int value 
			d.WriteLine( string.Format( "bitrate=[{0}]", this._configuration.GetValue<int>("AppSettings:bitrate")));
		}

		public void Configure( Microsoft.AspNetCore.Builder.IApplicationBuilder app)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			app.UseSwagger	( );
			app.UseSwaggerUI( c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });
			app.UseRouting	( );
			app.UseEndpoints( c => { c.MapControllers();	});
		}
	}

	public static class Global
	{
		public static int CallCount = 0;
	}

	/// <summary date="03-06-2021, 16:09:17" author="S.Deckers">
	/// Entry point
	/// </summary>
	public partial class Program
	{
		public static void Main( string[] args )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", typeof( Program).Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			 CreateHostBuilder( args).Build().Run( );
		}

		public static Microsoft.Extensions.Hosting.IHostBuilder CreateHostBuilder( string[] args ) =>
			Host.CreateDefaultBuilder( args )
				.ConfigureWebHostDefaults( webBuilder =>
				 {
					 webBuilder.UseStartup<Startup>( );
				 } );
	}
}
*/

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

WebApplicationBuilder builder = WebApplication.CreateBuilder( args );

// Add services to the container.

builder.Services.AddControllers			( );
builder.Services.AddEndpointsApiExplorer( );
builder.Services.AddSwaggerGen			( );

WebApplication app = builder.Build( );

app.UseSwagger			( );
app.UseSwaggerUI		( );
app.UseHttpsRedirection	( );
app.UseAuthorization	( );
app.MapControllers		( );
app.Run					( );

public static class Global
{
	public static int CallCount = 0;
}

namespace Whoua.Core.Api.Controllers
{
	#region -- Using directives --
	using System;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.OData.Edm;
	using Microsoft.OpenApi.Models;
	using d=System.Diagnostics.Debug;
	#endregion

	[ Microsoft.AspNetCore.Mvc.ApiController	( )]	
	[ Microsoft.AspNetCore.Mvc.Route			( "[controller]" )]
	public class FooController : Microsoft.AspNetCore.Mvc.ControllerBase
	{
		public FooController( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));			
		}

		[ Microsoft.AspNetCore.Mvc.HttpGet		( )]
		public string Get( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));			

            string theString = string.Format( "[{0}]", System.DateTime.Now.Second);
            return( theString);
		}
	}
}
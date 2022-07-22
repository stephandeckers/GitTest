/**
 * @Name Logging.cs
 * @Purpose 
 * @Date 16 November 2021, 18:43:31
 * @Author S.Deckers
 * @Description 
 * - DotNet core biedt geen standaard faciliteiten voor loggen naar een file
 * 
 * @url https://weblog.west-wind.com/posts/2018/Dec/31/Dont-let-ASPNET-Core-Default-Console-Logging-Slow-your-App-down
 * Loglevels : Trace/Debug/Information/Warning/Error/Critical/None
 * 
    "LogLevel": {
        "Default": "None",
        "Microsoft": "Trace",
        "Microsoft.Hosting.Lifetime": "None"
    }

"Microsoft": "Trace"
dbug: Microsoft.AspNetCore.Mvc.Infrastructure.ControllerActionInvoker[1]
      Executing controller factory for controller Whoua.Core.Api.Controllers.WeatherForecastController (Whoua.Core.Api.50)
      
    "LogLevel": {
        "Default": "None",
        "Microsoft": "None",
        "Microsoft.Hosting.Lifetime": "Trace"
    }      
    
No Messages 

 */

namespace Whoua.Core.Api
{
	internal sealed partial class AssemblyProperties
	{
		public const string Author		= "SDE Computing";
		public const string Description = "Foo Rulez";
		public const string Name		= "whatever zTuff";
		public const string Version		= "1.2";
	}
}

namespace Whoua.Core.Api.Controllers
{
	#region -- Using directives --
	using System;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Logging;	
	using Swashbuckle.AspNetCore.Annotations;

	using d = System.Diagnostics.Debug;

	#endregion

	[ ApiController	( )]
	[ Route			( "[controller]" )]
	public class WeatherForecastController : ControllerBase
	{
		private readonly	ILogger<WeatherForecastController>	_logger;

		public WeatherForecastController
		( 
			ILogger<WeatherForecastController>	logger
		)
		{			
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			this._logger = logger;
		}

		[ HttpGet( )]
		[ SwaggerOperation( Tags = new[] { "A in control" }, Summary = "Retrieve single instance" )]
		public ActionResult<string> Get( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			string s=string.Format( "Pietje Puk a {0}e", System.DateTime.Now.Second);
			_logger.LogInformation("Hello World In Values Controller");
			_logger.LogInformation( s);


			_logger.Log( LogLevel.Trace,			$"Trace message:{System.DateTime.Now}");
			_logger.Log( LogLevel.Debug,			$"Critical message:{System.DateTime.Now}");
			_logger.Log( LogLevel.Information,		$"Information message:{System.DateTime.Now}");
			_logger.Log( LogLevel.Error,			$"Error message:{System.DateTime.Now}");
			_logger.Log( LogLevel.Warning,			$"Warning message:{System.DateTime.Now}");
			_logger.Log( LogLevel.Critical,			$"Critical message:{System.DateTime.Now}");

			return( s);
		}
	}
}

namespace Whoua.Core.Api
{
	#region -- Using directives --
	using System;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;
	using Microsoft.Extensions.Logging;
	using Microsoft.OpenApi.Models;
	using d=System.Diagnostics.Debug;
	#endregion

	public class Startup
	{
		public Startup( IConfiguration configuration)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		/// <summary>
		/// ConfigureServices
		/// </summary>
		/// <param name="services"></param>
		public void ConfigureServices( IServiceCollection services )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			services.AddControllers( );

			services.AddSwaggerGen( c =>
			{
				c.SwaggerDoc( name: "v1", info: new OpenApiInfo { Title = "Generics", Version = "v1" } );

				c.EnableAnnotations();
			});
		}

		/// <summary>
		/// Configure
		/// </summary>
		/// <param name="app"></param>
		/// <param name="env"></param>
		public void Configure
		( 
			IApplicationBuilder app
		,	IWebHostEnvironment env
		,	ILoggerFactory		loggerFactory
		)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			//loggerFactory.AddConsole(LogLevel.Warning);

			app.UseDeveloperExceptionPage	( );
			app.UseSwagger					( );
			app.UseSwaggerUI				( c => c.SwaggerEndpoint( url:"/swagger/v1/swagger.json", name:"Generics v1" ) );
			app.UseHttpsRedirection			( );
			app.UseRouting					( );
			app.UseAuthorization			( );
			app.UseEndpoints				( endpoints =>	{ endpoints.MapControllers( );} );
		}
	}

	public static class Global
	{
		public static int CallCount = 0;
	}

	public partial class Program
	{
		public static void Main( string[] args )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", typeof( Program).Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			var host = CreateHostBuilder(args).Build();
			host.Run();
		}

		public static IHostBuilder CreateHostBuilder( string[] args ) =>
			Host.CreateDefaultBuilder( args )
				.ConfigureWebHostDefaults( webBuilder =>
				 {
					 webBuilder.UseStartup<Startup>( );
				 } );
	}
}
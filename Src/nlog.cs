/**
 * @Name nlog.cs
 * @Purpose 
 * @Date 04 July 2022, 08:54:23
 * @Author S.Deckers
 * @Description
 * Loglevels : Trace/Debug/Information/Warning/Error/Critical/None
 * PM> install-package NLog.web.AspNetCore
 */

namespace Whoua.Core.Api.Controllers
{
	#region -- Using directives --
	using System;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Logging;	
	using Swashbuckle.AspNetCore.Annotations;

	using NLog;
	using NLog.Web;

	using d = System.Diagnostics.Debug;
	#endregion

	[ ApiController	( )]
	[ Route			( "[controller]" )]
	public class WeatherForecastController : ControllerBase
	{
		private readonly ILogger<WeatherForecastController>	_logger;

		public WeatherForecastController
		( 
			ILogger<WeatherForecastController>	logger
		)
		{			
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			this._logger = logger;
		}

		[ HttpGet( )]
		[ SwaggerOperation( Tags = new[] { "Controller with logging" }, Summary = "Logging demo" )]
		public ActionResult<string> Get( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			string s=string.Format( "SDE:Pietje Puk a {0}e", System.DateTime.Now.Second);
			_logger.LogInformation( "SDE:Hello World In Values Controller");
			_logger.LogInformation( s);

			_logger.LogDebug( eventId:1, message:"SDE:NLog injected into HomeController");

			// --- Log using Microsoft enums (20220707 SDE)

			_logger.Log( Microsoft.Extensions.Logging.LogLevel.Trace,		$"SDE:Microsoft Trace message:{System.DateTime.Now}" );
			_logger.Log( Microsoft.Extensions.Logging.LogLevel.Debug,		$"SDE:Microsoft Debug message:{System.DateTime.Now}" );
			_logger.Log( Microsoft.Extensions.Logging.LogLevel.Information, $"SDE:Microsoft Information message:{System.DateTime.Now}" );
			_logger.Log( Microsoft.Extensions.Logging.LogLevel.Error,		$"SDE:Microsoft Error message:{System.DateTime.Now}" );
			_logger.Log( Microsoft.Extensions.Logging.LogLevel.Warning,		$"SDE:Microsoft Warning message:{System.DateTime.Now}" );
			_logger.Log( Microsoft.Extensions.Logging.LogLevel.Critical,	$"SDE:Microsoft Critical message:{System.DateTime.Now}" );

			// --- Log using Nlog enums (20220707 SDE)

			_logger.LogTrace		( message:$"Nlog Trace message:{System.DateTime.Now}" );
			_logger.LogDebug		( message:$"Nlog Debug message:{System.DateTime.Now}" );
			_logger.LogInformation	( message:$"Nlog Information message:{System.DateTime.Now}" );
			_logger.LogError		( message:$"Nlog Error message:{System.DateTime.Now}" );
			_logger.LogWarning		( message:$"Nlog Warning message:{System.DateTime.Now}" );
			_logger.LogCritical		( message:$"Nlog Critical message:{System.DateTime.Now}" );

			return ( s);
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

	using NLog;
	using NLog.Web;

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

			string srcTitle		= $"Nlog";
			string src			= "https://medium.com/projectwt/nlog-with-ilogger-in-net-6-0-web-api-fb7072d8ac6c";
			string theUrl		= @$"<a href={src} target=_blank>{srcTitle}:{System.DateTime.Now.ToString()}</a>";
			string description	= $"using nlog in a dotnet core WebApi project<br/><br/>{theUrl}";

			services.AddSwaggerGen( c =>
			{
				c.SwaggerDoc		( name: "v1", info: new OpenApiInfo { Title = srcTitle, Version = "v1", Description = description } );
				c.EnableAnnotations ( );
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
			Console.WriteLine(  "Console:Starting app");
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", typeof( Program).Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

            var logger = LogManager.Setup()
                                   .LoadConfigurationFromAppSettings()
                                   .GetCurrentClassLogger();

			try
			{
				logger.Debug("init main");
				var host = CreateHostNlogBuilder(args).Build();
				host.Run();
			}
            catch( Exception exception)
            {
                //NLog: catch setup errors
                logger.Error(exception, "Stopped program because of exception");
                throw;
            }
            finally
            {
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                NLog.LogManager.Shutdown();
            }
		}

		public static IHostBuilder CreateHostNlogBuilder( string[] args ) =>
			Host.CreateDefaultBuilder( args )
				.ConfigureWebHostDefaults( webBuilder =>
				 {
					 webBuilder.UseStartup<Startup>( );
				 } )
			 .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
                })
                .UseNLog();
	}
}
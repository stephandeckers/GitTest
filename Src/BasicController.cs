/**
 * @Name BasicController.cs
 * @Purpose 
 * @Date 22 July 2022, 21:28:13
 * @Author S.Deckers
 * @Description 
 */

namespace Whoua.Core.Api.Controllers
{
	#region -- Using directives --
	using System;
    using System.Text;
	using System.Collections.Generic;
	using System.Linq;
    using Microsoft.AspNetCore.Mvc;
	using Swashbuckle.AspNetCore.Annotations;	
	using d=System.Diagnostics.Debug;
	#endregion

	[ApiController]
	[Route( "api/[controller]" )]
	public class WeatherForecastController : ControllerBase
	{
		private static readonly string[] strings = new[]
		{
			"one", "two", "three", "four"
		};

		private const string swaggerControllerDescription = "Weather forecast";

		[ HttpGet			( )]
		[ SwaggerOperation	( Tags = new[] { swaggerControllerDescription }, Summary = "Get All items" )]
		public IEnumerable<string> Get( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			return (strings).ToArray( );
		}
	}
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
	using System.IO;
	using System.Collections;
	using System.Linq;
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;
	using Microsoft.OpenApi.Models;
	using Whoua.Core.Api.Logging;

	using d = System.Diagnostics.Debug;
	#endregion

	public static class Global
	{
		public static int	 CallCount		= 0;
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
		public	const		string			PRXCORS = "PrxCors";
		private readonly	LoggerManager	_logger;

		public Startup( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			string logLevel = "Debug";
			_logger = new LoggerManager( logLevel, logLevel);
		}

		public void Configure( IApplicationBuilder app, IWebHostEnvironment env )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			app.UseDeveloperExceptionPage	( );
            app.UseHttpsRedirection			( );
            app.UseRouting					( );
            app.UseAuthorization			( );
            app.UseEndpoints				( e => { e.MapControllers(); });
            app.UseAuthentication			( );
			app.UseCors						( Startup.PRXCORS);
            app.UseSwagger					( );
            app.UseSwaggerUI				( c => 
			{ 
				c.SwaggerEndpoint			( "/swagger/v1/swagger.json", "Foo API v1"); 
				c.DisplayOperationId		( );
				//c.EnableTryItOutByDefault	( ); // --- only available in install-package Swashbuckle.AspNetCore -version 6.1.3
			});
		}

		public void ConfigureServices( IServiceCollection services )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}.{3}):{4}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, System.Threading.Thread.CurrentThread.ManagedThreadId, Global.CallCount++, string.Empty));

			services.AddSingleton<ILoggerManager>(_logger);

            services.AddCors(options =>
            {
                options.AddPolicy(name: PRXCORS,
                    builder =>
                    {
                        builder
                        .SetIsOriginAllowed	( origin => true) // allow any origin
                        .AllowAnyHeader		( )
                        .AllowAnyMethod		( )
                        .AllowCredentials	( );
                    });
            });

			//services.RegisterDiContainers();
			_logger.Debug("Configuring services");

			services.AddHttpContextAccessor	( );

			string srcTitle		= $"Basic controller";
			string src			= "no source";
			string theUrl		= @$"<a href={src} target=_blank>{srcTitle}</a>";
			string description	= $"Build date:{System.DateTime.Now.ToString()}";

			services.AddControllers			( );
			
			services.AddSwaggerGen( c =>
			{
				c.SwaggerDoc		( name: "v1", info: new OpenApiInfo { Title = srcTitle, Version = "v1", Description = description } );
				c.EnableAnnotations								( );
			});
		}
	}
}

namespace Whoua.Core.Api.Logging
{
	#region -- Using directives --
	using Microsoft.AspNetCore.Http;
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	#endregion

    public interface ILoggerManager
    {
        void Info			( string message, string context = "");
        void Warn			( string message, string context = "");
        void Debug			( string message, string context = "");
        void Error			( string message, string context = "");
        void Trace			( string message, string context = "");
        void LogWebRequest	( Type wrapperType, HttpContext httpContext, bool hasResponse, long? responseTimeMs = null);

        public IDictionary<string, string> GetLogLevels();

        public bool IsTraceEnabled	{ get; }
        public bool IsDebugEnabled	{ get; }
        public bool IsInfoEnabled	{ get; }
        public bool IsWarnEnabled	{ get; }
        public bool IsErrorEnabled	{ get; }
    }

	public class LoggerManager : ILoggerManager
    {
        const string LOGGERNAME = "CustomLogger";

        public LoggerManager() : this(fileLogLevel: "Info", consoleLogLevel: "Debug")
        { }

        public LoggerManager(string fileLogLevel, string consoleLogLevel)
        { }

        public bool IsTraceEnabled	{ get => true; }
        public bool IsDebugEnabled	{ get => true; }
        public bool IsInfoEnabled	{ get => true; }
        public bool IsWarnEnabled	{ get => true; }
        public bool IsErrorEnabled	{ get => true; }

        public IDictionary<string,string> GetLogLevels()
        {
            IDictionary<string, string> loglevels = new Dictionary<string, string>();

            return loglevels;
        }

        public void Trace	( string message, string context = "")		{ }
        public void Debug	( string message, string context = "")		{ }
        public void Info	( string message, string context="")		{ }
        public void Warn	( string message, string context = "")		{ }
        public void Error	( string message, string context = "")		{ }
        public void LogWebRequest( Type wrapperType, HttpContext httpContext,  bool hasResponse, long? responseTimeMs = null) { }
    }
}

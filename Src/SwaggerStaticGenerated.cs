/**
 * @Name SwaggerStaticGenerated.cs
 * @Purpose Generate Swagger build time, not runtime
 * @Date 27 October 2021, 06:51:56
 * @Author S.Deckers
 * @url https://khalidabuhakmeh.com/generate-aspnet-core-openapi-spec-at-build-time
 * @Description Werkt niet met VS 2022 preview geinstalleerd, is wel ok bij PXS
 */

namespace Whoua.Core.Api
{
	internal sealed partial class AssemblyProperties
	{
		public const string Author		= "SDE Computing";
		public const string Description = "Foo Rulez";
		public const string Name		= "SwaggerDemo";
		public const string Version		= "1.2";
	}
}

namespace Whoua.Core.Api.Controllers
{
	#region -- Using directives --
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.AspNetCore.Mvc;
	using Whoua.Core.Api.Model;
	using Microsoft.Extensions.Logging;
	using d = System.Diagnostics.Debug;
	#endregion

	[ ApiController	( )]
	[ Route			( "[controller]" )]
	public class FooController : ControllerBase
	{
		[ HttpGet( )]
		public ActionResult<string> YouCanGiveAnyNameToThisMethod( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			string s=string.Format( "Pietje Puk de {0}e", System.DateTime.Now.Second);
			return( s);
		}

		[ HttpPost( )]
		public IActionResult SamplePost( string s1)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			string s= string.Format( "[{0}] was posted", s1);
			d.WriteLine( s);

			return( Ok( s));
		}

		[ HttpDelete( )]
		public IActionResult SampleDelete( string arg)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			string s= string.Format( "[{0}] was deleted", arg);
			d.WriteLine( s);

			return( Ok( s));
		}

		[ HttpPut( )]
		public IActionResult SamplePut( string arg)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			string s= string.Format( "[{0}] was put", arg);
			d.WriteLine( s);

			return( Ok( s));
		}

		[ HttpPatch( )]
		public IActionResult SamplePatch( string p)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			string s= string.Format( "[{0}] was patched", p);
			d.WriteLine( s);

			return( Ok( s));
		}
	}

	[ ApiController	( )]
	[ Route			( "[controller]" )]
	public class WeatherForecastController : ControllerBase
	{
		private static readonly string[] Summaries = new[]
		{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};

		private readonly ILogger<WeatherForecastController> _logger;

		public WeatherForecastController( ILogger<WeatherForecastController> logger )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			_logger = logger;
		}

		[ HttpGet]
		public IEnumerable<WeatherForecast> Get( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			var rng = new Random( );
			return Enumerable.Range( 1, 5 ).Select( index => new WeatherForecast
			{
				Date = DateTime.Now.AddDays( index ),
				TemperatureC = rng.Next( -20, 55 ),
				Summary = Summaries[rng.Next( Summaries.Length )]
			} )
			.ToArray( );
		}
	}
}

namespace Whoua.Core.Api.Model
{
	#region -- Using directives --
	using System;
	using d=System.Diagnostics.Debug;
	#endregion

	public class WeatherForecast
	{
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
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;
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

			services.AddSwaggerGen( swagger =>
			{
				string nameOnPage		= "Cool WepApi";
				string versionOnPage	= "666";
				swagger.SwaggerDoc( name: "v1", info: new OpenApiInfo { Title = nameOnPage, Version = versionOnPage } );
			});
		}

		/// <summary>
		/// Configure
		/// </summary>
		/// <param name="app"></param>
		/// <param name="env"></param>
		public void Configure( IApplicationBuilder app, IWebHostEnvironment env )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage( );
				app.UseSwagger( );
				app.UseSwaggerUI( c => c.SwaggerEndpoint( url:"/swagger/v1/swagger.json", name:"WebApplication1000 v1" ) );
			}

			app.UseHttpsRedirection	( );
			app.UseRouting			( );
			app.UseAuthorization	( );

			app.UseEndpoints( endpoints =>
			{
				 endpoints.MapControllers( );
			} );
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
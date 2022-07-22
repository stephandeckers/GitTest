/**
 * @Name SwaggerDemo.cs
 * @Purpose 
 * @Date 03 June 2021, 10:59:11
 * @Author S.Deckers
 * @Description 
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

		/// <summary>
		/// Put werkt via de uri : http://localhost:14875/Foo?s=aaa
		/// </summary>
		/// <param name="s"></param>
		[ HttpPut( )]
		public IActionResult SamplePut( string arg)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			string s= string.Format( "[{0}] was put", arg);
			d.WriteLine( s);

			return( Ok( s));
		}

		/// <summary>
		/// Patch werkt via de body
		/// </summary>
		/// <param name="s"></param>
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
				//swagger.DescribeAllParametersInCamelCase();

				string nameOnPage		= "Cool WepApi";
				string versionOnPage	= "666";

				swagger.SwaggerDoc( name: "v1", info: new OpenApiInfo { Title = nameOnPage, Version = versionOnPage } );
				//swagger.SwaggerDoc( name: "v1", info: new OpenApiInfo { Title = "WebApplication2", Version = "v1111" } );
				swagger.SwaggerDoc( name: "v2", info: new OpenApiInfo { Title = "WebApplication2", Version = "v2" } );
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

			if( env.IsDevelopment( ))
			{
				app.UseDeveloperExceptionPage( );
				app.UseSwagger( );

				//string theUrl			= "/swagger/v1/swagger.json";
				//string dropDownVersion	= "WebApplication1000 v1";

				app.UseSwaggerUI( c => c.SwaggerEndpoint( url:"/swagger/v1/swagger.json", name:"WebApplication1000 v1" ) );
				//app.UseSwaggerUI( c => c.SwaggerEndpoint( url:"http://www.xs4all.nl", name:"Foo v5" ) );
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

	public class Foo
	{
		public void DoIt()
		{
			d.WriteLine( "Doing it");
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
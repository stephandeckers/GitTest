/**
 * @Name pxs6.cs
 * @Purpose 
 * @Date 25 April 2022, 12:18:33
 * @Author S.Deckers
 * @Description 
 * 
 * https://localhost:5001/WeatherForecast?$filter=Summary%20eq%20%27Freezing%27
 * 
 */

namespace Whoua.Core.Api.Controllers
{
	#region -- Using directives --
	using System;
	using Microsoft.Extensions.Logging;
	using System.Text;
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.AspNetCore.Mvc;
	using Whoua.Core.Data;
	
	using Swashbuckle.AspNetCore.Annotations;

	using d = System.Diagnostics.Debug;
	#endregion

	[ApiController( )]
	[Route( "[controller]" )]
	public class Controller : ControllerBase
	{
		public Controller( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );
		}

		[HttpGet( )]
		[SwaggerOperation( Tags = new[] { "Doit" }, Summary = "Just does it" )]
		public ActionResult<string> Get( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			string s = string.Format( "Get executed:{0}", System.DateTime.Now.Second );
			return (s);
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

        public WeatherForecastController(ILogger<WeatherForecastController> logger)
        {
            _logger = logger;
        }

        [ HttpGet( )]
        [ Microsoft.AspNetCore.OData.Query.EnableQuery()]
        public IEnumerable<WeatherForecast> Get()
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }
    }
}

namespace Whoua.Core.Data
{
	using System;

    public class WeatherForecast
    {
        public DateTime	Date			{ get; set; }
        public int		TemperatureC	{ get; set; }
        public int		TemperatureF => 32 + (int)(TemperatureC / 0.5556);
        public string	Summary			{ get; set; }
    }
}

namespace Whoua.Core.Api
{
	#region -- Using directives --
	using System;
	using System.IO;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.AspNetCore.StaticFiles;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.FileProviders;
	using Microsoft.Extensions.Hosting;
    using Microsoft.OpenApi.Models;
	using Microsoft.AspNetCore.OData;

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
			
			//services.AddControllers( ); // --- Requires entitysets to have a PK
			services.AddControllers().AddOData(options => options.Select().Filter().OrderBy());

			services.AddSwaggerGen( c =>
			{
				c.SwaggerDoc( name: "v1", info: new OpenApiInfo { Title = "PXS6", Version = "v1" } );
				c.EnableAnnotations();
			});
		}

		public void Configure( IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseDeveloperExceptionPage	( );
			app.UseHttpsRedirection			( );        
			app.UseStaticFiles				( );        
			app.UseRouting					( );
			app.UseAuthorization			( );

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute( name: "default", pattern: "{controller=Home}/{action=Index}/{id?}");
			});

			app.UseSwagger	( );
			app.UseSwaggerUI( c => c.SwaggerEndpoint( url:"/swagger/v1/swagger.json", name:"PXS6" ) );
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

			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder( string[] args ) =>
			Host.CreateDefaultBuilder( args )
				.ConfigureWebHostDefaults( webBuilder =>
				 {
					 webBuilder.UseStartup<Startup>( );
				 } );
	}
}

/* --- Minimal api :
using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder( args );

// Add services to the container.

//builder.Services.AddControllers			( );
builder.Services.AddControllers			( )
builder.Services.AddEndpointsApiExplorer( );
builder.Services.AddSwaggerGen			( );

var app = builder.Build( );

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment( ))
{
	app.UseSwagger( );
	app.UseSwaggerUI( );
}

app.UseAuthorization( );
app.MapControllers	( );
app.Run				( );

public static class Global
{
	public static int CallCount = 0;
}

namespace Whoua.Core.Api.Controllers
{	
	#region -- Using directives --
	using System;
	using System.Text;
	using System.Collections.Generic;
	using System.Linq;
	using Microsoft.AspNetCore.Mvc;
	
	using Swashbuckle.AspNetCore.Annotations;

	using d = System.Diagnostics.Debug;
	#endregion

	public class WeatherForecast
	{
		public DateTime	Date				{ get; set; }
		public int		TemperatureC		{ get; set; }
		public int		TemperatureF => 32 + (int)(TemperatureC / 0.5556);
		public string	Summary				{ get; set; }
	}

	[ ApiController	( )]
	[ Route			( "[controller]" )]
	public class Controller : ControllerBase
	{
		public Controller( )
		{			
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );
		}

		[ HttpGet( )]
		[ SwaggerOperation( Tags = new[] { "Doit" }, Summary = "Just does it" )]
		public ActionResult<string> Get( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			string s=string.Format( "Get executed:{0}", System.DateTime.Now.Second);
			return( s);
		}
	}
}
*/
/**
 * @Name IRF.Proto.cs
 * @Purpose 
 * @Date 29 May 2021, 21:50:23
 * @Author S.Deckers
 * @Description 
 */

namespace Whoua.Core.Api.Controllers
{
	#region -- Using directives --
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Mvc;
	using PXS.IRF.Data.Model;
	using Whoua.Core.Api.Model;
	using Microsoft.Extensions.Logging;
	using d=System.Diagnostics.Debug;
	#endregion

	/// <summary>
	/// https://localhost:44321/SomeApi
	/// </summary>
	[ ApiController]
	[ Route( "[controller]" )]
	public class SomeApiController : ControllerBase
	{
		private static readonly string[] Summaries = new[]
		{
			"Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
		};

		private readonly ILogger<SomeApiController> _logger;

		public SomeApiController( ILogger<SomeApiController> logger )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));			

			_logger = logger;
		}

		[ HttpGet( )]
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

	/// <summary>
	/// https://localhost:44321/IrfPopAssembly
	/// </summary>
	[ ApiController]
	[ Route( "[controller]" )]
	public class IrfPopAssemblyController : ControllerBase
	{
		private readonly ILogger<IrfPopAssemblyController> _logger;

		public IrfPopAssemblyController( ILogger<IrfPopAssemblyController> logger )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));			

			_logger = logger;
		}

		[ HttpGet( )]
		public string Get( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));			

			return( "Though rule");
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

namespace PXS.IRF.Data.Model
{
	using System;
	using System.Collections.Generic;

	#nullable disable

	/// <summary>
	/// Generated
	/// </summary>
    public partial class IrfRackspace
    {
        public decimal RackspaceId { get; set; }
        public decimal? FrameId { get; set; }
        public decimal? PopId { get; set; }
    }
}

namespace Whoua.Core.Api
{
	#region -- Using directives --
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Threading.Tasks;
	using Microsoft.AspNetCore.Builder;
	using Microsoft.AspNetCore.Hosting;
	using Microsoft.AspNetCore.HttpsPolicy;
	using Microsoft.AspNetCore.Mvc;
	using Microsoft.Extensions.Configuration;
	using Microsoft.Extensions.DependencyInjection;
	using Microsoft.Extensions.Hosting;
	using Microsoft.Extensions.Logging;
	//using Microsoft.OpenApi.Models;
	//using Microsoft.EntityFrameworkCore;

	//using Whoua.Core.Api.Data;

	using d=System.Diagnostics.Debug;
	#endregion

	public static class Global
	{
		public static int CallCount = 0;
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
		public Startup( IConfiguration configuration )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));			

			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices( IServiceCollection services )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));			

			//services.AddDbContext<TodoContext>( opt =>  opt.UseInMemoryDatabase("TodoList"));

			services.AddControllers( );
			//services.AddSwaggerGen( c =>
			// {
			//	 c.SwaggerDoc( "v1", new OpenApiInfo { Title = "WebApplication4", Version = "v1" } );
			// } );
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure( IApplicationBuilder app, IWebHostEnvironment env )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));			

			if (env.IsDevelopment( ))
			{
				app.UseDeveloperExceptionPage( );
				//app.UseSwagger( );
				//app.UseSwaggerUI( c => c.SwaggerEndpoint( "/swagger/v1/swagger.json", "WebApplication4 v1" ) );
			}

			app.UseHttpsRedirection	( );
			app.UseRouting			( );
			app.UseAuthorization	( );

			app.UseDefaultFiles( );
			app.UseStaticFiles( );

			app.UseEndpoints( endpoints =>
			 {
				 endpoints.MapControllers( );
			 } );
		}
	}
}
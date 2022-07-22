/**
 * @Name CustomCollections.cs
 * @Purpose 
 * @Date 10 September 2021, 07:02:13
 * @Author S.Deckers
 * @Description 
 */

namespace Whoua.Core.Api.Controllers
{
	#region -- Using directives --
	using System;
	using System.Text;	
	using System.Linq;
	using System.Collections.Generic;

	using Microsoft.AspNetCore.Mvc;
	
	using Swashbuckle.AspNetCore.Annotations;

	using Whoua.Core.Api;

	using d = System.Diagnostics.Debug;
	using System.ComponentModel.DataAnnotations;
	#endregion

	internal static class Swagger
	{
		internal static string Title = "Custom Collections";
	}

	/// <summary>
	/// Doordat je hier een ServiceFilter-attribuut declareerd wordt deze aangeroepen
	/// </summary>
	/// <seealso cref="Microsoft.AspNetCore.Mvc.ControllerBase" />
	[ ApiController	( )]
	[ Route			( "[controller]" )]
	public class Controller : ControllerBase
	{
		[ HttpGet			( )]
		[ SwaggerOperation	( Tags = new[] {  "Get Person" }, Summary = "Get Person summary" )]
		public ActionResult<Person> Get1( int personId)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			if( personId == 10)
			{
				Person p1 = new Person( ){ Id = personId, Name = "Willem Puk", BSN = "12345B" };

				List<Error> errors = new List<Error>() { new Error( ) { Id = 1, Issue = "Error 1" }, new Error( ) { Id = 2, Issue = "Error 2" } };
				p1.Errors = errors; 
				return( p1);
			}

			Person p = new Person( ){ Id = 1, Name = "Pietje Puk", BSN = "12345A" };
			return( p);
		}

		[ HttpPatch			( )]
		[ SwaggerOperation	( Tags = new[] { "Patch" }, Summary = "Patch summary" )]
		public ActionResult<string> Patch
		( 
			[FromBody] Person	a
		,	long?					order = null
		)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			// --- Take action if order is set (20210903 SDE)

			if( order.HasValue)
			{
				d.WriteLine( $"Taking action:{ order.Value }");
			}
			else
			{
				d.WriteLine( $"No action");
			}

			string s=string.Format( "Patch executed:{0}", System.DateTime.Now.Second);
			return( s);
		}
	}

	public class Error
	{
		public long		Id		{ get; set; }
		public string	Issue	{ get; set; }
	}

	public class Person
	{
		public long					Id		{ get; set; }
		public string				Name	{ get; set; }
		public string				BSN		{ get; set; }
		public IEnumerable<Error>	Errors	{ get; set; }
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
	using Microsoft.Extensions.Hosting;
    using Microsoft.OpenApi.Models;

	using Whoua.Core.Api.Controllers;

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
				c.SwaggerDoc( name: "v1", info: new OpenApiInfo { Title = Swagger.Title, Version = "v1" } );
				c.EnableAnnotations();
			});
		}

		public void Configure( IApplicationBuilder app, IWebHostEnvironment env)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType().Name, System.Reflection.MethodInfo.GetCurrentMethod().Name, Global.CallCount++, string.Empty));

			app.UseDeveloperExceptionPage	( );
			app.UseHttpsRedirection			( );        
			app.UseStaticFiles				( );        
			app.UseRouting					( );
			app.UseAuthorization			( );

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
			});

			app.UseSwagger( );
			app.UseSwaggerUI( c => c.SwaggerEndpoint( url:"/swagger/v1/swagger.json", name:"Dropdown title" ) );
		}
	}

	public static class Global	{	public static int CallCount = 0;	}

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
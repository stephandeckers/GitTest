/**
 * @Name SwaggerDiscovery.cs
 * @Purpose 
 * @Date 31 July 2021, 10:13:45
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

	using d = System.Diagnostics.Debug;
	#endregion

	[ ApiController	( )]
	[ Route			( "[controller]" )]
	public class Controller : ControllerBase
	{
		public Controller( )
		{			
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );
		}

/*
 		//
		// Summary:
		//     A short summary of what the operation does. For maximum readability in the swagger-ui,
		//     this field SHOULD be less than 120 characters.
		public string Summary { get; set; }
		//
		// Summary:
		//     A verbose explanation of the operation behavior. GFM syntax can be used for rich
		//     text representation.
		public string Description { get; set; }
		//
		// Summary:
		//     Unique string used to identify the operation. The id MUST be unique among all
		//     operations described in the API. Tools and libraries MAY use the operationId
		//     to uniquely identify an operation, therefore, it is recommended to follow common
		//     programming naming conventions.
		public string OperationId { get; set; }
		//
		// Summary:
		//     A list of tags for API documentation control. Tags can be used for logical grouping
		//     of operations by resources or any other qualifier.
		public string[] Tags { get; set; }
*/

		//string summary = null, string description = null
		[ HttpGet			( )]
		[ Route				( "Home/Get2")]
		//[ SwaggerOperation	( Tags = new[] {  "Optional parameters" }, Summary = "Complex arguments" )]
		[ SwaggerOperation	
		( Summary		= "Method summary"
		, Description	= "Method description verschijnt niet echt"
		, OperationId = "Operation Id verschijnt niet echt"
		, Tags = new string[] { "A1", "A2", "A3", "A4" }
		)]
		public ActionResult<string> Get2
		( 
			string requiredArg
		)
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			SwaggerOperationAttribute o = new SwaggerOperationAttribute( )
			{
				Summary		= "Ze Summary"
			,	Description = "Ze Description"
			,	OperationId = "Ze OperationId"
			,	Tags		= new string[] { "T1", "T2" }
			};

			string s=string.Format( "Get executed:{0}", System.DateTime.Now.Second);
			return( s);
		}

		//2do:post
		// - complex args post
	}

	public class ComplexType
	{
		public string	Id		{ get; set; }
		public string	Name	{ get; set; }
		public string	City	{ get; set; }
		public int		Age		{ get; set; }
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
				c.SwaggerDoc( name: "v1", info: new OpenApiInfo { Title = "DirectoryBrowsing", Version = "v1" } );
				c.EnableAnnotations();
			});

			services.AddDirectoryBrowser();
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
				endpoints.MapControllerRoute(
					name: "default",
					pattern: "{controller=Home}/{action=Index}/{id?}");
			});

			app.UseSwagger( );
			app.UseSwaggerUI( c => c.SwaggerEndpoint( url:"/swagger/v1/swagger.json", name:"Directory Browsing" ) );

			// --- Middels de volgende constructie kun je folder wwwroot/images benaderen middels /images, als
			//     folder wwwroot/images er niet is krijg je een exception tijdens opstarten

			app.UseDirectoryBrowser( new DirectoryBrowserOptions
			{
				FileProvider = new PhysicalFileProvider( Path.Combine( Directory.GetCurrentDirectory( ), "wwwroot", "images" ) ),
				RequestPath = "/images"
			} );

			app.UseDirectoryBrowser( new DirectoryBrowserOptions
			{
				FileProvider = new PhysicalFileProvider( Path.Combine( Directory.GetCurrentDirectory( ), "wwwroot" ) ),
				RequestPath = "/root"
			} );

			string logPath = Path.Combine( Directory.GetCurrentDirectory( ), "logs");

			var provider = new FileExtensionContentTypeProvider();
			provider.Mappings[".log"] = "text/html";

			if( System.IO.Directory.Exists( logPath))
			{
				app.UseStaticFiles(new StaticFileOptions
				{
					FileProvider = new PhysicalFileProvider(
						Path.Combine(Directory.GetCurrentDirectory(), "logs")),
					RequestPath = "/logs",
					ContentTypeProvider = provider
				});

				app.UseFileServer(new FileServerOptions
				{
					FileProvider = new PhysicalFileProvider(
						Path.Combine(Directory.GetCurrentDirectory(), "logs")),
					RequestPath = "/logs",
					EnableDirectoryBrowsing = true
				});
			}
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
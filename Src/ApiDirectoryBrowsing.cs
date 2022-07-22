﻿/**
 * @Name DirectoryBrowsing.cs
 * @Purpose 
 * @Date 17 July 2021, 14:51:10
 * @Author S.Deckers
 * @Description 
 * @url https://docs.microsoft.com/en-us/aspnet/core/fundamentals/static-files?view=aspnetcore-2.2#serve-files-outside-of-web-root
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

	using Whoua.Core.Services;

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

		[ HttpGet( )]
		[ SwaggerOperation( Tags = new[] { "A in control" }, Summary = "/images retrieves folder wwwwroot/images, /root lists /" )]
		public ActionResult<string> Get( )
		{
			d.WriteLine( string.Format( "{0}.{1} ({2}):{3}", GetType( ).Name, System.Reflection.MethodInfo.GetCurrentMethod( ).Name, Global.CallCount++, string.Empty ) );

			string s=string.Format( "Get executed:{0}", System.DateTime.Now.Second);
			return( s);
		}
	}
}

namespace Whoua.Core.Services
{
	#region -- Using directives --
	using System;
	using Whoua.Core.Api;
	using d=System.Diagnostics.Debug;
	#endregion
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

	using Whoua.Core.Services;

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